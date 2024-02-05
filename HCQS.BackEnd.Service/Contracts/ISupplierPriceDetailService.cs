using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.BaseRequest;

namespace HCQS.BackEnd.Service.Contracts
{
    public interface ISupplierPriceDetailService
    {
        public Task<AppActionResult> GetQuotationPriceById(Guid Id);

        public Task<AppActionResult> GetQuotationPricesByMaterialId(Guid Id, int pageIndex, int pageSize, IList<SortInfo> sortInfos);

        public Task<AppActionResult> GetQuotationPricesByMaterialName(string name, int pageIndex, int pageSize, IList<SortInfo> sortInfos);

        public Task<AppActionResult> GetQuotationPricesBySupplierId(Guid Id, int pageIndex, int pageSize, IList<SortInfo> sortInfos);

        public Task<AppActionResult> GetQuotationPricesBySupplierName(string name, int pageIndex, int pageSize, IList<SortInfo> sortInfos);

        public Task<AppActionResult> GetLatestQuotationPricesByMaterialId(Guid Id, int pageIndex, int pageSize, IList<SortInfo> sortInfos);

        public Task<AppActionResult> GetLatestQuotationPricesByMaterialName(string name, int pageIndex, int pageSize, IList<SortInfo> sortInfos);

        public Task<AppActionResult> GetLatestQuotationPricesBySupplierId(Guid Id, int pageIndex, int pageSize, IList<SortInfo> sortInfos);

        public Task<AppActionResult> GetLatestQuotationPricesBySupplierName(string name, int pageIndex, int pageSize, IList<SortInfo> sortInfos);

        //public Task<AppActionResult> GetQuotationPricesBySupplierandMaterialName(string supplierName, string materialName, int pageIndex, int pageSize, IList<SortInfo> sortInfos);

        public Task<AppActionResult> GetAll(int pageIndex, int pageSize, IList<SortInfo> sortInfos);
    }
}