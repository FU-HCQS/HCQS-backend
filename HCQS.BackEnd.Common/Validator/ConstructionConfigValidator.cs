using FluentValidation;
using HCQS.BackEnd.Common.Dto.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCQS.BackEnd.Common.Validator
{
    public class ConstructionConfigValidator : AbstractValidator<ConstructionConfigRequest>
    {
        public ConstructionConfigValidator()
        {
            RuleFor(x => x.NumOfFloor).NotNull().NotEmpty().WithMessage("The NumOfFloor is required!");
            RuleFor(x => x.Area).NotNull().NotEmpty().WithMessage("The Area is required!");
            RuleFor(x => x.configType).IsInEnum().NotNull().WithMessage("The config type is required!");
            RuleFor(x => x.constructionType).IsInEnum().NotNull().WithMessage("The construction type isrequired!");
            RuleFor(x => x.Value).NotEmpty().NotNull().GreaterThan(0).WithMessage("The value is required!");
        }
    }
}
