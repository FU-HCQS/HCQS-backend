
using FluentValidation;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.Common.Validator;
using Microsoft.AspNetCore.Identity;

namespace HCQS.BackEnd.API.Installers
{
    public class ValidatorInstaller : IInstaller
    {
        public void InstallService(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<HandleErrorValidator>();
            services.AddValidatorsFromAssemblyContaining<BlogRequest>();
        }
    }
}
