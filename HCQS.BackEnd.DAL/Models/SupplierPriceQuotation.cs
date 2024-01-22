using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HCQS.BackEnd.DAL.Models
{
    public class SupplierPriceQuotation
    {
        [Key]
        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public Guid SupplierId { get; set; }
        [ForeignKey(nameof(SupplierId))]
        public Supplier? Supplier { get; set; }
    }
}