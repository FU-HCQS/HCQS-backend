namespace HCQS.BackEnd.API.Installers
{
    public class SystemController : IInstaller
    {
        public void InstallService(IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
        }
    }
}