using Microsoft.AspNetCore.Http;

namespace HCQS.BackEnd.Common.Dto.Request
{
    public class SampleProjectRequest
    {
        public Guid? Id { get; set; }

        public int? NumOfFloor { get; set; }
        public double? ConstructionArea { get; set; }
        public double? TotalArea { get; set; }
        public Type ProjectType { get; set; }

        public enum Type
        {
            Level_4_House,
            House_With_Multiple_Floors
        }

        public string? Function { get; set; }
        public string? Header { get; set; }
        public string? Content { get; set; }
        public double? EstimatePrice { get; set; }
        public string? Location { get; set; }
        public string? AccountId { get; set; }

        /// <summary>
        /// Image files to upload (multiple files allowed).
        /// </summary>

        public List<IFormFile> ImageFiles { get; set; }
    }
}