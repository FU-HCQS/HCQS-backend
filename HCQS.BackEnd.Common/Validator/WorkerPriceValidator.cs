using FluentValidation;
using HCQS.BackEnd.Common.Dto.Request;

namespace HCQS.BackEnd.Common.Validator
{
    public class WorkerPriceValidator : AbstractValidator<WorkerPriceRequest>
    {
        public WorkerPriceValidator()
        {
            RuleFor(x => x.LaborCost).NotNull().NotEmpty().WithMessage("The LaborCost must be required!");
            RuleFor(x => x.PositionName).NotEmpty().NotNull().WithMessage("The PositionName must be required!");
        }
    }
}