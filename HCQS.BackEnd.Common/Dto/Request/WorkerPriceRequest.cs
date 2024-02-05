namespace HCQS.BackEnd.Common.Dto.Request
{
    public class WorkerPriceRequest
    {
        public Guid? Id { get; set; }

        public string? PositionName { get; set; }
        public double? LaborCost { get; set; }
    }
}