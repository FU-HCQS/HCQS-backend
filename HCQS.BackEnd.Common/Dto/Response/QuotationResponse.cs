using HCQS.BackEnd.DAL.Models;

namespace HCQS.BackEnd.Common.Dto.Response
{
    public class QuotationResponse
    {
        public Quotation Quotation { get; set; }
        public List<QuotationDetail> QuotationDetails { get; set; }
        public List<QuotationDealing> QuotationDealings { get; set; }
        public List<WorkerForProject> WorkerForProjects { get; set; }
    }
}