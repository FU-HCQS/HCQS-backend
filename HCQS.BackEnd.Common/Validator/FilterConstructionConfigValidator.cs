using FluentValidation;
using HCQS.BackEnd.Common.Dto.Request;

namespace HCQS.BackEnd.Common.Validator
{
    public class FilterConstructionConfigValidator : AbstractValidator<FilterConstructionConfigRequest>
    {
        public FilterConstructionConfigValidator()
        {
            RuleFor(x => x.NumOfFloorMin)
            .NotNull().GreaterThan(-1)
            .WithMessage("The minimum number of floors must be left empty or higher than 0!");

            RuleFor(x => x.NumOfFloorMax)
                .NotNull().GreaterThan(-1)
            .WithMessage("The maximum number of floors must be left empty or higher than 0!");


            RuleFor(x => new { x.NumOfFloorMin, x.NumOfFloorMax })
                .Must(x => (x.NumOfFloorMin == 0 && x.NumOfFloorMax == 0 || x.NumOfFloorMin > 0 && x.NumOfFloorMax > x.NumOfFloorMin))
                .WithMessage("NumOfFloorMax must be greater than NumOfFloorMin and both must be greater than 0!");

            RuleFor(x => x.AreaMin)
            .NotNull().GreaterThan(-1)
            .WithMessage("The minimum area is must be left empty or higher than 0!");

            RuleFor(x => x.AreaMax)
                .NotNull().GreaterThan(-1)
            .WithMessage("The maximum area is must be left empty or higher than 0!");

            RuleFor(x => new { x.AreaMin, x.AreaMax })
                .Must(x => (x.AreaMin == 0 && x.AreaMax == 0 || x.AreaMin > 0 && x.AreaMax > x.AreaMin))
                .WithMessage("AreaMax must be greater than AreaMin and both must be greater than 0!");

            RuleFor(x => x.TiledAreaMin)
                .NotNull().GreaterThan(-1)
                .WithMessage("The minimum tiled area must be left empty or higher than 0!");

            RuleFor(x => x.TiledAreaMax)
                .NotNull().GreaterThan(-1)// TiledAreaMax must be provided to apply this rule
                .WithMessage("The maximum tiled area must be left empty or higher than 0!");

            RuleFor(x => new { x.TiledAreaMin, x.TiledAreaMax })
                .Must(x => (x.TiledAreaMin == 0 && x.TiledAreaMax == 0 || x.TiledAreaMin > 0 && x.TiledAreaMax > x.TiledAreaMin))
                .WithMessage("TiledAreaMax must be greater than TiledAreaMin and both must be greater than 0!");
            RuleFor(x => x.ConstructionType).NotNull().WithMessage("The construction type is required!");
        }
    }
}