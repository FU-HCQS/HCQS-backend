namespace HCQS.BackEnd.Common.Dto.Request
{
    public class CreateQuotationDeallingStaffRequest
    {
        public Guid QuotationId { get; set; }

        public double RawMaterialDiscount { get; set; }
        public double FurnitureDiscount { get; set; }
        public double LaborDiscount { get; set; }
    }
}