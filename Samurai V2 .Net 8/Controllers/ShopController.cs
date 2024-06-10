using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Samurai_V2_.Net_8.DbContexts.Models;
using Samurai_V2_.Net_8.DTOs;
using Samurai_V2_.Net_8.IRepository;
using Samurai_V2_.Net_8.Repository;
using System.Collections;
using System.Reflection.Metadata.Ecma335;

namespace Samurai_V2_.Net_8.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class ShopController : Controller
    {
        private readonly IShop _shop;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ShopController(IShop shop, IWebHostEnvironment webHostEnvironment)
        {
            _shop = shop;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var tokenResponse = await _shop.Authenticate(loginDto);
            if (tokenResponse == null)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }
            else
            {
                return Ok(new {token = tokenResponse.Token, expiration = tokenResponse.Expiration });//,expiration=tokenResponse.Expiration
            }
        }
        [Authorize]
        [HttpGet]
        [Route("VerifyToken")]
        public IActionResult VerifyToken()
        {

            return Ok(new { message = "Token is valid" });
        }

        [HttpPost]
        [Route("CreateItem")]
        public async Task<IActionResult> CreateNew([FromForm] ItemDto itemDto)
        {
            if (itemDto == null)
            {
                return BadRequest(new { message = "ItemDto is required." });
            }
            string imagePath = null;

            if (itemDto.ImageUrl != null && itemDto.ImageUrl.Length > 0)
            {
                try
                {
                    string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "Upload");

                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    // Generate a unique file name using timestamp
                    string fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Path.GetFileName(itemDto.ImageUrl.FileName);
                    string filePath = Path.Combine(uploadPath, fileName);

                    // Save the uploaded file
                    using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await itemDto.ImageUrl.CopyToAsync(fileStream);
                    }
                    // Update the imagePath variable with the file path
                    imagePath = fileName;
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to save the image.", detail = ex.Message });
                }
            }


            try
            {
                var result = await _shop.CreateItem(itemDto, imagePath);
                return StatusCode(StatusCodes.Status201Created, result);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while creating the item.", detail = ex.Message });
            }
        }
        [HttpGet]
        [Route("GetItems")]
        public async Task<IActionResult> GetItems(int id)
        {
            var response= await _shop.GetItems(id);
            if(response.Count()==0)
            {
                return StatusCode(StatusCodes.Status404NotFound,"Data not found");
            }
            else
            {
                return Ok(response);
            }
        }

        [HttpGet]
        [Route("GetAllItems")]
        public async Task<IActionResult> GetAllItems()
        {
            try
            {
                var result = await _shop.GetAllItems();
                return Ok(result);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPost]
        [Route("CreateSale")]
        public async Task<IActionResult> CreateNew(SaleDto saleDto)
        {

            if (saleDto == null)
            {
                return BadRequest(new { message = "SaleDto is required." });
            }

            try
            {
                var result = await _shop.CreateSale(saleDto);
                return StatusCode(StatusCodes.Status201Created, result);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
        [HttpPost]
        [Route("DailyPurchaseDetails")]
        public async Task<IActionResult> GetDailyPurchaseDetails([FromBody] PurchaseReportDto purchaseReportDto)
        {
            if(purchaseReportDto == null)
            {
                return BadRequest(new { message = "PurchaseReportDto is required" });
            }
            try
            {
                var result = await _shop.GetDailyPurchaseDetails(purchaseReportDto);
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
            
        }
     
    }
}
