using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HCQS.BackEnd.DAL.Models
{
    public class Quotation
    {
        [Key]
        public Guid Id { get; set; }

        public double RawMaterialPrice { get; set; }
        public double RawMaterialDiscount { get; set; }
        public double FurniturePrice { get; set; }
        public double FurnitureDiscount { get; set; }
        public double LaborPrice { get; set; }
        public double LaborDiscount { get; set; }
        public double Total { get; set; }
        public Status QuotationStatus { get; set; }

        public enum Status
        {
            Pending,
            WaitingForCustomerResponse,
            Cancel,
            Approved,
        }

        public Guid ProjectId { get; set; }

        [ForeignKey(nameof(ProjectId))]
        public Project Project { get; set; }
    }
}