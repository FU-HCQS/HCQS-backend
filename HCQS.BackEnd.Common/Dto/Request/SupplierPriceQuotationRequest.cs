using HCQS.BackEnd.Common.Dto.Record;

namespace HCQS.BackEnd.Common.Dto.Request
{
    public class SupplierPriceQuotationRequest
    {
        public Guid Id { get; set; }
        public Guid SupplierId { get; set; }

        public List<SupplierMaterialQuotationRecord>? MaterialQuotationRecords { get; set; }
    }
}