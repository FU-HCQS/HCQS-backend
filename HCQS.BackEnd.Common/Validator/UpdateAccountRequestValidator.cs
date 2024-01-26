using FluentValidation;
using HCQS.BackEnd.Common.Dto.Request;

namespace HCQS.BackEnd.Common.Validator
{
    public class UpdateAccountRequestValidator : AbstractValidator<UpdateAccountRequestDto>
    {
        public UpdateAccountRequestValidator()
        {
            RuleFor(x => x.Email).NotNull().NotEmpty().WithMessage("The email is required!");
        }
    }
}