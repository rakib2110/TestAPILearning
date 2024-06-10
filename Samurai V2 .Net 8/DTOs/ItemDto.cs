using System.ComponentModel.DataAnnotations;

namespace Samurai_V2_.Net_8.DTOs
{
    public class ItemDto
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = null!;
        public int StockQuantity { get; set; }
        public IFormFile ImageUrl { get; set; }// For file upload
        public bool IsActive { get; set; }
    }
}
