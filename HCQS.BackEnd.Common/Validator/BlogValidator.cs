using FluentValidation;
using HCQS.BackEnd.Common.Dto.Request;

namespace HCQS.BackEnd.Common.Validator
{
    public class BlogValidator : AbstractValidator<BlogRequest>
    {
        public BlogValidator()
        {
            RuleFor(x => x.AccountId).NotNull().NotEmpty().WithMessage("The accountId must be required!");
            RuleFor(x => x.Content).NotEmpty().NotNull().WithMessage("The content must be required!");
            RuleFor(x => x.Date).NotEmpty().NotNull().WithMessage("The date must be required!");
            RuleFor(x => x.ImageUrl).NotEmpty().NotNull().WithMessage("The file must be required!");
        }
    }
}