using HCQS.BackEnd.DAL.Data;
using HCQS.BackEnd.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HCQS.BackEnd.API.Installers
{
    public class DbInstaller : IInstaller
    {
        public void InstallService(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<HCQSDbContext>(option =>
            {
                option.UseSqlServer(configuration["ConnectionStrings:DB"]);
            });

            services.AddIdentity<Account, IdentityRole>().AddEntityFrameworkStores<HCQSDbContext>().AddDefaultTokenProviders();
        }
    }
}
