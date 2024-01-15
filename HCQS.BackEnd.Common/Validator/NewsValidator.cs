using FluentValidation;
using HCQS.BackEnd.Common.Dto.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCQS.BackEnd.Common.Validator
{
    public class NewsValidator:AbstractValidator<NewsRequest>
    {
        public NewsValidator() {
            RuleFor(x => x.AccountId).NotNull().NotEmpty().WithMessage("the accountid is required!");
            RuleFor(x => x.Date).NotNull().NotEmpty().WithMessage("the createDate is required!");
            RuleFor(x => x.Header).NotNull().NotEmpty().WithMessage("the header is required!");
            RuleFor(x => x.Content).NotNull().NotEmpty().WithMessage("the content is required!");
            RuleFor(x => x.ImgUrl).NotEmpty().NotEmpty().WithMessage("the file is required!");
        }

    }
}
