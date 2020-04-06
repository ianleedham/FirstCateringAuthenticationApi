using FirstCateringAuthenticationApi.Classes;
using FirstCateringAuthenticationApi.Interfaces;
using FirstCateringAuthenticationApi.Managers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            services.AddDbContext<AuthenticationContext>(o => o.UseSqlServer("Server=IANSPC02\\MSSQLSERVER01;Database=FirstCateringAuthentication;Trusted_Connection=True;"));

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

            services.AddTransient<ICardManager, CardManager>();
            // services.AddTransient<ICardSignInManager, CardSignInManager>();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // app.UseAuthentication();
            // app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
