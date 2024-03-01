using static HCQS.BackEnd.Common.Dto.Request.ProjectDto;
using static HCQS.BackEnd.DAL.Models.Project;

namespace HCQS.BackEnd.Common.Dto.Request
{
    public class DeleteConstructionConfigRequest
    {
        public ProjectConstructionType ConstructionType { get; set; }
        public double NumOfFloor { get; set; }
        public double Area { get; set; }
        public double TiledArea { get; set; }
    }
}