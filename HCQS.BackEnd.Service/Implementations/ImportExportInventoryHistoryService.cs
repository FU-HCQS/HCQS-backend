using AutoMapper;
using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.BaseRequest;
using HCQS.BackEnd.Common.Dto.Record;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.Common.Util;
using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Models;
using HCQS.BackEnd.Service.Contracts;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System.Globalization;
using System.Transactions;

namespace HCQS.BackEnd.Service.Implementations
{
    public class ImportExportInventoryHistoryService : GenericBackendService, IImportExportInventoryHistoryService
    {
        private IImportExportInventoryHistoryRepository _importExportInventoryHistoryRepository;
        private IMapper _mapper;
        private BackEndLogger _logger;
        private IUnitOfWork _unitOfWork;

        public ImportExportInventoryHistoryService(IImportExportInventoryHistoryRepository importExportInventoryHistoryRepository, IMapper mapper, BackEndLogger logger, IUnitOfWork unitOfWork, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _importExportInventoryHistoryRepository = importExportInventoryHistoryRepository;
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public Task<AppActionResult> FulfillMaterialWithExcel(IFormFile file)
        {
            throw new NotImplementedException();
        }

        public async Task<AppActionResult> FulfillSingleMatertial(List<ImportExportInventoryRequest> ImportExportInventoryRequests)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var progressConstructionMaterialRepository = Resolve<IProgressConstructionMaterialRepository>();
                    foreach (var ImportExportInventoryRequest in ImportExportInventoryRequests)
                    {
                        if (ImportExportInventoryRequest.ProgressConstructionMaterialId.HasValue)
                        {
                            var progressConstructionMaterialDb = await progressConstructionMaterialRepository.GetById(ImportExportInventoryRequest.ProgressConstructionMaterialId);
                            if (progressConstructionMaterialDb == null)
                            {
                                result = BuildAppActionResultError(result, $"The progress construction material with id: {ImportExportInventoryRequest.ProgressConstructionMaterialId} does not exist!");
                            }
                            else
                            {
                                var quotationDetailRepository = Resolve<IQuotationDetailRepository>();
                                var quotationDetailDb = await quotationDetailRepository.GetById(progressConstructionMaterialDb.QuotationDetailId);
                                if (quotationDetailDb != null)
                                {
                                    var materialRepository = Resolve<IMaterialRepository>();
                                    var materialDb = await materialRepository.GetById(quotationDetailDb.MaterialId);
                                    if (materialDb != null)
                                    {
                                        if (ImportExportInventoryRequest.Quantity <= materialDb.Quantity)
                                        {
                                            var exportInventory = _mapper.Map<ImportExportInventoryHistory>(ImportExportInventoryRequest);
                                            exportInventory.Id = Guid.NewGuid();
                                            result.Result.Data = await _importExportInventoryHistoryRepository.Insert(exportInventory);
                                            if (result.Result.Data != null)
                                            {
                                                materialDb.Quantity -= ImportExportInventoryRequest.Quantity;
                                                await materialRepository.Update(materialDb);
                                                await _unitOfWork.SaveChangeAsync();
                                            }
                                        }
                                        else
                                        {
                                            result = BuildAppActionResultError(result, $"The current material quantity is less than demand!");
                                        }
                                    }
                                    else
                                    {
                                        result = BuildAppActionResultError(result, $"The material with id: {quotationDetailDb.MaterialId} does not exist!");
                                    }
                                }
                                else
                                {
                                    result = BuildAppActionResultError(result, $"The quotation detail with id: {progressConstructionMaterialDb.QuotationDetailId} does not exist!");
                                }
                            }
                        }
                        else
                        {
                            result = BuildAppActionResultError(result, SD.ResponseMessage.INTERNAL_SERVER_ERROR, true);
                        }
                    }

                    if (!BuildAppActionResultIsError(result))
                    {
                        scope.Complete();
                    }
                }
                catch (Exception ex)
                {
                    result = BuildAppActionResultError(result, SD.ResponseMessage.INTERNAL_SERVER_ERROR, true);
                    _logger.LogError(ex.Message, this);
                }
                return result;
            }
        }

        public async Task<AppActionResult> GetAll(int pageIndex, int pageSize, IList<SortInfo> sortInfos)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var supplierPriceDetailDb = await _importExportInventoryHistoryRepository.GetAllDataByExpression(null, null);
                    if (supplierPriceDetailDb.Any())
                    {
                        if (pageIndex <= 0) pageIndex = 1;
                        if (pageSize <= 0) pageSize = SD.MAX_RECORD_PER_PAGE;
                        int totalPage = DataPresentationHelper.CalculateTotalPageSize(supplierPriceDetailDb.Count(), pageSize);

                        if (sortInfos != null)
                        {
                            supplierPriceDetailDb = DataPresentationHelper.ApplySorting(supplierPriceDetailDb, sortInfos);
                        }
                        if (pageIndex > 0 && pageSize > 0)
                        {
                            supplierPriceDetailDb = DataPresentationHelper.ApplyPaging(supplierPriceDetailDb, pageIndex, pageSize);
                        }
                        result.Result.Data = supplierPriceDetailDb;
                        result.Result.TotalPage = totalPage;
                    }
                    else
                    {
                        result.Messages.Add("Empty supplier list");
                    }

                    if (!BuildAppActionResultIsError(result))
                    {
                        scope.Complete();
                    }
                }
                catch (Exception ex)
                {
                    result = BuildAppActionResultError(result, SD.ResponseMessage.INTERNAL_SERVER_ERROR, true);
                    _logger.LogError(ex.Message, this);
                }
                return result;
            }
        }

        public async Task<AppActionResult> GetAllExport(int pageIndex, int pageSize, IList<SortInfo> sortInfos)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var supplierPriceDetailDb = await _importExportInventoryHistoryRepository.GetAllDataByExpression(ie => !ie.SupplierPriceDetailId.HasValue && ie.ProgressConstructionMaterialId.HasValue, null);
                    if (supplierPriceDetailDb.Any())
                    {
                        if (pageIndex <= 0) pageIndex = 1;
                        if (pageSize <= 0) pageSize = SD.MAX_RECORD_PER_PAGE;
                        int totalPage = DataPresentationHelper.CalculateTotalPageSize(supplierPriceDetailDb.Count(), pageSize);

                        if (sortInfos != null)
                        {
                            supplierPriceDetailDb = DataPresentationHelper.ApplySorting(supplierPriceDetailDb, sortInfos);
                        }
                        if (pageIndex > 0 && pageSize > 0)
                        {
                            supplierPriceDetailDb = DataPresentationHelper.ApplyPaging(supplierPriceDetailDb, pageIndex, pageSize);
                        }
                        result.Result.Data = supplierPriceDetailDb;
                        result.Result.TotalPage = totalPage;
                    }
                    else
                    {
                        result.Messages.Add("Empty supplier list");
                    }

                    if (!BuildAppActionResultIsError(result))
                    {
                        scope.Complete();
                    }
                }
                catch (Exception ex)
                {
                    result = BuildAppActionResultError(result, SD.ResponseMessage.INTERNAL_SERVER_ERROR, true);
                    _logger.LogError(ex.Message, this);
                }
                return result;
            }
        }

        public async Task<AppActionResult> GetAllImport(int pageIndex, int pageSize, IList<SortInfo> sortInfos)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var supplierPriceDetailDb = await _importExportInventoryHistoryRepository.GetAllDataByExpression(ie => ie.SupplierPriceDetailId.HasValue && !ie.ProgressConstructionMaterialId.HasValue, null);
                    if (supplierPriceDetailDb.Any())
                    {
                        if (pageIndex <= 0) pageIndex = 1;
                        if (pageSize <= 0) pageSize = SD.MAX_RECORD_PER_PAGE;
                        int totalPage = DataPresentationHelper.CalculateTotalPageSize(supplierPriceDetailDb.Count(), pageSize);

                        if (sortInfos != null)
                        {
                            supplierPriceDetailDb = DataPresentationHelper.ApplySorting(supplierPriceDetailDb, sortInfos);
                        }
                        if (pageIndex > 0 && pageSize > 0)
                        {
                            supplierPriceDetailDb = DataPresentationHelper.ApplyPaging(supplierPriceDetailDb, pageIndex, pageSize);
                        }
                        result.Result.Data = supplierPriceDetailDb;
                        result.Result.TotalPage = totalPage;
                    }
                    else
                    {
                        result.Messages.Add("Empty supplier list");
                    }

                    if (!BuildAppActionResultIsError(result))
                    {
                        scope.Complete();
                    }
                }
                catch (Exception ex)
                {
                    result = BuildAppActionResultError(result, SD.ResponseMessage.INTERNAL_SERVER_ERROR, true);
                    _logger.LogError(ex.Message, this);
                }
                return result;
            }
        }

        public async Task<AppActionResult> ImportMaterial(List<ImportExportInventoryRequest> ImportExportInventoryRequests)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var supplierPriceDetailRepository = Resolve<ISupplierPriceDetailRepository>();
                    foreach (var ImportExportInventoryRequest in ImportExportInventoryRequests)
                    {
                        if (ImportExportInventoryRequest.SupplierPriceDetailId.HasValue)
                        {
                            var supplierPriceDetailDb = await supplierPriceDetailRepository.GetById(ImportExportInventoryRequest.SupplierPriceDetailId);
                            if (supplierPriceDetailDb == null)
                            {
                                result = BuildAppActionResultError(result, $"The supplier price detail with id: {ImportExportInventoryRequest.SupplierPriceDetailId} does not exist!");
                            }
                            else
                            {
                                if (ImportExportInventoryRequest.Quantity >= supplierPriceDetailDb.MOQ)
                                {
                                    var importInventory = _mapper.Map<ImportExportInventoryHistory>(ImportExportInventoryRequest);
                                    importInventory.Id = Guid.NewGuid();
                                    result.Result.Data = await _importExportInventoryHistoryRepository.Insert(importInventory);
                                    if (result.Result.Data != null)
                                    {
                                        var materialRepository = Resolve<IMaterialRepository>();
                                        var materialDb = await materialRepository.GetByExpression(m => m.Id == supplierPriceDetailDb.MaterialId);
                                        if (materialDb != null)
                                        {
                                            materialDb.Quantity += ImportExportInventoryRequest.Quantity;
                                            await materialRepository.Update(materialDb);
                                            await _unitOfWork.SaveChangeAsync();
                                        }
                                        else
                                        {
                                            result = BuildAppActionResultError(result, $"The material queried from supplier price detail with id: {materialDb.Id} does not exist!");
                                        }
                                    }
                                }
                                else
                                {
                                    result = BuildAppActionResultError(result, $"Import quantity: {ImportExportInventoryRequest.Quantity} is less than MOQ:{supplierPriceDetailDb.MOQ}");
                                }
                            }
                        }
                        else
                        {
                            result = BuildAppActionResultError(result, SD.ResponseMessage.INTERNAL_SERVER_ERROR, true);
                        }
                    }

                    if (!BuildAppActionResultIsError(result))
                    {
                        scope.Complete();
                    }
                }
                catch (Exception ex)
                {
                    result = BuildAppActionResultError(result, SD.ResponseMessage.INTERNAL_SERVER_ERROR, true);
                    _logger.LogError(ex.Message, this);
                }
                return result;
            }
        }

        public async Task<AppActionResult> ImportMaterialWithExcel(IFormFile file)
        {
            AppActionResult result = new AppActionResult();
            if (file == null || file.Length == 0)
            {
                result.Result.Data = null;
                result.Messages.Add("Empty Excel file");
            }
            else
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        //Format: Name_ddmmyyy
                        string dateString = file.FileName.Substring(0, 8);
                        if (!DateTime.TryParseExact(dateString, "ddMMyyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                        {
                            result = BuildAppActionResultError(result, SD.ResponseMessage.INTERNAL_SERVER_ERROR, true);
                            _logger.LogError($"{dateString} is not in format: ddMMyyyy", this);
                        }
                        else
                        {
                            Dictionary<String, Guid> materials = new Dictionary<String, Guid>();
                            Dictionary<String, Guid> suppliers = new Dictionary<String, Guid>();
                            List<ImportInventoryRecord> records = await GetImportListFromExcel(file);
                            List<ImportExportInventoryHistory> importInventoryList = new List<ImportExportInventoryHistory>();
                            var materialRepository = Resolve<IMaterialRepository>();
                            var supplierRepository = Resolve<ISupplierRepository>();
                            var supplierPriceDetailRepository = Resolve<ISupplierPriceDetailRepository>();
                            HashSet<int> invalidRowInput = new HashSet<int>();
                            int i = 2;
                            foreach (ImportInventoryRecord record in records)
                            {
                                Guid materialId = Guid.Empty;
                                Guid supplierId = Guid.Empty;
                                if (materials.ContainsKey(record.MaterialName)) materialId = materials[record.MaterialName];
                                else
                                {
                                    var material = await materialRepository.GetByExpression(m => m.Name.Equals(record.MaterialName));
                                    if (material == null)
                                    {
                                        invalidRowInput.Add(i);
                                    }
                                    else
                                    {
                                        materialId = material.Id;
                                        materials.Add(record.MaterialName, materialId);
                                    }
                                }

                                if (suppliers.ContainsKey(record.SupplierName)) supplierId = suppliers[record.SupplierName];
                                else
                                {
                                    var supplier = await supplierRepository.GetByExpression(m => m.SupplierName.Equals(record.SupplierName));
                                    if (supplier == null)
                                    {
                                        invalidRowInput.Add(i);
                                    }
                                    else
                                    {
                                        supplierId = supplier.Id;
                                        suppliers.Add(record.SupplierName, supplierId);
                                    }
                                }

                                if (invalidRowInput.Count == 0)
                                {
                                    var supplierPriceDetailId = await GetSupplierQuationDetail(materialId, supplierId, record.Quantity);
                                    if (supplierPriceDetailId != Guid.Empty)
                                    {
                                        var newImportInventory = new ImportExportInventoryHistory()
                                        {
                                            Id = Guid.NewGuid(),
                                            Quantity = record.Quantity,
                                            Date = date,
                                            SupplierPriceDetailId = supplierPriceDetailId
                                        };
                                        importInventoryList.Add(newImportInventory);
                                    }
                                    else
                                    {
                                        invalidRowInput.Add(i);
                                    }
                                }
                                i++;
                            }

                            if (invalidRowInput.Count > 0)
                            {
                                List<List<string>> recordDataString = new List<List<string>>();
                                int j = 1;
                                foreach (var record in records)
                                {
                                    recordDataString.Add(new List<string>
                                        {
                                            j++.ToString(), record.MaterialName, record.SupplierName, record.Quantity.ToString()
                                        });
                                }
                                ExcelExporter.ExportToExcel(SD.ExcelHeaders.IMPORT_INVENTORY, recordDataString, invalidRowInput, ExcelExporter.GetDownloadsPath(file.FileName));
                                result = BuildAppActionResultError(result, $"Invalid rows are colored in the excel file!");
                            }

                            if (!BuildAppActionResultIsError(result))
                            {
                                await _importExportInventoryHistoryRepository.InsertRange(importInventoryList);
                                await _unitOfWork.SaveChangeAsync();
                                result.Result.Data = importInventoryList;
                            }

                            if (!BuildAppActionResultIsError(result))
                            {
                                scope.Complete();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        result = BuildAppActionResultError(result, SD.ResponseMessage.INTERNAL_SERVER_ERROR, true);
                        _logger.LogError(ex.Message, this);
                    }
                }
            }
            return result;
        }

        public async Task<AppActionResult> UpdateInventory(Guid Id, ImportExportInventoryRequest ImportExportInventoryRequest)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var supplierDb = await _importExportInventoryHistoryRepository.GetByExpression(n => n.Id.Equals(Id));
                    if (supplierDb == null)
                    {
                        result = BuildAppActionResultError(result, $"The supplier with {Id} not found !");
                    }
                    else
                    {
                        supplierDb.Quantity = ImportExportInventoryRequest.Quantity;
                        supplierDb.Date = ImportExportInventoryRequest.Date;
                        result.Result.Data = await _importExportInventoryHistoryRepository.Update(supplierDb);
                        await _unitOfWork.SaveChangeAsync();
                    }

                    if (!BuildAppActionResultIsError(result))
                    {
                        scope.Complete();
                    }
                }
                catch (Exception ex)
                {
                    result = BuildAppActionResultError(result, SD.ResponseMessage.INTERNAL_SERVER_ERROR, true);
                    _logger.LogError(ex.Message, this);
                }
                return result;
            }
        }

        private async Task<List<ImportInventoryRecord>> GetImportListFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }

            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    using (ExcelPackage package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Assuming data is in the first sheet

                        int rowCount = worksheet.Dimension.Rows;
                        int colCount = worksheet.Dimension.Columns;

                        List<ImportInventoryRecord> records = new List<ImportInventoryRecord>();

                        for (int row = 2; row <= rowCount; row++) // Assuming header is in the first row
                        {
                            ImportInventoryRecord record = new ImportInventoryRecord()
                            {
                                Id = Guid.NewGuid(),
                                MaterialName = worksheet.Cells[row, 2].Value.ToString(),
                                SupplierName = worksheet.Cells[row, 3].Value.ToString(),
                                Quantity = int.Parse(worksheet.Cells[row, 4].Value.ToString())
                            };
                            records.Add(record);
                        }
                        return records;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, this);
            }
            return null;
        }

        private async Task<Guid> GetSupplierQuationDetail(Guid materialId, Guid supplierId, int quantity)
        {
            try
            {
                var supplierPriceDetailRepository = Resolve<ISupplierPriceDetailRepository>();
                var supplierPriceDetails = await supplierPriceDetailRepository
                    .GetAllDataByExpression(s => s.MaterialId == materialId && s.SupplierPriceQuotation.SupplierId == supplierId, s => s.SupplierPriceQuotation);
                if (supplierPriceDetails != null)
                {
                    var supplierPriceDetailWithLatestDate = supplierPriceDetails.OrderByDescending(s => s.SupplierPriceQuotation.Date).FirstOrDefault();
                    var res = supplierPriceDetails.Where(s => s.MOQ <= quantity && s.SupplierPriceQuotation.Date == supplierPriceDetailWithLatestDate.SupplierPriceQuotation.Date).OrderByDescending(s => s.MOQ).FirstOrDefault();
                    return res.Id;
                }
                return Guid.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, this);
            }
            return Guid.Empty;
        }
    }
}