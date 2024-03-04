using static HCQS.BackEnd.DAL.Models.Project;

namespace HCQS.BackEnd.Common.Dto.Request
{
    public class SearchConstructionConfigRequest
    {
        public ProjectConstructionType ConstructionType { get; set; }
        public int NumOfFloor { get; set; }
        public double Area { get; set; }
        public double TiledArea { get; set; }
    }
}