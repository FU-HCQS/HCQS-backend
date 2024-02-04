using AutoMapper;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.Common.Dto.Response;
using HCQS.BackEnd.DAL.Models;

namespace HCQS.BackEnd.Common.Mapping
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
                .ForMember(desc => desc.ProjectType, act => act.MapFrom(src => src.ProjectType))
                .ForMember(desc => desc.Function, act => act.MapFrom(src => src.Function))
                .ForMember(desc => desc.Location, act => act.MapFrom(src => src.Location))
                .ForMember(desc => desc.NumOfFloor, act => act.MapFrom(src => src.NumOfFloor))
                .ForMember(desc => desc.TotalArea, act => act.MapFrom(src => src.TotalArea))
                .ForMember(desc => desc.ProjectType, act => act.MapFrom(src => src.ProjectType))
                .ForMember(desc => desc.AccountId, act => act.MapFrom(src => src.AccountId))
                ;

                config.CreateMap<MaterialRequest, Material>()
              .ForMember(desc => desc.Id, act => act.MapFrom(src => src.Id))
              .ForMember(desc => desc.Name, act => act.MapFrom(src => src.Name))
              .ForMember(desc => desc.UnitMaterial, act => act.MapFrom(src => src.UnitMaterial))
              .ForMember(desc => desc.MaterialType, act => act.MapFrom(src => src.MaterialType))
              .ForMember(desc => desc.Quantity, act => act.MapFrom(src => src.Quantity));
                config.CreateMap<QuotationDetailDto, QuotationDetail>()
             .ForMember(desc => desc.Id, act => act.MapFrom(src => src.Id))
             .ForMember(desc => desc.MaterialId, act => act.MapFrom(src => src.MaterialId))
             .ForMember(desc => desc.QuotationId, act => act.MapFrom(src => src.QuotationId));

                config.CreateMap<SupplierPriceQuotationRequest, SupplierPriceQuotation>()
              .ForMember(desc => desc.Id, act => act.MapFrom(src => src.Id))
              .ForMember(desc => desc.Date, act => act.MapFrom(src => src.Date))
              .ForMember(desc => desc.SupplierId, act => act.MapFrom(src => src.SupplierId));

                config.CreateMap<ExportPriceMaterialRequest, ExportPriceMaterial>()
              .ForMember(desc => desc.Id, act => act.MapFrom(src => src.Id))
              .ForMember(desc => desc.Date, act => act.MapFrom(src => src.Date))
              .ForMember(desc => desc.Price, act => act.MapFrom(src => src.Price))
              .ForMember(desc => desc.MaterialId, act => act.MapFrom(src => src.MaterialId));

                config.CreateMap<ProgressConstructionMaterialRequest, ProgressConstructionMaterial>()
              .ForMember(desc => desc.Id, act => act.MapFrom(src => src.Id))
              .ForMember(desc => desc.Quantity, act => act.MapFrom(src => src.Quantity))
              .ForMember(desc => desc.Date, act => act.MapFrom(src => src.Date))
              .ForMember(desc => desc.QuotationDetailId, act => act.MapFrom(src => src.QuotationDetailId));

                config.CreateMap<ImportExportInventoryRequest, ImportExportInventoryHistory>()
             .ForMember(desc => desc.Id, act => act.MapFrom(src => src.Id))
             .ForMember(desc => desc.Date, act => act.MapFrom(src => src.Date))
             .ForMember(desc => desc.Quantity, act => act.MapFrom(src => src.Quantity))
             .ForMember(desc => desc.SupplierPriceDetailId, act => act.MapFrom(src => src.SupplierPriceDetailId))
             .ForMember(desc => desc.ProgressConstructionMaterialId, act => act.MapFrom(src => src.ProgressConstructionMaterialId));

                config.CreateMap<StaticFile, StaticFileResponse>()
               .ForMember(desc => desc.Id, act => act.MapFrom(src => src.Id))
               .ForMember(desc => desc.StaticFileType, act => act.MapFrom(src => src.StaticFileType))
               .ForMember(desc => desc.Url, act => act.MapFrom(src => src.Url))
               .ForMember(desc => desc.StaticFileType, act => act.MapFrom(src => src.StaticFileType))
               ;

                config.CreateMap<ProjectDto, Project>()
               .ForMember(desc => desc.Id, act => act.MapFrom(src => src.Id))
               .ForMember(desc => desc.NumOfFloor, act => act.MapFrom(src => src.NumOfFloor))
               .ForMember(desc => desc.Area, act => act.MapFrom(src => src.Area))
               .ForMember(desc => desc.ConstructionType, act => act.MapFrom(src => src.Type))
               .ForMember(desc => desc.AccountId, act => act.MapFrom(src => src.AccountId))
               ;

                config.CreateMap<WorkerPriceRequest, WorkerPrice>()
              .ForMember(desc => desc.Id, act => act.MapFrom(src => src.Id))
              .ForMember(desc => desc.LaborCost, act => act.MapFrom(src => src.LaborCost))
              .ForMember(desc => desc.PositionName, act => act.MapFrom(src => src.PositionName))
              ;
            });
            return mappingConfig;
        }
    }
}