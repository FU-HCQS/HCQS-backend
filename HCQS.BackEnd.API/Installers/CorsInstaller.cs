namespace HCQS.BackEnd.API.Installers
{
    public class CorsInstaller : IInstaller
    {
        public void InstallService(IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(p => p.AddPolicy("_myAllowSpecificOrigins", builder =>
              {
                  builder.WithOrigins("http://localhost:3000").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
              }));
        }
    }
}