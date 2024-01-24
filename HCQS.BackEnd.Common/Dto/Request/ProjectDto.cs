using Microsoft.AspNetCore.Http;

namespace HCQS.BackEnd.Common.Dto.Request
{
    public class ProjectDto
    {
        public Guid? Id { get; set; }

        public int NumOfFloor { get; set; }
        public double Area { get; set; }
        public IFormFile LandDrawingFile { get; set; }
        public string AccountId { get; set; }
    }
}