using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HCQS.BackEnd.API.Installers
{
  

    public class SwaggerInstaller : IInstaller
    {
        public void InstallService(IServiceCollection services, IConfiguration configuration)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(option =>
             {
                 option.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme, securityScheme: new OpenApiSecurityScheme
                 {
                     Name = "Authorization",
                     Description = "Enter the Bear Authorization string as following: `Bearer Generate-JWT-Token`",
                     In = ParameterLocation.Header,
                     Type = SecuritySchemeType.ApiKey,
                     Scheme = "Bearer"
                 });
                 option.AddSecurityRequirement(new OpenApiSecurityRequirement
     {
        {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = JwtBearerDefaults.AuthenticationScheme
            }
        }, new string[]{}
    }
     });
                 option.SwaggerDoc("v1", new OpenApiInfo
                 {
                     Version = "v1",
                     Title = "House construction quotation API",
                     Description = "©Copyright: Hồng Quân & Minh Khang",
                     TermsOfService = new Uri("https://example.com/terms"),
                     Contact = new OpenApiContact
                     {
                         Name = "Example Contact",
                         Url = new Uri("https://example.com/contact")
                     },
                     License = new OpenApiLicense
                     {
                         Name = "Example License",
                         Url = new Uri("https://example.com/license")
                     },
                 });
             });
        }
    }
}