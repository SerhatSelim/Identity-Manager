﻿using IDM.WebApi.Claim;
using IDM.WebApi.Customs;
using IDM.WebApi.Persistence.Context;
using IDM.WebApi.Persistence.Models;
using IDM.WebApi.Policy.Concretes;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;

namespace IDM.WebApi
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
            services.AddDbContext<IdmContext>(options =>
            options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<User, Role>(i=>
            {
                i.Password.RequiredLength = 5; 
                i.Password.RequireNonAlphanumeric = false; 
                i.Password.RequireLowercase = false; 
                i.Password.RequireUppercase = false; 
                i.Password.RequireDigit = false; 

                i.User.RequireUniqueEmail= true;
            }).
            AddPasswordValidator<CustomPasswordValidation>().
            AddUserValidator<CustomUserValidation>().
            AddErrorDescriber<CustomIdentityErrorDescriber>().
            AddDefaultTokenProviders().
            AddEntityFrameworkStores<IdmContext>();

            //services.ConfigureApplicationCookie(_ =>
            //{
            //    _.LogoutPath = new PathString("/User/Logout");
            //    _.LoginPath = new PathString("/User/Login");
            //    _.Cookie = new CookieBuilder
            //    {
            //        Name = "AspNetCoreIdentityExampleCookie", //Oluşturulacak Cookie'yi isimlendiriyoruz.
            //        HttpOnly = false, //Kötü niyetli insanların client-side tarafından Cookie'ye erişmesini engelliyoruz.
            //        Expiration = TimeSpan.FromMinutes(2), //Oluşturulacak Cookie'nin vadesini belirliyoruz.
            //        SameSite = SameSiteMode.Lax, //Top level navigasyonlara sebep olmayan requestlere Cookie'nin gönderilmemesini belirtiyoruz.
            //        SecurePolicy = CookieSecurePolicy.Always //HTTPS üzerinden erişilebilir yapıyoruz.
            //    };
            //    _.SlidingExpiration = true; //Expiration süresinin yarısı kadar süre zarfında istekte bulunulursa eğer geri kalan yarısını tekrar sıfırlayarak ilk ayarlanan süreyi tazeleyecektir.
            //    _.ExpireTimeSpan = TimeSpan.FromMinutes(2); //CookieBuilder nesnesinde tanımlanan Expiration değerinin varsayılan değerlerle ezilme ihtimaline karşın tekrardan Cookie vadesi burada da belirtiliyor.
            //});

            services.AddAuthorization(x =>
            {
                x.AddPolicy("TimeControl", policy => policy.Requirements.Add(new TimeRequirement()));
            });
            services.AddSingleton<IAuthorizationHandler, TimeHandler>();

            services.AddScoped<IClaimsTransformation, UserClaimProvider>();

            services.AddScoped<IClaimsTransformation, UserClaimProvider>();
            services.AddAuthorization(x => x.AddPolicy("UserClaimNamePolicy", policy => policy.RequireClaim("username", "gncy")));
            services.AddAuthorization(x => x.AddPolicy("UserClaimPositionPolicy", policy => policy.RequireClaim("pozisyon", "admin")));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(option =>
            {
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Token:Issuer"],
                    ValidAudience = Configuration["Token:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Token:SecurityKey"])),
                    ClockSkew = TimeSpan.Zero
                };
            });


            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("CoreSwagger", new OpenApiInfo
                {
                    Title = "Swagger on BluePrint IDM Framework",
                    Version = "1.0.0",
                    Description = "BluePrint IDM on (.NET Core 3.1)",
                });
            });
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

            app.UseAuthorization();

            app.UseSwagger()
          .UseSwaggerUI(c =>
          {
              c.SwaggerEndpoint("/swagger/CoreSwagger/swagger.json", "Swagger BluePrint Idm v1");
          });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
