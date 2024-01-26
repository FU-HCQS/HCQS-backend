using FluentValidation;
using HCQS.BackEnd.Common.Dto.Request;

namespace HCQS.BackEnd.Common.Validator
{
    public class SupplierValidator : AbstractValidator<SupplierRequest>
    {
        public SupplierValidator()
        {
            RuleFor(x => x.SupplierName).NotNull().NotEmpty().WithMessage("the supplierName is required!");
            RuleFor(x => x.Type).NotNull().WithMessage("the supplierName is required!");
        }
    }
}