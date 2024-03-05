namespace HCQS.BackEnd.Common.Dto.Record
{
    public class SupplierMaterialQuotationRecord
    {
        public Guid Id { get; set; }
        public int No { get; set; }
        public string MaterialName { get; set; }
        public string Unit { get; set; }
        public int MOQ { get; set; }
        public double Price { get; set; }
    }
}