using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCQS.BackEnd.Common.Dto.Request
{
    public class ConstructionConfigRequest
    {
        public Guid Id { get; set; }
        

        public ConfigType configType { get; set; }

        public enum ConfigType
        {
            SandMixingRatio,
            CementMixingRatio,
            StoneMixingRatio,
            EstimatedTimeOfCompletion,
            NumberOfLabor
        }

        public ConstructionType constructionType { get; set; }

        public enum ConstructionType
        {
            RoughConstruction,
            CompleteConstruction
        }
        public string NumOfFloor {  get; set; }
        public string Area { get; set; }
        public float Value { get; set; }
       
    }
}
