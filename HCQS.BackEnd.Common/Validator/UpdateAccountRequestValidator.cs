using FluentValidation;
using HCQS.BackEnd.Common.Dto.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
