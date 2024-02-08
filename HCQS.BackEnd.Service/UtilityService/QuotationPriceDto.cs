using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCQS.BackEnd.Service.UtilityService
{
    public class QuotationPriceDto
    {
        public double RawPrice { get; set; }
        public double FurniturePrice { get; set; }
        public double LaborPrice { get; set; }
    }
}
