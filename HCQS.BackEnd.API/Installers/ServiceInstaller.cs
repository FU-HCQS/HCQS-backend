using Hangfire;
using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Implementations;
using HCQS.BackEnd.Service.Contracts;
using HCQS.BackEnd.Service.Implementations;

namespace HCQS.BackEnd.API.Installers
{
    public class ServiceInstaller : IInstaller
    {
        public void InstallService(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IIdentityRoleRepository, IdentityRoleRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IRoleService, RoleService>();

            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IFileService, FileService>();

            services.AddScoped<IBlogRepository, BlogRepository>();
            services.AddScoped<IBlogService, BlogService>();

            services.AddScoped<ISupplierRepository, SupplierRepository>();
            services.AddScoped<ISupplierService, SupplierService>();

            services.AddScoped<INewsRepository, NewsRepository>();
            services.AddScoped<INewsService, NewsService>();

            services.AddScoped<ISampleProjectRepository, SampleProjectRepository>();
            services.AddScoped<ISampleProjectService, SampleProjectService>();

            services.AddScoped<IMaterialRepository, MaterialRepository>();
            services.AddScoped<IMaterialService, MaterialService>();

            services.AddScoped<ISupplierPriceQuotationRepository, SupplierPriceQuotationRepository>();
            services.AddScoped<ISupplierPriceQuotationService, SupplierPriceQuotationService>();

            services.AddScoped<ISupplierPriceDetailRepository, SupplierPriceDetailRepository>();
            //services.AddScoped<ISupplierPriceDetailService, SupplierPriceDetailService>();

            services.AddScoped<IStaticFileRepository, StaticFileRepository>();
            services.AddHangfire(x => x.UseSqlServerStorage(configuration["ConnectionStrings:Host"]));
            services.AddHangfireServer();
        }
    }
}