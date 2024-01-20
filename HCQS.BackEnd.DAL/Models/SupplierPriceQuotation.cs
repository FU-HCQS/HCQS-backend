using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HCQS.BackEnd.DAL.Models
{
    public class SupplierPriceQuotation
    {
        [Key]
        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public Guid SupllierId { get; set; }
        [ForeignKey(nameof(SupllierId))]
        public Supplier? Supplier { get; set; }
    }
}