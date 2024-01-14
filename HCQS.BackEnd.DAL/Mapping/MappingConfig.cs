using AutoMapper;


namespace HCQS.BackEnd.DAL.Mapping
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMap()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
             
            });
            return mappingConfig;
        }
    }
}