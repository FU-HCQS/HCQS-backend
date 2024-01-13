using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HCQS.BackEnd.DAL.Models
{
    public class MaterialSupplier
    {
        [Key]
        public Guid Id { get; set; }

        public Guid MaterialId { get; set; }

        [ForeignKey(nameof(MaterialId))]
        public Material Material { get; set; }

        public Guid SupplierId { get; set; }

        [ForeignKey(nameof(SupplierId))]
        public Supplier Supplier { get; set; }
    }
}