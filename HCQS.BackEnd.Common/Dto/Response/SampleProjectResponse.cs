using HCQS.BackEnd.DAL.Models;

namespace HCQS.BackEnd.Common.Dto.Response
{
    public class SampleProjectResponse
    {
        public SampleProject SampleProject { get; set; }
        public List<StaticFileResponse> StaticFiles { get; set; }
    }
}