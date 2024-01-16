using System.ComponentModel.DataAnnotations;

namespace HCQS.BackEnd.DAL.Models
{
    public class Supplier
    {
        [Key]
        public Guid Id { get; set; }

        public string SupplierName { get; set; }
        public SupplierType Type { get; set; }
        public enum SupplierType
        {
            ConstructionMaterialsSupplier,
            FurnitureSupplier,
            Both
        }
    }
}