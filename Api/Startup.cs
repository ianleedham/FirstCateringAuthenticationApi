using DAL;
using DAL.Interfaces;
using DAL.Models;
using DAL.Repositories;
using FirstCateringAuthenticationApi.Interfaces;
using FirstCateringAuthenticationApi.Managers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace FirstCateringAuthenticationApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(x => x.AddDefaultPolicy(builder => builder.AllowAnyOrigin().Build()));
            services.AddControllers();
            services.AddDbContext<AuthenticationContext>(o => o.
                UseSqlServer("Server=IANSPC02\\MSSQLSERVER01;Database=FirstCateringAuthentication;Trusted_Connection=True;"));

            services.AddIdentityCore<IdentityCard>(o =>
            {
                o.Password.RequiredLength = 4;
                o.Password.RequireDigit = true;
                o.Password.RequireLowercase = false;
                o.Password.RequiredUniqueChars = 1;
                o.Password.RequireUppercase = false;
                o.Password.RequireNonAlphanumeric = false;
            })
                .AddEntityFrameworkStores<AuthenticationContext>();

            services.AddHttpContextAccessor();
            services.AddAuthentication(x =>
                {
                    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = true;
                    x.SaveToken = true;
                    x.IncludeErrorDetails = true;
                    x.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = false,
                        ValidateLifetime = true,
                    };
                });

            services.AddScoped<ICardManager, CardManager>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
