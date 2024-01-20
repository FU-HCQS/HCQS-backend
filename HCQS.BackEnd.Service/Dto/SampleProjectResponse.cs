using HCQS.BackEnd.DAL.Models;

namespace HCQS.BackEnd.Service.Dto
{
    public class SampleProjectResponse
    {
        public SampleProject SampleProject { get; set; }
        public List<StaticFile> StaticFiles { get; set; }
    }
}