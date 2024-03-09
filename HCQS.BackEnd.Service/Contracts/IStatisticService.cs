using HCQS.BackEnd.Common.Dto;

namespace HCQS.BackEnd.Service.Contracts
{
    public interface IStatisticService
    {
        //Revenue
        //Money stream of import and export
        //Number of new account
        public Task<AppActionResult> GetTotalAccount();

        public Task<AppActionResult> GetTotalProject();

        public Task<AppActionResult> GetTotalContract();

        public Task<AppActionResult> GetTotalQuotationRequest();

        public Task<AppActionResult> GetTotalImport();

        public Task<AppActionResult> GetTotalExport();

        public Task<AppActionResult> GetIncreaseProject(int year = -1, int timePeriod = 1);

        public Task<AppActionResult> GetIncreaseContract(int year = -1, int timePeriod = 1);

        public Task<AppActionResult> GetIncreaseImportExportInventory(int year = -1, int timePeriod = 1);

        public Task<AppActionResult> GetIncreaseImportInventory(int year = -1, int timePeriod = 1);

        public Task<AppActionResult> GetIncreaseExportInventory(int year = -1, int timePeriod = 1);

        public Task<AppActionResult> GetAccountRatio();

        public Task<AppActionResult> GetProjectContructionTypeRatio(int year = -1, int timePeriod = 1);

        public Task<AppActionResult> GetProjectStatusRatio(int year = -1, int timePeriod = 1);

        public Task<AppActionResult> GetContractRatio(int year = -1, int timePeriod = 1);

        public Task<AppActionResult> GetTotalImportBySupplierRatio(int year = -1, int timePeriod = 1);

        public Task<AppActionResult> GetTotalImportByMaterialRatio(int year = -1, int timePeriod = 1);

        public Task<AppActionResult> GetTotalExportByMaterialRatio(int year = -1, int timePeriod = 1);

        public Task<AppActionResult> GetQuantityImportByMaterialRatio(int year = -1, int timePeriod = 1);

        public Task<AppActionResult> GetQuantityExportByMaterialRatio(int year = -1, int timePeriod = 1);
    }
}