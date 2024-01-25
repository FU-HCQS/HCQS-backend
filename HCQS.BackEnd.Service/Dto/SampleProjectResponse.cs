using HCQS.BackEnd.Common.Dto.Response;
using HCQS.BackEnd.DAL.Models;

namespace HCQS.BackEnd.Service.Dto
{
    public class SampleProjectResponse
    {
        public SampleProject SampleProject { get; set; }
        public List<StaticFileResponse> StaticFiles { get; set; }
    }
}