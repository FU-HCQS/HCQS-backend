namespace HCQS.BackEnd.Common.Dto.Record
{
    public class ExportPriceMaterialRecord
    {
        public Guid Id { get; set; }
        public string MaterialName { get; set; }
        public double Price { get; set; }
        public DateTime Date { get; set; }
    }
}