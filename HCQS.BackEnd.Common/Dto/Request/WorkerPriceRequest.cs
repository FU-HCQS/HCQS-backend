using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCQS.BackEnd.Common.Dto.Request
{
    public class WorkerPriceRequest
    {
        public Guid? Id { get; set; }

        public string? PositionName { get; set; }
        public double? LaborCost { get; set; }
    }
}
