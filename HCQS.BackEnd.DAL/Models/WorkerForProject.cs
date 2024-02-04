using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HCQS.BackEnd.DAL.Models
{
    public class WorkerForProject
    {
        [Key]
        public Guid Id { get; set; }

        public double ExportLaborCost { get; set; }
        public int Quantity { get; set; }
        public Guid? WorkerPriceId { get; set; }

        [ForeignKey(nameof(WorkerPriceId))]
        public WorkerPrice? WorkerPrice { get; set; }

        public Guid? QuotationId { get; set; }

        [ForeignKey(nameof(QuotationId))]
        public Quotation? Quotation { get; set; }
    }
}