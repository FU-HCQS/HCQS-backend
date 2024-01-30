using AutoMapper;
using DinkToPdf;
using DinkToPdf.Contracts;
using Firebase.Storage;
using Hangfire;
using HCQS.BackEnd.Common.Mapping;
using HCQS.BackEnd.Common.Util;
using OfficeOpenXml;

namespace HCQS.BackEnd.API.Installers
{
    public class LibraryInstaller : IInstaller
    {
        public void InstallService(IServiceCollection services, IConfiguration configuration)
        {
            IMapper mapper = MappingConfig.RegisterMap().CreateMapper();
            services.AddSingleton(mapper);
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddSingleton<BackEndLogger>();
            services.AddSingleton(_ => new FirebaseStorage(configuration["Firebase:Bucket"]));
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            services.AddSingleton<Utility>();
            services.AddSingleton<SD>();
            services.AddSingleton<TemplateMappingHelper>();
            services.AddHangfire(x => x.UseSqlServerStorage(configuration["ConnectionStrings:Host"]));
            services.AddHangfireServer();
        }
    }
}