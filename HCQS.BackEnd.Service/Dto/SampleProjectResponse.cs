using HCQS.BackEnd.DAL.Models;

namespace HCQS.BackEnd.Service.Dto
{
    public class SampleProjectResponse
    {
        public SampleProject SampleProject { get; set; }
        public IOrderedQueryable<StaticFile> StaticFiles { get; set; }
    }
}