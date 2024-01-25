using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HCQS.BackEnd.DAL.Models
{
    public class ImportExportInventoryHistory
    {
        [Key]
        public Guid Id { get; set; }

        public int Quantity { get; set; }
        public Guid? SupplierPriceDetailId { get; set; }

        [ForeignKey(nameof(SupplierPriceDetailId))]
        public SupplierPriceDetail? SupplierPriceDetail { get; set; }
      
    }
}