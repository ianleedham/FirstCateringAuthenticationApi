using System;
using System.IO;
using System.Reflection;
using System.Text;
using DAL;
using DAL.Interfaces;
using DAL.Models;
using DAL.Repositories;
using FirstCateringAuthenticationApi.Interfaces;
using FirstCateringAuthenticationApi.Managers;
using FirstCateringAuthenticationApi.Mappings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

#pragma warning disable 1591

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
            // allow cross site origin requests
            services.AddCors(x => x.AddDefaultPolicy(builder => builder.AllowAnyOrigin().Build()));
            // add the controllers
            services.AddControllers();
            // add the database context getting the connection settings from appaettings.json
            services.AddDbContext<AuthenticationContext>(o => o.
                UseSqlServer(Configuration.GetConnectionString("Default")));

            // add identity with password rules to allow a pin and storing users details in the database
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

            // create Http context access 
            services.AddHttpContextAccessor();
            
            // configure jwt authentication using issuer key from appsettings
            services.AddAuthentication(x =>
                {
                    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = true;
                    x.SaveToken = true;
                    x.IncludeErrorDetails = true;
                    x.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(Configuration["Jwt:IssuerKey"]))
                    };
                });

            // add injectables
            services.AddScoped<ICardManager, CardManager>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<ITokenManager, TokenManager>();
            
            // add auto mapper with profile
            var mapper = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            }).CreateMapper();
            services.AddSingleton(mapper);
            
            // Register the Swagger generator, defining Swagger document
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "First Catering API",
                    Version = "v1",
                    Contact = new OpenApiContact
                    {
                        Name = "Ian Leedham",
                        Email = "ian.leedham@ehsdata.com",
                        Url = new Uri("https://uk.linkedin.com/in/ian-leedham"),
                    },
                });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "First Catering API");
            });
            
            // use cors
            app.UseCors(x => x.AllowAnyOrigin());
            
            // if in development show devloper exeption page when exceptions occur
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // use jwt authetication
            app.UseAuthentication();
            app.UseAuthorization();

            // add controller end points
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
