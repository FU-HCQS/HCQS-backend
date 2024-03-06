using FluentValidation;
using HCQS.BackEnd.Common.Dto.Request;

namespace HCQS.BackEnd.Common.Validator
{
    public class ContractProgressPaymentDtoValidator : AbstractValidator<ContractProgressPaymentDto>
    {
        public ContractProgressPaymentDtoValidator()
        {
            RuleFor(x => x.Price).NotNull().NotEmpty().GreaterThan(0).WithMessage("The Price must be required!");
            RuleFor(x => x.ContractId).NotNull().NotEmpty().WithMessage("The ContractId must be required!");
            RuleFor(x => x.Content).NotNull().NotEmpty().WithMessage("The Content must be required!");
            RuleFor(x => x.Name).NotNull().NotEmpty().WithMessage("The Name must be required!");
            RuleFor(x => x.EndDate).NotNull().NotEmpty().WithMessage("The EndDate must be required!");

        }
    }
}