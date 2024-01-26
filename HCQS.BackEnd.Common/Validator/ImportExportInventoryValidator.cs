using FluentValidation;
using HCQS.BackEnd.Common.Dto.Request;

namespace HCQS.BackEnd.Common.Validator
{
    public class ImportExportInventoryValidator : AbstractValidator<ImportExportInventoryRequest>
    {
        public ImportExportInventoryValidator()
        {
            RuleFor(x => x.Quantity).NotEmpty().NotNull().GreaterThan(0).WithMessage("The Quantity is required!");
            RuleFor(x => x)
            .Must(x => x.SupplierPriceDetailId.HasValue ^ x.ProgressConstructionMaterialId.HasValue)
            .WithMessage("Either SupplierPriceDetailId or ProgressConstructionMaterialId must be not null, but not both.");
        }
    }
}