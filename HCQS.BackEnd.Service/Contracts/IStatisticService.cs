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

        public Task<AppActionResult> getTotalImport();

        public Task<AppActionResult> getTotalExport();

        public Task<AppActionResult> getIncreaseProject(int year = -1, int timePeriod = 1);

        public Task<AppActionResult> getIncreaseContract(int year = -1, int timePeriod = 1);

        public Task<AppActionResult> getIncreaseImportInventory(int year = -1, int timePeriod = 1);

        public Task<AppActionResult> getIncreaseExportInventory(int year = -1, int timePeriod = 1);

        public Task<AppActionResult> getAccountRatio();

        public Task<AppActionResult> getProjectContructionTypeRatio(int year = -1, int timePeriod = 1);

        public Task<AppActionResult> getProjectStatusRatio(int year = -1, int timePeriod = 1);

        public Task<AppActionResult> getContractRatio(int year = -1, int timePeriod = 1);

        public Task<AppActionResult> getTotalImportBySupplierRatio(int year = -1, int timePeriod = 1);

        public Task<AppActionResult> getTotalImportByMaterialRatio(int year = -1, int timePeriod = 1);

        public Task<AppActionResult> getTotalExportByMaterialRatio(int year = -1, int timePeriod = 1);

        public Task<AppActionResult> getQuantityImportByMaterialRatio(int year = -1, int timePeriod = 1);

        public Task<AppActionResult> getQuantityExportByMaterialRatio(int year = -1, int timePeriod = 1);
    }
}