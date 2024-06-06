using System.Text.Json.Serialization;

namespace Samurai_V2_.Net_8.DTOs
{
    public class ItemsDto
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = null!;
        public int StockQuantity { get; set; }
        public string ImageUrl { get; set; }
        public IFormFile ImageFile { get; set; }
        public bool? IsActive { get; set; }
    }
}
