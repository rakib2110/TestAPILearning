using Samurai_V2_.Net_8.DbContexts.Models;
using Samurai_V2_.Net_8.DTOs;

namespace Samurai_V2_.Net_8.IRepository
{
    public interface IShop
    {
        Task<TokenResponseDto> Authenticate(LoginDto loginDto);
        Task <string> CreateItem(ItemDto itemDto, string imagePath);
        Task<List<ItemsDto>> GetItems(int id);
        Task<List<ItemsDto>> GetAllItems();
        Task<List<SPItemDto>> GetItemById(int itemId);
        Task<List<SPItemDto>> DeleteById(int itemID);
        Task<string> CreateSale(SaleDto saleDto);
        Task<List<SaleDto>> GetSaleByID(int itemid);
        Task<List<PurchaseReportDto>> GetDailyPurchaseDetails(PurchaseReportDto purchaseReportDto);
    }
}
