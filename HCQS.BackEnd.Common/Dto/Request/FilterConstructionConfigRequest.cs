using static HCQS.BackEnd.DAL.Models.Project;

namespace HCQS.BackEnd.Common.Dto.Request
{
    public class FilterConstructionConfigRequest
    {
        public ProjectConstructionType ConstructionType { get; set; }
        public int? NumOfFloorMin { get; set; }
        public int? NumOfFloorMax { get; set; }
        public int? AreaMin { get; set; }
        public int? AreaMax { get; set; }
        public int? TiledAreaMin { get; set; }
        public int? TiledAreaMax { get; set; }
    }
}