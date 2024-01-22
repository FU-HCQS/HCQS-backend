using FluentValidation;
using HCQS.BackEnd.Common.Dto.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCQS.BackEnd.Common.Validator
{
    public class MaterialValidator:AbstractValidator<MaterialRequest>
    {
        public MaterialValidator()
        {
            RuleFor(x => x.Name).NotNull().NotEmpty().WithMessage("The name is required!");
            RuleFor(x => x.Quantity).NotEmpty().NotNull().WithMessage("The quantity is required and greater or equal 0!");
            RuleFor(x => x.UnitMaterial).NotEmpty().NotNull().WithMessage("The material unit is required!");
            RuleFor(x => x.MaterialType).NotEmpty().NotNull().WithMessage("The material type is required!");
        }
    }
}
