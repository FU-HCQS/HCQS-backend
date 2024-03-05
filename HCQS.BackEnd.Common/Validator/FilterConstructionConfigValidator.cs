using FluentValidation;
using HCQS.BackEnd.Common.Dto.Request;

namespace HCQS.BackEnd.Common.Validator
{
    public class FilterConstructionConfigValidator : AbstractValidator<FilterConstructionConfigRequest>
    {
        public FilterConstructionConfigValidator()
        {
            RuleFor(x => x.NumOfFloorMin)
            .NotNull().NotEmpty().GreaterThan(0)
            .WithMessage("The minimum number of floors is required!");

            RuleFor(x => x.NumOfFloorMax)
                .NotNull().NotEmpty().GreaterThan(0)
            .WithMessage("The maximum number of floors is required!");


            RuleFor(x => new { x.NumOfFloorMin, x.NumOfFloorMax })
                .Must(x => (x.NumOfFloorMax > x.NumOfFloorMin))
                .WithMessage("NumOfFloorMax must be greater than NumOfFloorMin");

            RuleFor(x => x.AreaMin)
            .NotNull().NotEmpty().GreaterThan(0)
            .WithMessage("The minimum area  is required!");

            RuleFor(x => x.AreaMax)
                .NotNull().NotEmpty().GreaterThan(0)
            .WithMessage("The maximum area is required ");

            RuleFor(x => new { x.AreaMin, x.AreaMax })
                .Must(x => (x.AreaMax > x.AreaMin))
                .WithMessage("AreaMax must be greater than AreaMin!");

            RuleFor(x => x.TiledAreaMin)
                .NotNull().NotEmpty().GreaterThan(0)
                .WithMessage("The minimum tiled area  is required!");

            RuleFor(x => x.TiledAreaMax)
                .NotNull().NotEmpty().GreaterThan(0)// TiledAreaMax must be provided to apply this rule
                .WithMessage("The maximum tiled area is required!");

            RuleFor(x => new { x.TiledAreaMin, x.TiledAreaMax })
                .Must(x => (x.TiledAreaMax > x.TiledAreaMin))
                .WithMessage("TiledAreaMax must be greater than TiledAreaMin!");
            RuleFor(x => x.ConstructionType).NotNull().WithMessage("The construction type is required!");
        }
    }
}