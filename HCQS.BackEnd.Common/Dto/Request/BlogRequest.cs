using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace HCQS.BackEnd.Common.Dto.Request
{
    public class BlogRequest
    {
        public Guid? Id { get; set; }
        public string? Header { get; set; }
        public string? Content { get; set; }
        public IFormFile ImageUrl { get; set; }
        public DateTime? Date { get; set; }
        public string? AccountId { get; set; }
    }
}
