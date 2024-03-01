using FluentValidation;
using HCQS.BackEnd.Common.Dto.Request;

namespace HCQS.BackEnd.Common.Validator
{
    public class DeleteConstructionConfigValidator : AbstractValidator<DeleteConstructionConfigRequest>
    {
        public DeleteConstructionConfigValidator()
        {
            RuleFor(x => x.NumOfFloor).NotNull().NotEmpty().WithMessage("The number of floor is required!");
            RuleFor(x => x.Area).NotNull().NotEmpty().WithMessage("The area is required!");
            RuleFor(x => x.TiledArea).NotNull().NotEmpty().WithMessage("The tiled area is required!");
            RuleFor(x => x.ConstructionType).NotNull().WithMessage("The construction type is required!");
        }
    }
}