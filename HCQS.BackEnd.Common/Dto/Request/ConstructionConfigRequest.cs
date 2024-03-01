using static HCQS.BackEnd.DAL.Models.Project;

namespace HCQS.BackEnd.Common.Dto.Request
{
    public class ConstructionConfigRequest
    {
        public Guid Id { get; set; }

        public double SandMixingRatio { get; set; }
        public double CementMixingRatio { get; set; }
        public double StoneMixingRatio { get; set; }

        public ProjectConstructionType ConstructionType { get; set; }

        public double NumOfFloor { get; set; }
        public double Area { get; set; }
        public double TiledArea { get; set; }
    }
}