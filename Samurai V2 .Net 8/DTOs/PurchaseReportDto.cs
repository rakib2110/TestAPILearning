using System.ComponentModel.DataAnnotations.Schema;

namespace Samurai_V2_.Net_8.DTOs
{
    public class PurchaseReportDto
    {
        public DateTime PurchaseDate { get; set; }

        public required string ItemName { get; set; }

        public int TotalQuantityPurchased { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotalPurchaseAmount { get; set; }

    }
}
