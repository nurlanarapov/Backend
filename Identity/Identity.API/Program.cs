using BackEnd.Services.Jwt;
using Identity.API;
using Identity.API.Extensions;
using Identity.API.Middlewares.Exceptions;
using Identity.API.Models.DbModels.Context;
using Identity.API.Models.ServiceModels;
using Identity.API.Services;
using Identity.API.Services.Authentication;
using Identity.API.Services.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Globalization;
using System.Security.Claims;
using System.Text;

{
    var builder = WebApplication.CreateBuilder(args);
    {
        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("db"), 
                               x => x.MigrationsHistoryTable("_MigrationHistory",
                                                             schema: "auth"));
        });
        builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddIdentities();
        builder.Services.AddTransient<UserService>();

        builder.Services.Configure<IdentityOptions>(options => options.ClaimsIdentity.UserIdClaimType = ClaimTypes.NameIdentifier);

        JwtOptions jwtOptions = new JwtOptions();
        builder.Configuration.GetSection("Jwt").Bind(jwtOptions);

        builder.Services.AddSingleton<JwtOptions>(x => jwtOptions);
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
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

        builder.Services.AddTransient<IJwtService, JwtService>(x => new JwtService(jwtOptions));
        builder.Services.AddTransient<IAuthenticationService, AuthenticationService>();

        builder.Services.AddControllers().AddDataAnnotationsLocalization(options =>
        {
            options.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(typeof(SharedResource));
        });

        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = $"BackEnd - {builder.Environment.EnvironmentName}", Version = "v1" });
        });
    }
    {
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BackEnd v1"));
        }

        app.UseMiddleware<ExceptionMiddleware>();
        app.UseRequestLocalization(new RequestLocalizationOptions
        {
            ApplyCurrentCultureToResponseHeaders = true
        });
        var supportedCultures = new[]
            {
                new CultureInfo("en-US"),
                new CultureInfo("ru-RU"),
            };

        app.UseRequestLocalization(new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture("ru-RU"),
            // Formatting numbers, dates, etc.
            SupportedCultures = supportedCultures,
            // UI strings that we have localized.
            SupportedUICultures = supportedCultures
        });
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
        app.Run();
    }
}