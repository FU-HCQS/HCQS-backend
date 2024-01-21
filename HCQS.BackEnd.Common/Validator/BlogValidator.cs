using FluentValidation;
using HCQS.BackEnd.Common.Dto.Request;

namespace HCQS.BackEnd.Common.Validator
{
    public class BlogValidator : AbstractValidator<BlogRequest>
    {
        public BlogValidator()
        {
            RuleFor(x => x.AccountId).NotNull().NotEmpty().WithMessage("The accountId is required!");
            RuleFor(x => x.Content).NotEmpty().NotNull().WithMessage("The content is required!");
            RuleFor(x => x.Date).NotEmpty().NotNull().WithMessage("The date is required!");
            RuleFor(x => x.ImageUrl).NotEmpty().NotNull().WithMessage("The file is required!");
        }
    }
}