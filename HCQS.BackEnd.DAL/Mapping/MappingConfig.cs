using AutoMapper;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.DAL.Models;


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
                   .ForMember(desc => desc.Date, act => act.MapFrom(src => src.Date))
                   .ForMember(desc => desc.AccountId, act => act.MapFrom(src => src.AccountId))
                   ;

                config.CreateMap<NewsRequest, News>()
                   .ForMember(desc => desc.Id, act => act.MapFrom(src => src.Id))
                   .ForMember(desc => desc.Header, act => act.MapFrom(src => src.Header))
                   .ForMember(desc => desc.Content, act => act.MapFrom(src => src.Content))
                   .ForMember(desc => desc.Date, act => act.MapFrom(src => src.Date))
                   .ForMember(desc => desc.AccountId, act => act.MapFrom(src => src.AccountId))
                   ;



            });
            return mappingConfig;
        }
    }
}