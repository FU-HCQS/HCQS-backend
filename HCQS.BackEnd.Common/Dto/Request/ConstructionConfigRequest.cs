using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HCQS.BackEnd.Common.Dto.Request.ProjectDto;

namespace HCQS.BackEnd.Common.Dto.Request
{
    public class ConstructionConfigRequest
    {
        public Guid Id { get; set; }
        

        public ConfigType Type { get; set; }

        public enum ConfigType
        {
            SandMixingRatio,
            CementMixingRatio,
            StoneMixingRatio,
            EstimatedTimeOfCompletion,
            NumberOfLabor
        }

        public ConstructionType ConstructionType { get; set; }

      
        public string NumOfFloor {  get; set; }
        public string Area { get; set; }
        public float Value { get; set; }
       
    }
}
