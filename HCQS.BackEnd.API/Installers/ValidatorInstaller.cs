using FluentValidation;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.Common.Validator;

namespace HCQS.BackEnd.API.Installers
{
    public class ValidatorInstaller : IInstaller
    {
        public void InstallService(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<HandleErrorValidator>();
            services.AddValidatorsFromAssemblyContaining<BlogRequest>();
            services.AddValidatorsFromAssemblyContaining<NewsRequest>();
            services.AddValidatorsFromAssemblyContaining<UpdateAccountRequestDto>();

        }
    }
}