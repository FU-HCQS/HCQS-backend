using HCQS.BackEnd.DAL.Models;

namespace HCQS.BackEnd.Common.Dto.Response
{
    public class SupplierPriceQuotationResponse
    {
        public SupplierPriceQuotation SupplierPriceQuotation { get; set; }
        public List<SupplierPriceDetail> SupplierPriceDetails { get; set; }
        public DateTime? Date { get; set; }
    }
}