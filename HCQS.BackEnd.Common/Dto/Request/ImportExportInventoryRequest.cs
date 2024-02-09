namespace HCQS.BackEnd.Common.Dto.Request
{
    public class ImportExportInventoryRequest
    {
        public Guid? Id { get; set; }
        public int Quantity { get; set; }
        public Guid? SupplierPriceDetailId { get; set; }
        public Guid? ProgressConstructionMaterialId { get; set; }
    }
}