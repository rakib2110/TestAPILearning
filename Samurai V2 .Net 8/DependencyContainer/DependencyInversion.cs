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
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder =>
                    {
                        builder.WithOrigins("http://127.0.0.1:5500") // Replace with your frontend URL
                               .AllowAnyHeader()
                               .AllowAnyMethod();
                    });
            });
            services.AddControllers();
            services.AddScoped<IShop, ShopRepo>();
            // Other service registrations
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Other configurations
            app.UseCors("AllowSpecificOrigin");
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
