using FluentValidation;
using HCQS.BackEnd.Common.Dto.Request;

namespace HCQS.BackEnd.Common.Validator
{
    public class QuotationDetailValidator : AbstractValidator<QuotationDetailDto>
    {
        public QuotationDetailValidator()
        {
            RuleFor(x => x.Quantity).NotNull().NotEmpty().WithMessage("The Quantity must be required!");
            RuleFor(x => x.QuotationId).NotNull().NotEmpty().WithMessage("The QuotationId must be required!");
            RuleFor(x => x.MaterialId).NotNull().NotEmpty().WithMessage("The MaterialId must be required!");
        }
    }
}