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
        }
    }
}