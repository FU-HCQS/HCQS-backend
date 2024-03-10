using Microsoft.AspNetCore.Http;
using static HCQS.BackEnd.DAL.Models.Project;

namespace HCQS.BackEnd.Common.Dto.Request
{
    public class ProjectDto
    {
        public Guid? Id { get; set; }

        public int NumOfFloor { get; set; }
        public double Area { get; set; }
        public IFormFile LandDrawingFile { get; set; }
        public ProjectConstructionType Type { get; set; }

        public string? AddressProject { get; set; }
        public string? AccountId { get; set; }
    }
}