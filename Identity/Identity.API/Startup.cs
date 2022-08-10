using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BackEnd.Services.Jwt;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Identity.API.Services;
using Identity.API.Services.Authentication;
using Identity.API.Data.Authentication;
using Identity.API.Models;
using Identity.API.Models.Context;
using Identity.API.Services.User;
using Identity.API.Middlewares.Exceptions;

namespace BackEnd
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
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("Dev"));
            }, ServiceLifetime.Singleton);
            services.AddHttpContextAccessor();
            services.AddIdentityCore<AppUser>()
                    .AddUserManager<UserManager<AppUser>>()
                    .AddRoles<IdentityRole>()
                    .AddSignInManager<SignInManager<AppUser>>()
                    .AddRoleManager<RoleManager<IdentityRole>>()
                    .AddEntityFrameworkStores<AppDbContext>();
            services.AddTransient<UserInfoService>();

            services.Configure<IdentityOptions>(options => options.ClaimsIdentity.UserIdClaimType = ClaimTypes.NameIdentifier);

            JwtOptions jwtOptions = new JwtOptions()
            {
                Key = Configuration["Jwt:Key"],
                Issuer = Configuration["Jwt:Issuer"],
                Audience = Configuration["Jwt:Audience"],
                AccessTokenLifeTime = int.Parse(Configuration["Jwt:AccessTokenLifeTime"]),
                RefreshTokenLifeTime = int.Parse(Configuration["Jwt:RefreshTokenLifeTime"]),
            };

            services.AddSingleton<JwtOptions>(x => jwtOptions);
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key))
                };
            });

            services.AddTransient<IJwtService, JwtService>(x => new JwtService(jwtOptions));
            services.AddTransient<IAuthenticationService, AuthenticationService>();

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BackEnd", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BackEnd v1"));
            }
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
