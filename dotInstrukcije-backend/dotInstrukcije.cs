using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using dotInstrukcije.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using dotInstrukcije.Controllers;


namespace dotInstrukcije
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration["Urls"] = "http://localhost:50760";

            builder.Services.AddControllers();
            builder.Services.AddRazorPages();
            // Configure CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowMyOrigins",
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:50760", "http://localhost:5173")
                               .AllowAnyHeader()
                               .AllowAnyMethod();
                    });
            });

            builder.Services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            //builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });

            //builder.Services.AddSingleton(provider =>
            //{
            //    var config = provider.GetRequiredService<IConfiguration>();
            //    return new StudentController(
            //        provider.GetRequiredService<DataContext>(),
            //        config.GetValue<string>("Jwt:Key"),
            //        config
            //    );
            //});

            var app = builder.Build();

            app.UseHttpsRedirection();

            app.UseCors("AllowMyOrigins");

            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}


            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
            app.UseStaticFiles();
  
            app.Run();
        }
    }
}
