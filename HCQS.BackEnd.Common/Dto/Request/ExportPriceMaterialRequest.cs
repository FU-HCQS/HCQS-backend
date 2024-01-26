namespace HCQS.BackEnd.Common.Dto.Request
{
    public class ExportPriceMaterialRequest
    {
        public Guid Id { get; set; }
        public double Price { get; set; }
        public DateTime? Date { get; set; }

        public Guid MaterialId { get; set; }
    }
}