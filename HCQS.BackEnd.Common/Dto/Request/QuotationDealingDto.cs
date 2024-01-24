namespace HCQS.BackEnd.Common.Dto.Request
{
    public class QuotationDealingDto
    {
        public Guid? Id { get; set; }
        public double MaterialDiscount { get; set; }
        public double FurnitureDiscount { get; set; }
        public double LaborDiscount { get; set; }
        public Guid? QuotationId { get; set; }
    }
}