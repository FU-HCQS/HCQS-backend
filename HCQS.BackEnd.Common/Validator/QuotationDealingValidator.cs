using FluentValidation;
using HCQS.BackEnd.Common.Dto.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCQS.BackEnd.Common.Validator
{
    public class QuotationDealingValidator : AbstractValidator<QuotationDealingDto>
    {
        public QuotationDealingValidator()
        {
            RuleFor(x => x.MaterialDiscount).NotNull().NotEmpty().WithMessage("The MaterialDiscount must be required!");
            RuleFor(x => x.LaborDiscount).NotNull().NotEmpty().WithMessage("The LaborDiscount must be required!");
            RuleFor(x => x.FurnitureDiscount).NotNull().NotEmpty().WithMessage("The FurnitureDiscount must be required!");
            RuleFor(x => x.QuotationId).NotNull().NotEmpty().WithMessage("The QuotationId must be required!");

        }
    }
}
