using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
