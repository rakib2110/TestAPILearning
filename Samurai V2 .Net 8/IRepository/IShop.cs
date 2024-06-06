using Samurai_V2_.Net_8.DbContexts.Models;
using Samurai_V2_.Net_8.DTOs;

namespace Samurai_V2_.Net_8.IRepository
{
    public interface IShop
    {
        Task <string> CreateItem(ItemDto itemDto, string imagePath);
        Task<List<ItemsDto>> GetItems(int id);
        Task<List<ItemsDto>> GetAllItems(ItemsDto itemsDto);
        Task<string> CreateSale(SaleDto saleDto);
        Task<List<PurchaseReportDto>> GetDailyPurchaseDetails(PurchaseReportDto purchaseReportDto);
    }
}
