using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using AspectCore.Configuration;
using AspectCore.Extensions.DependencyInjection;
using Domain;
using Domain.Aop;
using Domain.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            this.ConfigureDatabaseSettings<IItemDatabaseSettings, ItemDatabaseSettings>(services);
            this.ConfigureDatabaseSettings<IAuctionItemDatabaseSettings, AuctionItemDatabaseSettings>(services);
            this.ConfigureDatabaseSettings<IBidDatabaseSettings, BidDatabaseSettings>(services);
            this.ConfigureDatabaseSettings<IUserDatabaseSettings, UserDatabaseSettings>(services);

            this.ConfigureClasses(services);

            services.ConfigureDynamicProxy(configure => configure.Interceptors.AddTyped<CacheAttribute>());

            services.AddControllers();

            this.ConfigureJwtAuthentication(services);

            this.ConfigureSwagger(services);
        }

        private void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Auction Backend",
                    Version = "v1"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        private void ConfigureJwtAuthentication(IServiceCollection services)
        {
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            services
                .AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
        }

        private void ConfigureClasses(IServiceCollection services)
        {
            // Some rules we follow here:
            // 1. Class should inherit from one interface only
            // 2. Class should be public and not abstract one
            // 3. Class name shouldn't end with Settings
            // 4. Class should belong to Domain, Application or Data namespace
            // 5. Api project should have reference to Domain, Application and Data projects

            var namespaces = new HashSet<string>() {
                 nameof(Domain),
                 nameof(Application),
                 nameof(Data) };

            var definitions = namespaces
                .Select(x => Assembly.Load(x))
                .SelectMany(t => t.GetTypes())
                .Where(x =>
                    x.IsClass && x.IsPublic && !x.IsAbstract &&
                    !x.Name.EndsWith("Settings") &&
                    namespaces.Contains(x.Namespace) &&
                    x.GetInterfaces().Length == 1
                    )
                .Select(x => new { implementation = x, service = x.GetInterfaces().First() })
                .ToList();

            foreach (var def in definitions)
            {
                services.AddTransient(def.service, def.implementation);
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auction Backend");
                c.RoutePrefix = string.Empty;
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    var exception = exceptionHandlerPathFeature.Error;

                    var result = JsonConvert.SerializeObject(new
                    {
                        message = exception.Message,
                    });
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(result);
                });
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(x => x//
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();//
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void ConfigureDatabaseSettings<I, T>(IServiceCollection services)
            where T : class, I, new()
            where I : class
        {
            services.Configure<T>(
               Configuration.GetSection(typeof(T).Name));

            services.AddSingleton<I>(
                sp => sp.GetRequiredService<IOptions<T>>().Value);
        }
    }
}
