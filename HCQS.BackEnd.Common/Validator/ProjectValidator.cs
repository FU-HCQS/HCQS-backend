using FluentValidation;
using HCQS.BackEnd.Common.Dto.Request;

namespace HCQS.BackEnd.Common.Validator
{
    public class ProjectValidator : AbstractValidator<ProjectDto>
    {
        public ProjectValidator()
        {
            RuleFor(x => x.AccountId).NotNull().NotEmpty().WithMessage("The accountId must be required!");
            RuleFor(x => x.LandDrawingFile).NotNull().NotEmpty().WithMessage("The LandDrawingFile must be required!");
            RuleFor(x => x.NumOfFloor).NotNull().NotEmpty().WithMessage("The NumOfFloor must be required!");
            RuleFor(x => x.Area).NotNull().NotEmpty().WithMessage("The Area must be required!");
        }
    }
}