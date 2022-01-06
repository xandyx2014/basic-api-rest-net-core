using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WebApiAutores.Filter;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using WebApiAutores.DTO;
using WebApiAutores.Service;
using WebApiAutores.Utils;

namespace WebApiAutores
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            // limpia la cabezera por default
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<ApplicationDbContext>(
                opt => opt.UseSqlServer(Configuration.GetConnectionString("defaultConnection")
            ));
            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(ExceptionFilter));
                //
                options.Conventions.Add(new SwaggerGroupVersion());
            }).AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
            .AddFluentValidation(fluent =>
            {
                fluent.RegisterValidatorsFromAssemblyContaining<Startup>();
            }).AddNewtonsoftJson();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(e =>
            {
                e.OperationFilter<AgregarParametroHateoas>();
                e.SwaggerDoc("v1", new OpenApiInfo() { Title = "WebApiAUtores", Version = "v1" });
                e.SwaggerDoc("v2", new OpenApiInfo() { Title = "WebApiAUtores", Version = "v2" });
                e.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header
                });
                e.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme {
                        Reference = new OpenApiReference {

                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                        },
                        Array.Empty<string>()
                    }
                });
            });
            services.AddResponseCaching();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["jwtKey"])),
                    ClockSkew = TimeSpan.Zero
                });
            services.AddTransient<FilterAction>();
            services.AddAutoMapper(typeof(Startup));
            //  services.AddHostedService<WritteFileHostedServices>();
            // configuration de Entity framework core
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireClaim("Admin"));
                // options.AddPolicy("Vendedor", policy => policy.RequireClaim("Vendedor"));
            });
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins("")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders(new string[] { "cantidadTotalRegistro" });
                });
            });
            services.AddDataProtection();
            services.AddTransient<HashService>();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            /*app.Use(async (context, next) =>
            {
                using (var ms = new MemoryStream())
                {
                    var original = context.Response.Body;
                    context.Response.Body = ms;
                    await next.Invoke();
                    ms.Seek(0, SeekOrigin.Begin);
                    string response = new StreamReader(ms).ReadToEnd();
                    ms.Seek(0, SeekOrigin.Begin);
                    await ms.CopyToAsync(original);
                    context.Response.Body = original;
                    logger.LogInformation(response);
                }
            }); */
            // app.UseMiddleware<LogResponseHttpMIddleware>();
            // app.UseLogResponseHttp();
            // forma manual
            /*app.Map("/ruta1", app =>
            {
                app.Run(async context =>
                {
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync("estoy interceptando la tube");
                });
            });*/
            // app.UseResponseCaching();
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(e =>
                {
                    e.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApiAUtores v1");
                    e.SwaggerEndpoint("/swagger/v2/swagger.json", "WebApiAUtores v2");
                });
            }

            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseCors();




            app.UseEndpoints(endpoint =>
            {
                endpoint.MapControllers();
            });
        }

    }
}
