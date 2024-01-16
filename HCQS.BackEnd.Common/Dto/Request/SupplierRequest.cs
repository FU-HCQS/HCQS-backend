using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCQS.BackEnd.Common.Dto.Request
{
    public class SupplierRequest
    {
        public Guid? Id { get; set; }
        public string? SupplierName { get; set; }
        public SupplierType Type { get; set; }
        public enum SupplierType
        {
            ConstructionMaterialsSupplier,
            FurnitureSupplier,
            Both
        }
    }
}
