using FluentValidation;
using HCQS.BackEnd.Common.Dto.Request;

namespace HCQS.BackEnd.Common.Validator
{
    public class ConfigProjectValidator : AbstractValidator<ConfigProjectRequest>
    {
        public ConfigProjectValidator()
        {
            RuleFor(x => x.TiledArea).NotNull().NotEmpty().WithMessage("The TiledArea must be required!");
            RuleFor(x => x.WallLength).NotNull().NotEmpty().GreaterThan(0).WithMessage("The WallLength must be required!");
            RuleFor(x => x.WallHeight).NotNull().NotEmpty().GreaterThan(0).WithMessage("The WallLength must be required!");
            RuleFor(x => x.SandMixingRatio).NotNull().NotEmpty().GreaterThan(0).WithMessage("The SandMixingRatio must be required!");
            RuleFor(x => x.StoneMixingRatio).NotNull().NotEmpty().GreaterThan(0).WithMessage("The StoneMixingRatio must be required!");
            RuleFor(x => x.CementMixingRatio).NotNull().NotEmpty().GreaterThan(0).WithMessage("The CementMixingRatio must be required!");
            RuleFor(x => x.EstimatedTimeOfCompletion).NotNull().GreaterThan(0).NotEmpty().WithMessage("The EstimatedTimeOfCompletion must be required!");
            RuleFor(x => x.LaborRequests)
                      .Must(laborRequests => laborRequests.All(a => a.Quantity > 0))
                      .WithMessage("Quantity should be greater than 0 for all LaborRequests");
        }
    }
}