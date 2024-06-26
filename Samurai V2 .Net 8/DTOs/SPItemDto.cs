namespace Samurai_V2_.Net_8.DTOs
{
    public class SPItemDto
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = null!;
        public int StockQuantity { get; set; }
        public string ImageUrl { get; set; }// For file upload
        public bool IsActive { get; set; }
    }
}
