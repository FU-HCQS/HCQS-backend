namespace HCQS.BackEnd.Common.Dto.Request
{
    public class ProgressConstructionMaterialRequest
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public DateTime Date { get; set; }
        public Guid? QuotationDetailId { get; set; }
    }
}