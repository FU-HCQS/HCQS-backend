using FluentValidation;
using HCQS.BackEnd.Common.Dto.Request;

namespace HCQS.BackEnd.Common.Validator
{
    public class SampleProjectValidator : AbstractValidator<SampleProjectRequest>
    {
        public SampleProjectValidator()
        {
            RuleFor(x => x.Header).NotNull().NotEmpty().WithMessage("The Header is required!");
            RuleFor(x => x.EstimatePrice).GreaterThan(0).NotNull().NotEmpty().WithMessage("The estimate price is required and greater than 0!");
            RuleFor(x => x.TotalArea).NotNull().NotEmpty().WithMessage("The TotalArea is required!");
            RuleFor(x => x.NumOfFloor).NotEmpty().NotEmpty().WithMessage("The NumOfFloor is required!");
            RuleFor(x => x.Location).NotEmpty().NotEmpty().WithMessage("The Location is required!");
            RuleFor(x => x.ProjectType).IsInEnum().NotNull().WithMessage("The ProjectType is required!");
        }
    }
}