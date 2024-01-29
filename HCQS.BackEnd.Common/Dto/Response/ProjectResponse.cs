using HCQS.BackEnd.DAL.Models;

namespace HCQS.BackEnd.Common.Dto.Response
{
    public class ProjectResponse
    {
        public Project Project { get; set; }
        public List<Quotation> Quotations { get; set; }
        public List<QuotationDealing> QuotationDealings { get; set; }
    }
}