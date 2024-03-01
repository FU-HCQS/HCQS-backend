using FluentValidation;
using HCQS.BackEnd.Common.Dto.Request;

namespace HCQS.BackEnd.Common.Validator
{
    public class ConstructionConfigValidator : AbstractValidator<ConstructionConfigRequest>
    {
        public ConstructionConfigValidator()
        {
            RuleFor(x => x.NumOfFloor).NotNull().NotEmpty().GreaterThan(0).WithMessage("The number of floor is required!");
            RuleFor(x => x.Area).NotNull().NotEmpty().GreaterThan(0).WithMessage("The area is required!");
            RuleFor(x => x.TiledArea).NotNull().NotEmpty().GreaterThan(0).WithMessage("The tiled area is required!");
            RuleFor(x => x.ConstructionType).NotNull().WithMessage("The construction type is required!");
            RuleFor(x => x.SandMixingRatio).NotNull().NotEmpty().GreaterThan(0).WithMessage("The sand mixing ratio must be greater than 0!");
            RuleFor(x => x.StoneMixingRatio).NotNull().NotEmpty().GreaterThan(0).WithMessage("The stone mixing ratio must be greater than 0!");
            RuleFor(x => x.CementMixingRatio).NotNull().NotEmpty().GreaterThan(0).WithMessage("The cement mixing ratio must be greater than 0!");
        }
    }
}