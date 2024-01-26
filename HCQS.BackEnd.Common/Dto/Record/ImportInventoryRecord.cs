using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCQS.BackEnd.Common.Dto.Record
{
    public class ImportInventoryRecord
    {
        public Guid Id { get; set; }
        public string MaterialName { get; set; }
        public string SupplierName { get; set; }
        public int Quantity { get; set; }
    }
}
