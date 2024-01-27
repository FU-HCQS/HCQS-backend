namespace HCQS.BackEnd.Common.Dto.Request
{
    public class QuotationDetailDto
    {
        public Guid? Id { get; set; }

        public int Quantity { get; set; }
        public Guid? QuotationId { get; set; }
        public Guid? MaterialId { get; set; }
    }
}