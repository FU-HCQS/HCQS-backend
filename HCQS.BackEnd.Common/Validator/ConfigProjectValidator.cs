using FluentValidation;
using HCQS.BackEnd.Common.Dto.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCQS.BackEnd.Common.Validator
{
    public class ConfigProjectValidator : AbstractValidator<ConfigProjectRequest>
    {
        public ConfigProjectValidator()
        {
            RuleFor(x => x.TiledArea).NotNull().NotEmpty().WithMessage("The TiledArea must be required!");
            RuleFor(x => x.NumberOfLabor).NotNull().NotEmpty().WithMessage("The NumberOfLabor must be required!");
            RuleFor(x => x.WallLength).NotNull().NotEmpty().WithMessage("The WallLength must be required!");
            RuleFor(x => x.WallHeight).NotNull().NotEmpty().WithMessage("The WallLength must be required!");
            RuleFor(x => x.SandMixingRatio).NotNull().NotEmpty().WithMessage("The SandMixingRatio must be required!");
            RuleFor(x => x.StoneMixingRatio).NotNull().NotEmpty().WithMessage("The StoneMixingRatio must be required!");
            RuleFor(x => x.CementMixingRatio).NotNull().NotEmpty().WithMessage("The CementMixingRatio must be required!");
            RuleFor(x => x.EstimatedTimeOfCompletion).NotNull().NotEmpty().WithMessage("The EstimatedTimeOfCompletion must be required!");

        }

    }
}
