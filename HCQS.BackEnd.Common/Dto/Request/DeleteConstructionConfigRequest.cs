using static HCQS.BackEnd.Common.Dto.Request.ProjectDto;

namespace HCQS.BackEnd.Common.Dto.Request
{
    public class DeleteConstructionConfigRequest
    {
        public ConstructionType ConstructionType { get; set; }
        public string NumOfFloor { get; set; }
        public string Area { get; set; }
        public string TiledArea { get; set; }
    }
}