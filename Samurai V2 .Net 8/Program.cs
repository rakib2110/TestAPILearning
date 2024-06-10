using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Samurai_V2_.Net_8.DbContexts;
using Samurai_V2_.Net_8.DependencyContainer;
using Samurai_V2_.Net_8.IRepository;
using Samurai_V2_.Net_8.Middlewares;
using Samurai_V2_.Net_8.Repository;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);



        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();




        //Enviorment Set For Docker Container  

        var DbHost = Environment.GetEnvironmentVariable("DB_HOST");
        var DbName = Environment.GetEnvironmentVariable("DB_NAME");
        var DbPassword = Environment.GetEnvironmentVariable("DB_SA_PASSWORD");

        if (builder.Environment.IsDevelopment())
        {

            builder.Services.AddDbContext<BookContexts>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("Development")));
            builder.Services.AddDbContext<ShopSystemDbContext>(options =>
          options.UseSqlServer(builder.Configuration.GetConnectionString("Development")));

        }
        else
        {

            builder.Services.AddDbContext<BookContexts>(options =>
             options.UseSqlServer(builder.Configuration.GetConnectionString("Production")));
            builder.Services.AddDbContext<ShopSystemDbContext>(options =>
              options.UseSqlServer(builder.Configuration.GetConnectionString("Production")));
        }

        DependencyInversion.RegisterServices(builder.Services);

        builder.Services.AddCors(options =>
                        options.AddPolicy("Open", builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

        var key = builder.Configuration["Jwt:Key"];
        var issuer = builder.Configuration["Jwt:Issuer"];
        var audience = builder.Configuration["Jwt:Audience"];

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                };
            });

        builder.Services.AddAuthorization();
        builder.Services.AddScoped<IShop, ShopRepo>();
        var app = builder.Build();
        // Configure the HTTP request pipeline.
        app.UseMiddleware<ExceptionMiddleware>();
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My DEVELOPER V1");
            });
        }
        else
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My LIVE V1");
            });
        }

        app.UseCors("AllowSpecificOrigin");
        app.UseHttpsRedirection();
        app.UseCors("Open");
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}