using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Samurai_V2_.Net_8.DbContexts;
using Samurai_V2_.Net_8.DbContexts.Models;
using Samurai_V2_.Net_8.DTOs;
using Samurai_V2_.Net_8.IRepository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Samurai_V2_.Net_8.Repository
{
    public class ShopRepo : IShop
    {
        private ShopSystemDbContext _context;
        private readonly IConfiguration _configuration;

        public ShopRepo(ShopSystemDbContext contex, IConfiguration configuration)
        {
           _context = contex;
           _configuration = configuration;
        }
        public async Task<TokenResponseDto> Authenticate(LoginDto loginDto)
        {
            // Replace with your actual user validation logic
            if (loginDto.Username == "rakib" && loginDto.Password == "2110")
            {
                var key = _configuration["Jwt:Key"];
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                new Claim(ClaimTypes.Name, loginDto.Username)
                    }),
                    Expires = DateTime.Now.AddMinutes(30),
                    Issuer = _configuration["Jwt:Issuer"],
                    Audience = _configuration["Jwt:Audience"],
                    SigningCredentials = credentials
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                return new TokenResponseDto
                {
                    Token = tokenString,
                    Expiration = tokenDescriptor.Expires.Value
                };
            }
            return null;

        }
        public async Task<string> CreateItem(ItemDto itemDto, string imagePath)
        {
        string message = "";
            try
            {
                var item = await _context.TblItems.Where(x => x.ItemId == itemDto.ItemId).FirstOrDefaultAsync();
                if (item == null)
                {
                    // Create item
                    item = new TblItem()
                    {
                        ItemName = itemDto.ItemName,
                        StockQuantity = itemDto.StockQuantity,
                        ImageUrl = imagePath, // Store the file path
                        IsActive = itemDto.IsActive,
                    };
                    await _context.TblItems.AddAsync(item);
                    message = "Successfully Created";
                }
                else
                {
                    // Update item
                    item.ItemName = itemDto.ItemName;
                    item.StockQuantity = itemDto.StockQuantity;
                    item.ImageUrl = imagePath; // Store the file path
                    item.IsActive = itemDto.IsActive;

                    _context.TblItems.Update(item);
                    message = "Successfully Updated";
                }
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                var innerException = dbEx.InnerException?.Message;
                return $"An error occurred while saving the entity changes. Details: {innerException}";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return message;
        }

        public async Task<List<ItemsDto>> GetItems(int id)
        {

            var items = await (from a in _context.TblItems
                              where a.ItemId == id &&
                              a.IsActive == true
                              select new ItemsDto
                              {
                                  ItemId = a.ItemId,
                                  ItemName = a.ItemName,
                                  StockQuantity = a.StockQuantity,
                                  ImageUrl = a.ImageUrl,
                                  IsActive = a.IsActive,

                              }).ToListAsync();
            // Convert ImageUrl to IFormFile
            foreach (var item in items)
            {
                if (!string.IsNullOrEmpty(item.ImageUrl))
                {
                    item.ImageFile =ConvertStringToIFormFile(item.ImageUrl, item.ItemId.ToString());
                }
            }
            return items;


        }

        private IFormFile ConvertStringToIFormFile(string imageUrl, string ImageFile)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(imageUrl);
            MemoryStream stream = new MemoryStream(byteArray);

            IFormFile formFile = new FormFile(stream, 0, stream.Length, "file", ImageFile)
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/plain"
            };

            return formFile;
        }

        public async Task<List<ItemsDto>> GetAllItems()
        {
            var items = await (from a in _context.TblItems
                               select new ItemsDto
                               {
                                   ItemId = a.ItemId,
                                   ItemName = a.ItemName,
                                   StockQuantity = a.StockQuantity,
                                   ImageUrl = a.ImageUrl,
                                   IsActive = a.IsActive,
                               }).ToListAsync();

            // Convert ImageUrl to IFormFile
            foreach (var item in items)
            {
                if (!string.IsNullOrEmpty(item.ImageUrl))
                {
                    item.ImageFile = ConvertStringToIFormFile(item.ImageUrl, item.ItemId.ToString());
                }
            }
            return items;
        }

        public async Task<List<SPItemDto>> GetItemById(int itemId)
        {
            var items = new SqlParameter("@ItemId", itemId);

            var result = await _context.TblItems
                .FromSqlRaw("EXECUTE GetItemById @ItemId", items)
                .Select(item => new SPItemDto
                {
                    ItemId = item.ItemId,
                    ItemName = item.ItemName,
                    StockQuantity = item.StockQuantity,
                    ImageUrl = item.ImageUrl,
                    IsActive = item.IsActive
                })
                .ToListAsync();

            return result;
        }
        public async Task<string> CreateSale(SaleDto saleDto)
        {
            string message = "";
            try
            {
                var sale = await _context.TblSalesDetails

                    .Where(e => e.UnitPrice == saleDto.UnitPrice)
                    .FirstOrDefaultAsync();

                if (sale != null)
                {
                    var shop = await _context.TblItems
                        .Where(e => e.ItemId == saleDto.ItemId)
                        .FirstOrDefaultAsync();

                    if (shop != null)
                    {
                        var resume = shop.StockQuantity - saleDto.ItemQuantity;
                        if (resume >= 0)
                        {
                            sale.ItemId = saleDto.ItemId;
                            sale.ItemQuantity = saleDto.ItemQuantity;
                            sale.UnitPrice = saleDto.UnitPrice;
                            sale.IsActive = saleDto.IsActive;
                            _context.TblSalesDetails.Update(sale);
                            message = "Update";

                            var sales = new TblSale()
                            {
                                SalesId = sale.SalesId,
                                SalesDate =saleDto.SalesDate,
                            };
                            await _context.TblSales.AddAsync(sales);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            throw new Exception("Stock Out");
                        }
                    }
                    else
                    {
                        throw new Exception("Item not found");
                    }
                }
                else
                {
                    TblSalesDetail salesDetail = new TblSalesDetail
                    {
                        ItemId = saleDto.ItemId,
                        ItemQuantity = saleDto.ItemQuantity,
                        UnitPrice = saleDto.UnitPrice,
                        IsActive = saleDto.IsActive
                    };
                    await _context.TblSalesDetails.AddAsync(salesDetail);
                    await _context.SaveChangesAsync();

                    var sales = new TblSale()
                    {
                        SalesId = salesDetail.SalesId,
                        CustomerId=saleDto.CustomerId,
                        SalesDate = saleDto.SalesDate,
                    };
                    await _context.TblSales.AddAsync(sales);
                    await _context.SaveChangesAsync();
                    message = "created";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return message;
        }

        public async  Task<List<PurchaseReportDto>> GetDailyPurchaseDetails(PurchaseReportDto purchaseReportDto)
        {

            var purchase = await(from pd in _context.TblPurchaseDetails
                            join p in _context.TblPurchases on pd.PurchaseId equals p.PurchaseId
                            join i in _context.TblItems on pd.ItemId equals i.ItemId
                            where p.IsActive == true && pd.IsActive == true && i.IsActive == true
                            orderby p.PurchaseDate, i.ItemName
                            select new PurchaseReportDto
                            {
                                PurchaseDate = p.PurchaseDate,
                                ItemName = i.ItemName,
                                TotalQuantityPurchased = pd.ItemQuantity,
                                UnitPrice = pd.UnitPrice,
                                TotalPurchaseAmount = pd.ItemQuantity * pd.UnitPrice
                            }).ToListAsync();

            return purchase;
        }
    }
}
