using Microsoft.AspNetCore.Http;

namespace HCQS.BackEnd.Common.Dto.Request
{
    public class NewsRequest
    {
        public Guid? Id { get; set; }
        public string? Header { get; set; }
        public string? Content { get; set; }
        public IFormFile? ImgUrl { get; set; }
        public DateTime? Date { get; set; }
        public string? AccountId { get; set; }
    }
}