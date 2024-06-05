using Samurai_V2_.Net_8.IRepository;
using Samurai_V2_.Net_8.Repository;

namespace Samurai_V2_.Net_8.DependencyContainer
{
    public class DependencyInversion
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddTransient<IBookRepo, BookRepo>();
            services.AddTransient<IShop,ShopRepo>();



        }
        //public void ConfigureServices(IServiceCollection services)
        //{
        //    services.AddControllers();
        //    services.AddScoped<IShop, ShopRepo>();
        //    // Other service registrations
        //}
    }
}
