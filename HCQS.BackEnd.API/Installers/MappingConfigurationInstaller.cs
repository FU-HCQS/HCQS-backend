using HCQS.BackEnd.Common.ConfigurationModel;

namespace HCQS.BackEnd.API.Installers
{
    public class MappingConfigurationInstaller : IInstaller
    {
        public void InstallService(IServiceCollection services, IConfiguration configuration)
        {
            var jwtConfiguration = new JWTConfiguration();
            configuration.GetSection("JWT").Bind(jwtConfiguration);
            services.AddSingleton(jwtConfiguration);

            var emailConfiguration = new EmailConfiguration();
            configuration.GetSection("Email").Bind(emailConfiguration);
            services.AddSingleton(emailConfiguration);

            var firebaseConfiguration = new FirebaseConfiguration();
            configuration.GetSection("Firebase").Bind(firebaseConfiguration);
            services.AddSingleton(firebaseConfiguration);

            var firebaseAdminSDKConfiguration = new FirebaseAdminSDK();
            configuration.GetSection("FirebaseAdminSDK").Bind(firebaseAdminSDKConfiguration);
            services.AddSingleton(firebaseAdminSDKConfiguration);

            var momoConfiguration = new MomoConfiguration();
            configuration.GetSection("Momo").Bind(momoConfiguration);
            services.AddSingleton(momoConfiguration);

            var vnPayConfiguration = new VNPayConfiguration();
            configuration.GetSection("Vnpay").Bind(vnPayConfiguration);
            services.AddSingleton(vnPayConfiguration);
        }
    }
}