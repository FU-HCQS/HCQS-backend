namespace HCQS.BackEnd.Common.Dto.Request
{
    public class ConfigProjectRequest
    {
        public Guid? Id { get; set; }
        public double TiledArea { get; set; }
        public double WallLength { get; set; }
        public double WallHeight { get; set; }
        public int EstimatedTimeOfCompletion { get; set; }
        public List<LaborRequest> LaborRequests { get; set; }

        public class LaborRequest
        {
            public double ExportLaborCost { get; set; }
            public int Quantity { get; set; }
            public Guid WorkerPriceId { get; set; }
        }
    }
}