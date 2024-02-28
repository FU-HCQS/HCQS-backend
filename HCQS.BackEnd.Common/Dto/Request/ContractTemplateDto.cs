using HCQS.BackEnd.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCQS.BackEnd.Common.Dto.Request
{
    public class ContractTemplateDto
    {
        public List<ContractProgressPayment> ContractProgressPayments { get; set; }
        public Contract Contract { get; set; }
        public bool IsSigned { get; set; }
        public Account Account { get; set; }
        public Project Project { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime SignDate { get; set; }
    }
}
