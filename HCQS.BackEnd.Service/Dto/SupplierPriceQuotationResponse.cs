using HCQS.BackEnd.DAL.Models;

namespace HCQS.BackEnd.Service.Dto
{
    public class SupplierPriceQuotationResponse
    {
        public SupplierPriceQuotation SupplierPriceQuotation { get; set; }
        public List<SupplierPriceDetail> SupplierPriceDetails { get; set; }
        public DateTime? Date { get; set; }
    }
}