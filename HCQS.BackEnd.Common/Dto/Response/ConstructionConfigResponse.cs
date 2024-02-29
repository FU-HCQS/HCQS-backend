using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCQS.BackEnd.Common.Dto.Response
{
    public class ConstructionConfigResponse
    {
        public int SandMixingRatio { get; set; }
        public int CementMixingRatio { get; set; }
        public int StoneMixingRatio { get; set; }
        public int EstimatedTimeOfCompletion { get; set; }
        public int NumberOfLabor { get; set; }
    }
}
