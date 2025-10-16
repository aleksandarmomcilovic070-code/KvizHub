using AutoMapper;
using Azure.Core;
using BCrypt.Net;
using KvizHub.Interfaces;
using KvizHub.Mapping;
using KvizHub.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IO;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using KvizHub.Infrastructure;


using KvizHub.Models;




namespace KvizHub
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private readonly string _cors = "cors";
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
        .AddJsonOptions(options =>
        {
            // Ignore circular references
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

            // Optional: make JSON output pretty
            options.JsonSerializerOptions.WriteIndented = true;
        });

            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;   // <<< add this
            });

            // Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "KvizHub", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
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
            });

            // Database
            services.AddDbContext<DbContextt>(
            options => options.UseSqlServer(Configuration.GetConnectionString("Web2Database")),
            contextLifetime: ServiceLifetime.Scoped,
            optionsLifetime: ServiceLifetime.Singleton // <-- important
            );

            // JWT Authentication
            var jwtSection = Configuration.GetSection("Jwt");
            var secretKey = jwtSection["SecretKey"];
            var issuer = jwtSection["Issuer"];
            var audience = jwtSection["Audience"];

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = ctx =>
                    {
                        // Accept token as query string for hub connections: /hubs/livequiz?access_token=JWT
                        var accessToken = ctx.Request.Query["access_token"];
                        var path = ctx.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/livequiz"))
                        {
                            ctx.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            // CORS
            services.AddCors(options =>
            {
                options.AddPolicy(name: _cors, builder =>
                {
                    builder.WithOrigins("http://localhost:3000")
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowCredentials();
                });
            });

            // Services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IQuizService, QuizService>();
            services.AddDbContextFactory<DbContextt>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("Web2Database")));

            // AutoMapper
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "KvizHub v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(_cors);

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });






        }
    }
}
