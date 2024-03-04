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
            services.AddValidatorsFromAssemblyContaining<ConfigProjectRequest>();
            services.AddValidatorsFromAssemblyContaining<CreateQuotationDeallingStaffRequest>();
            services.AddValidatorsFromAssemblyContaining<ProjectDto>();
            services.AddValidatorsFromAssemblyContaining<QuotationDealingDto>();
            services.AddValidatorsFromAssemblyContaining<QuotationDetailDto>();
            services.AddValidatorsFromAssemblyContaining<WorkerPriceRequest>();
            services.AddValidatorsFromAssemblyContaining<ContractProgressPaymentDto>();
            services.AddValidatorsFromAssemblyContaining<ConstructionConfigRequest>();
            services.AddValidatorsFromAssemblyContaining<SearchConstructionConfigRequest>();
            services.AddValidatorsFromAssemblyContaining<FilterConstructionConfigRequest>();
        }
    }
}