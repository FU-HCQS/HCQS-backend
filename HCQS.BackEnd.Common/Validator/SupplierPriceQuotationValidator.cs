using FluentValidation;
using HCQS.BackEnd.Common.Dto.Request;

namespace HCQS.BackEnd.Common.Validator
{
    public class SupplierPriceQuotationValidator : AbstractValidator<SupplierPriceQuotationRequest>
    {
        public SupplierPriceQuotationValidator()
        {
            RuleFor(x => x.Id).NotNull().NotEmpty().WithMessage("The Id is required!");
            RuleFor(x => x.Date).NotEmpty().NotNull().WithMessage("The Date is required!");
            RuleFor(x => x.SupplierId).NotEmpty().NotNull().WithMessage("The SupplierId is required!");
        }
    }
}