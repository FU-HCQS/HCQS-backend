using System.ComponentModel.DataAnnotations;
using static HCQS.BackEnd.DAL.Models.Project;

namespace HCQS.BackEnd.DAL.Models
{
    public class ConstructionConfigValue
    {
        [Key]
        public Guid Id { get; set; }

        public double SandMixingRatio { get; set; }
        public double CementMixingRatio { get; set; }
        public double StoneMixingRatio { get; set; }

        public ProjectConstructionType ConstructionType { get; set; }

        public int NumOfFloorMin { get; set; }
        public int? NumOfFloorMax { get; set; }
        public int AreaMin { get; set; }
        public int? AreaMax { get; set; }
        public int TiledAreaMin { get; set; }
        public int? TiledAreaMax { get; set; }
    }
}