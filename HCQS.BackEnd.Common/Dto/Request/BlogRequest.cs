using Microsoft.AspNetCore.Http;

namespace HCQS.BackEnd.Common.Dto.Request
{
    public class BlogRequest
    {
        public Guid? Id { get; set; }
        public string? Header { get; set; }
        public string? Content { get; set; }
        public IFormFile ImageUrl { get; set; }
        public string? AccountId { get; set; }
    }
}