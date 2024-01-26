using System.ComponentModel.DataAnnotations.Schema;

namespace HCQS.BackEnd.DAL.Models
{
    public class SupplierPriceDetail
    {
        public Guid Id { get; set; }
        public int MOQ { get; set; }
        public double Price { get; set; }

        public Guid? MaterialId { get; set; }

        [ForeignKey(nameof(MaterialId))]
        public Material? Material { get; set; }

        public Guid? SupplierPriceQuotationId { get; set; }

        [ForeignKey(nameof(SupplierPriceQuotationId))]
        public SupplierPriceQuotation? SupplierPriceQuotation { get; set; }
    }
}