using Microservices.CouponAPI;
using Microservices.CouponAPI.Data;
using Microservices.CouponAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;
namespace Microservices.CouponAPI
{
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.Text;

    // define a generics class named Student
    public class Student<T>
    {
        // define a variable of type T 
        public T data;

        // define a constructor of the Student class 
        public Student(T data)
        {
            this.data = data;
            Console.WriteLine("Data passed: " + this.data);
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            //=================Adding DbContext========================
            builder.Services.AddDbContext<MsDbContext>(option =>
            {
                option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            //=================Adding DbContext========================

            //=================Adding AutoMapper========================
            var mapper = MappingConfig.RegisterMaps().CreateMapper();
            builder.Services.AddSingleton(mapper);
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            //=================Adding AutoMapper========================

            builder.Services.AddScoped<ICouponRepository, CouponRepository>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //=================JWT Config========================
            var secret = builder.Configuration.GetValue<string>("ApiSettings:JwtOptions:Secret");
            var issuer = builder.Configuration.GetValue<string>("ApiSettings:JwtOptions:Issuer");
            var audience = builder.Configuration.GetValue<string>("ApiSettings:JwtOptions:Audience");

            var securityKey = Encoding.UTF8.GetBytes(secret!);

            builder.Services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(token =>
            {
                token.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(securityKey),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience
                };
            });

            builder.Services.AddAuthorization();

            //=================JWT Config========================

            //=================Adding Serilog========================
            builder.Host.UseSerilog((fileContext, loggingConfig) =>
            {
                loggingConfig.WriteTo.File("logs\\log.log", rollingInterval: RollingInterval.Day);
                loggingConfig.MinimumLevel.Debug();
            });
            //=================Adding Serilog========================

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            ApplyPendingMigrations();

            app.Run();


            //applies the update-database command when the project runs
            void ApplyPendingMigrations()
            {
                using var scope = app.Services.CreateScope();
                var _db = scope.ServiceProvider.GetRequiredService<MsDbContext>();

                if (_db.Database.GetPendingMigrations().Any())
                {
                    _db.Database.Migrate();

                }
            }
        }
    }
}