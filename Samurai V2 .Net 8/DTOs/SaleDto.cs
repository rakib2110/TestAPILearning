using System.ComponentModel.DataAnnotations.Schema;

namespace Samurai_V2_.Net_8.DTOs
{
    public class SaleDto
    {
        public int ItemId { get; set; }
        public int CustomerId { get; set; }
        public DateTime SalesDate { get; set; }
        public int ItemQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public bool IsActive { get; set; }
    }
}
