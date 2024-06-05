using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Samurai_V2_.Net_8.DbContexts;
using Samurai_V2_.Net_8.DbContexts.Models;
using Samurai_V2_.Net_8.DTOs;
using Samurai_V2_.Net_8.IRepository;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Samurai_V2_.Net_8.Repository
{
    public class ShopRepo : IShop
    {
        private ShopSystemDbContext _context;

        public ShopRepo(ShopSystemDbContext contex)
        {
           _context = contex;
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
