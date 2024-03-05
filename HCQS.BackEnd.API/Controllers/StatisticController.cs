using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Service.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace HCQS.BackEnd.API.Controllers
{
    [Route("statistic")]
    [ApiController]
    public class StatisticController : Controller
    {
        private IStatisticService _statisticService;

        public StatisticController(IStatisticService statisticService)
        {
            _statisticService = statisticService;
        }

        [HttpPost("get-total-account")]
        public async Task<AppActionResult> GetTotalAccount()
        {
            return await _statisticService.GetTotalAccount();
        }

        [HttpPost("get-total-project")]
        public async Task<AppActionResult> GetTotalProject()
        {
            return await _statisticService.GetTotalProject();
        }

        [HttpPost("get-total-contract")]
        public async Task<AppActionResult> GetTotalContract()
        {
            return await _statisticService.GetTotalContract();
        }

        [HttpPost("get-total-quotation-request")]
        public async Task<AppActionResult> GetTotalQuotationRequest()
        {
            return await _statisticService.GetTotalQuotationRequest();
        }

        [HttpPost("get-total-import")]
        public async Task<AppActionResult> GetTotalImport()
        {
            return await _statisticService.GetTotalImport();
        }

        [HttpPost("get-total-export")]
        public async Task<AppActionResult> GetTotalExport()
        {
            return await _statisticService.GetTotalExport();
        }

        [HttpPost("get-increase-project")]
        public async Task<AppActionResult> GetIncreaseProject(int year, int timePeriod)
        {
            return await _statisticService.GetIncreaseProject(year, timePeriod);
        }

        [HttpPost("get-increase-contract")]
        public async Task<AppActionResult> GetIncreaseContract(int year, int timePeriod)
        {
            return await _statisticService.GetIncreaseContract(year, timePeriod);
        }

        [HttpPost("get-increase-import")]
        public async Task<AppActionResult> GetIncreaseImport(int year, int timePeriod)
        {
            return await _statisticService.GetIncreaseImportInventory(year, timePeriod);
        }

        [HttpPost("get-increase-export")]
        public async Task<AppActionResult> GetIncreaseExport(int year, int timePeriod)
        {
            return await _statisticService.GetIncreaseExportInventory(year, timePeriod);
        }

        [HttpPost("get-increase-import-export")]
        public async Task<AppActionResult> GetIncreaseImportExport(int year, int timePeriod)
        {
            return await _statisticService.GetIncreaseImportExportInventory(year, timePeriod);
        }

        [HttpPost("get-account-ratio")]
        public async Task<AppActionResult> GetAccountRatio()
        {
            return await _statisticService.GetAccountRatio();
        }

        [HttpPost("get-project-construction-type-ratio")]
        public async Task<AppActionResult> GetProjectConstructionTypeRatio(int year, int timePeriod)
        {
            return await _statisticService.GetProjectContructionTypeRatio(year, timePeriod);
        }

        [HttpPost("get-project-status-ratio")]
        public async Task<AppActionResult> GetProjectStatusRatio(int year, int timePeriod)
        {
            return await _statisticService.GetProjectStatusRatio(year, timePeriod);
        }

        [HttpPost("get-contract-ratio")]
        public async Task<AppActionResult> GetContractRatio(int year, int timePeriod)
        {
            return await _statisticService.GetContractRatio(year, timePeriod);
        }

        [HttpPost("get-total-import-by-supplier-ratio")]
        public async Task<AppActionResult> getTotalImportBySupplierRatio(int year, int timePeriod)
        {
            return await _statisticService.GetTotalImportBySupplierRatio(year, timePeriod);
        }

        [HttpPost("get-total-import-by-material-ratio")]
        public async Task<AppActionResult> getTotalImportByMaterialRatio(int year, int timePeriod)
        {
            return await _statisticService.GetTotalImportByMaterialRatio(year, timePeriod);
        }

        [HttpPost("get-total-export-by-material-ratio")]
        public async Task<AppActionResult> getTotalExportByMaterialRatio(int year, int timePeriod)
        {
            return await _statisticService.GetTotalExportByMaterialRatio(year, timePeriod);
        }

        [HttpPost("get-quantity-import-by-material-ratio")]
        public async Task<AppActionResult> getQuantityImportByMaterialRatio(int year, int timePeriod)
        {
            return await _statisticService.GetQuantityImportByMaterialRatio(year, timePeriod);
        }

        [HttpPost("get-quantity-export-by-material-ratio")]
        public async Task<AppActionResult> getQuantityExportByMaterialRatio(int year, int timePeriod)
        {
            return await _statisticService.GetQuantityExportByMaterialRatio(year, timePeriod);
        }
    }
}