using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HCQS.BackEnd.DAL.Models
{
    public class MaterialHistory
    {
        [Key]
        public Guid Id { get; set; }

        public double ImportPrice { get; set; }
        public DateTime Date { get; set; }

        public Guid MaterialSupplierId { get; set; }

        [ForeignKey(nameof(MaterialSupplierId))]
        public MaterialSupplier MaterialSupplier { get; set; }
    }
}