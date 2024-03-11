using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCQS.BackEnd.Common.Dto.Record
{
    public class SupplierMaterialQuotationRecordSample
    {
        public Guid Id { get; set; }
        public string MaterialName { get; set; }
        public string Unit { get; set; }
        public int MOQ { get; set; }
        public double Price { get; set; }
    }
}
