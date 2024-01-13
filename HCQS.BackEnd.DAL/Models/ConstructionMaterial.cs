using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HCQS.BackEnd.DAL.Models
{
    public class ConstructionMaterial
    {
        [Key]
        public Guid Id { get; set; }

        public double Discount { get; set; }
        public double Total { get; set; }
        public Guid ProjectId { get; set; }

        [ForeignKey(nameof(ProjectId))]
        public Project Project { get; set; }

        public Guid? ExportPriceMaterialId { get; set; }

        [ForeignKey(nameof(ExportPriceMaterialId))]
        public ExportPriceMaterial ExportPriceMaterial { get; set; }

        public Guid? MaterialHistoryId { get; set; }

        [ForeignKey(nameof(MaterialHistoryId))]
        public MaterialHistory? MaterialHistory { get; set; }
    }
}