using FluentValidation;
using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.Common.Validator;
using HCQS.BackEnd.Service.Contracts;
using HCQS.BackEnd.Service.Implementations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;

namespace HCQS.BackEnd.API.Controllers
{
    [Route("worker")]
    [ApiController]
    public class WorkerPriceController : ControllerBase
    {
        private readonly HandleErrorValidator _handleErrorValidator;
        private IWorkerPriceService _workerPriceService;
        private readonly IValidator<WorkerPriceRequest> _validator;

        public WorkerPriceController(HandleErrorValidator handleErrorValidator, IWorkerPriceService workerPriceService, IValidator<WorkerPriceRequest> validator)
        {
            _handleErrorValidator = handleErrorValidator;
            _workerPriceService = workerPriceService;
            _validator = validator;
        }


        //var result = await _validator.ValidateAsync(request);
        //    if (!result.IsValid)
        //    {
        //        return _handleErrorValidator.HandleError(result);
        //    }
        [HttpGet("get-all")]
        public async Task<AppActionResult> GetAll()
        {
            return await _workerPriceService.GetAll();
        }
        [HttpGet("get-by-postion-name/{name}")]
        public async Task<AppActionResult> GetByPositionName(string name)
        {
            return await _workerPriceService.GetByPositionName(name);
        }
        [HttpGet("get-by-id/{id}")]
        public async Task<AppActionResult> GetByPositionName(Guid id)
        {
            return await _workerPriceService.GetById(id);
        }
        [HttpPost("create-worker-price")]
        public async Task<AppActionResult> CreateWokerPrice(WorkerPriceRequest request)
        {
            var result = await _validator.ValidateAsync(request);
            if (!result.IsValid)
            {
                return _handleErrorValidator.HandleError(result);
            }
            return await _workerPriceService.CreateWokerPrice(request);
        }
        [HttpPut("update-worker-price")]
        public async Task<AppActionResult> UpdateWorkerPrice(WorkerPriceRequest request)
        {
            var result = await _validator.ValidateAsync(request);
            if (!result.IsValid)
            {
                return _handleErrorValidator.HandleError(result);
            }
            return await _workerPriceService.UpdateWorkerPrice(request);
        }
        [HttpDelete("delete-worker-price-by-id/{id}")]
        public async Task<AppActionResult> DeleteWorkerPriceById(Guid id)
        {
            return await _workerPriceService.DeleteWorkerPriceById(id);
        }
    }
}
