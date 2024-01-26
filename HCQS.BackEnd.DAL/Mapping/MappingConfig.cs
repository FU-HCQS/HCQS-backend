using AutoMapper;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.Common.Dto.Response;
using HCQS.BackEnd.DAL.Models;
using static HCQS.BackEnd.Common.Dto.Request.CreateQuotationDeallingStaffRequest;

namespace HCQS.BackEnd.DAL.Mapping
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMap()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<BlogRequest, Blog>()
                   .ForMember(desc => desc.Id, act => act.MapFrom(src => src.Id))
                   .ForMember(desc => desc.Header, act => act.MapFrom(src => src.Header))
                   .ForMember(desc => desc.Content, act => act.MapFrom(src => src.Content))
                   .ForMember(desc => desc.AccountId, act => act.MapFrom(src => src.AccountId))
                   ;

                config.CreateMap<NewsRequest, News>()
                   .ForMember(desc => desc.Id, act => act.MapFrom(src => src.Id))
                   .ForMember(desc => desc.Header, act => act.MapFrom(src => src.Header))
                   .ForMember(desc => desc.Content, act => act.MapFrom(src => src.Content))
                   .ForMember(desc => desc.AccountId, act => act.MapFrom(src => src.AccountId))
                   ;
                config.CreateMap<SupplierRequest, Supplier>()
                  .ForMember(desc => desc.Id, act => act.MapFrom(src => src.Id))
                  .ForMember(desc => desc.SupplierName, act => act.MapFrom(src => src.SupplierName))
                  ;
                config.CreateMap<SampleProjectRequest, SampleProject>()
                .ForMember(desc => desc.Id, act => act.MapFrom(src => src.Id))
                .ForMember(desc => desc.Header, act => act.MapFrom(src => src.Header))
                .ForMember(desc => desc.EstimatePrice, act => act.MapFrom(src => src.EstimatePrice))
                .ForMember(desc => desc.ConstructionArea, act => act.MapFrom(src => src.ConstructionArea))
                .ForMember(desc => desc.Content, act => act.MapFrom(src => src.Content))
                .ForMember(desc => desc.Function, act => act.MapFrom(src => src.Function))
                .ForMember(desc => desc.Location, act => act.MapFrom(src => src.Location))
                .ForMember(desc => desc.NumOfFloor, act => act.MapFrom(src => src.NumOfFloor))
                .ForMember(desc => desc.TotalArea, act => act.MapFrom(src => src.TotalArea))
                .ForMember(desc => desc.ProjectType, act => act.MapFrom(src => src.ProjectType))
                .ForMember(desc => desc.AccountId, act => act.MapFrom(src => src.AccountId))
                ;

                config.CreateMap<StaticFile, StaticFileResponse>()
               .ForMember(desc => desc.Id, act => act.MapFrom(src => src.Id))
               .ForMember(desc => desc.StaticFileType, act => act.MapFrom(src => src.StaticFileType))
               .ForMember(desc => desc.Url, act => act.MapFrom(src => src.Url))
               .ForMember(desc => desc.StaticFileType, act => act.MapFrom(src => src.StaticFileType))
               ;

                config.CreateMap<ProjectDto, Project>()
               .ForMember(desc => desc.Id, act => act.MapFrom(src => src.Id))
               .ForMember(desc => desc.NumOfFloor, act => act.MapFrom(src => src.NumOfFloor))
               .ForMember(desc => desc.AccountId, act => act.MapFrom(src => src.AccountId))
               .ForMember(desc => desc.Area, act => act.MapFrom(src => src.Area))
                 ;

                config.CreateMap<QuotationDetailDto, QuotationDetail>()
              .ForMember(desc => desc.Id, act => act.MapFrom(src => src.Id))
              .ForMember(desc => desc.QuotationId, act => act.MapFrom(src => src.QuotationId))
              .ForMember(desc => desc.Quantity, act => act.MapFrom(src => src.Quantity))
              .ForMember(desc => desc.MaterialId, act => act.MapFrom(src => src.MaterialId))
                ;

                config.CreateMap<ExportPriceMaterialDto, ExportPriceMaterial>()
              .ForMember(desc => desc.Id, act => act.MapFrom(src => src.Id))
              .ForMember(desc => desc.Price, act => act.MapFrom(src => src.Price))
              .ForMember(desc => desc.MaterialId, act => act.MapFrom(src => src.MaterialId))
               ;

                config.CreateMap<ExportPriceMaterialDto, ExportPriceMaterial>()
              .ForMember(desc => desc.Id, act => act.MapFrom(src => src.Id))
              .ForMember(desc => desc.Price, act => act.MapFrom(src => src.Price))
              .ForMember(desc => desc.MaterialId, act => act.MapFrom(src => src.MaterialId))
                 ;
                config.CreateMap<QuotationDealingDto, QuotationDealing>()
               .ForMember(desc => desc.Id, act => act.MapFrom(src => src.Id))
               .ForMember(desc => desc.QuotationId, act => act.MapFrom(src => src.QuotationId))
               .ForMember(desc => desc.FurnitureDiscount, act => act.MapFrom(src => src.FurnitureDiscount))
               .ForMember(desc => desc.MaterialDiscount, act => act.MapFrom(src => src.MaterialDiscount))
               .ForMember(desc => desc.LaborDiscount, act => act.MapFrom(src => src.LaborDiscount))

               ;
            }); return mappingConfig;
        }
    }
}