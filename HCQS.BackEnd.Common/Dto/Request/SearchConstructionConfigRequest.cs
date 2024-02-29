using static HCQS.BackEnd.Common.Dto.Request.ProjectDto;

namespace HCQS.BackEnd.Common.Dto.Request
{
    public class SearchConstructionConfigRequest
    {
        public ConstructionType ConstructionType { get; set; }
        public int NumOfFloor { get; set; }
        public double Area { get; set; }
        public double TiledArea { get; set; }
    }
}