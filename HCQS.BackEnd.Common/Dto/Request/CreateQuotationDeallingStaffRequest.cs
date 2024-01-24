namespace HCQS.BackEnd.Common.Dto.Request
{
    public class CreateQuotationDeallingStaffRequest
    {
        public Guid QuotationId { get; set; }

        public List<ExportPriceMaterialDto> ExportPriceMaterialDtos { get; set; }
        public double MaterialDiscount { get; set; }
        public double FurnitureDiscount { get; set; }
        public double LaborDiscount { get; set; }

        public class ExportPriceMaterialDto
        {
            public Guid? Id { get; set; }
            public double Price { get; set; }
            public Guid? MaterialId { get; set; }
        }
    }
}