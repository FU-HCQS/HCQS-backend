using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCQS.BackEnd.Common.Dto.Request
{
    public class QuotationDealingDto
    {
        public Guid? Id { get; set; }
        public double MaterialDiscount { get; set; }
        public double FurnitureDiscount { get; set; }
        public double LaborDiscount { get; set; }
        public Guid? QuotationId { get; set; }
      
    }
}
