using FluentValidation;
using HCQS.BackEnd.Common.Dto.Request;

namespace HCQS.BackEnd.Common.Validator
{
    public class QuotationDealingValidator : AbstractValidator<QuotationDealingDto>
    {
        public QuotationDealingValidator()
        {
            RuleFor(x => x.QuotationId).NotNull().NotEmpty().WithMessage("The QuotationId must be required!");
        }
    }
}