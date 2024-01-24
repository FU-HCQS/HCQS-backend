using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HCQS.BackEnd.DAL.Models
{
    public class ProgressConstructionMaterial
    {
        [Key]
        public Guid Id { get; set; }

        public double Discount { get; set; }
        public double Total { get; set; }
        public int Quantity { get; set; }
        public Guid ProjectId { get; set; }

        [ForeignKey(nameof(ProjectId))]
        public Project Project { get; set; }

        public Guid? ExportPriceMaterialId { get; set; }

        [ForeignKey(nameof(ExportPriceMaterialId))]
        public ExportPriceMaterial ExportPriceMaterial { get; set; }


        public Guid? QuotationDetailId { get; set; }

        [ForeignKey(nameof(QuotationDetailId))]
        public QuotationDetail? QuotationDetail { get; set; }

        public ImportExportInventoryHistory? InventoryHistory { get; set; }
    }
}