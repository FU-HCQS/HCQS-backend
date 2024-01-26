using FluentValidation;
using HCQS.BackEnd.Common.Dto.Request;

namespace HCQS.BackEnd.Common.Validator
{
    public class ExportPriceMaterialValidator : AbstractValidator<ExportPriceMaterialRequest>
    {
        public ExportPriceMaterialValidator()
        {
            RuleFor(x => x.Price).NotEmpty().NotNull().GreaterThan(0).WithMessage("The Price is required!");
            RuleFor(x => x.MaterialId).NotEmpty().NotNull().WithMessage("The MaterialId is required!");
        }
    }
}