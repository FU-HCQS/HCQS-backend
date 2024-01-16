using FluentValidation;
using HCQS.BackEnd.Common.Dto.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCQS.BackEnd.Common.Validator
{
    public class SupplierValidator : AbstractValidator<SupplierRequest>
    {
        public SupplierValidator()
        {
            RuleFor(x => x.SupplierName).NotNull().NotEmpty().WithMessage("the supplierName is required!");
        }

    }
}
