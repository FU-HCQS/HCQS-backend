namespace HCQS.BackEnd.Common.Dto.Request
{
    public class ContractProgressPaymentDto
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public double? Price { get; set; }
        public string? Content { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid? ContractId { get; set; }
    }
}