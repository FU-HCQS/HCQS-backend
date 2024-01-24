using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace HCQS.BackEnd.Common.Dto.Request
{
    public class ConfigProjectRequest
    {
        public Guid Id { get; set; }

        public int SandMixingRatio { get; set; }
        public int CementMixingRatio { get; set; }
        public int StoneMixingRatio { get; set; }
        public double TiledArea { get; set; }
        public double WallLength { get; set; }
        public double WallHeight { get; set; }
        public int EstimatedTimeOfCompletion { get; set; }
        public int NumberOfLabor { get; set; }


       
    }
}
