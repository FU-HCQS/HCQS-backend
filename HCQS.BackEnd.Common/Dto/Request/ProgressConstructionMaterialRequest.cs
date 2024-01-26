using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCQS.BackEnd.Common.Dto.Request
{
    public class ProgressConstructionMaterialRequest
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public DateTime Date {  get; set; }
        public Guid? QuotationDetailId { get; set; }
    }
}
