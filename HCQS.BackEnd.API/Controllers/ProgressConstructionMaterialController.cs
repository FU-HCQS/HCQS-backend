using FluentValidation;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.Common.Validator;
using HCQS.BackEnd.Service.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace HCQS.BackEnd.API.Controllers
{
    [Route("progress-construction-material")]
    [ApiController]
    public class ProgressConstructionMaterialController : Controller
    {
        private readonly IValidator<ProgressConstructionMaterialRequest> _validator;
        private readonly HandleErrorValidator _handleErrorValidator;
        private IProgressConstructionMaterialService _progressConstructionMaterialService;
    }
}