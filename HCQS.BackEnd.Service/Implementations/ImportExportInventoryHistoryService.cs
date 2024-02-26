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
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Globalization;
using System.Text;
using System.Transactions;

namespace HCQS.BackEnd.Service.Implementations
{
    public class ImportExportInventoryHistoryService : GenericBackendService, IImportExportInventoryHistoryService
    {
        private IImportExportInventoryHistoryRepository _importExportInventoryHistoryRepository;
        private IMapper _mapper;
        private BackEndLogger _logger;
        private IUnitOfWork _unitOfWork;
        private IFileService _fileService;

        public ImportExportInventoryHistoryService(IImportExportInventoryHistoryRepository importExportInventoryHistoryRepository, IMapper mapper, BackEndLogger logger, IUnitOfWork unitOfWork, IFileService fileService, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _importExportInventoryHistoryRepository = importExportInventoryHistoryRepository;
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }

        public Task<AppActionResult> FulfillMaterialWithExcel(IFormFile file)
        {
            throw new NotImplementedException();
        }

        public async Task<AppActionResult> FulfillMatertial(List<ImportExportInventoryRequest> ImportExportInventoryRequests)
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
                            result = BuildAppActionResultError(result, "Progress construction material Id is null");
                        }
                    }

                    if (!BuildAppActionResultIsError(result))
                    {
                        scope.Complete();
                    }
                }
                catch (Exception ex)
                {
                    result = BuildAppActionResultError(result, ex.Message);
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
                        result.Messages.Add("Empty inventory history list");
                    }

                    if (!BuildAppActionResultIsError(result))
                    {
                        scope.Complete();
                    }
                }
                catch (Exception ex)
                {
                    result = BuildAppActionResultError(result, ex.Message);
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
                        result.Messages.Add("Empty inventory history list");
                    }

                    if (!BuildAppActionResultIsError(result))
                    {
                        scope.Complete();
                    }
                }
                catch (Exception ex)
                {
                    result = BuildAppActionResultError(result, ex.Message);
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
                        result.Messages.Add("Empty inventory history list");
                    }

                    if (!BuildAppActionResultIsError(result))
                    {
                        scope.Complete();
                    }
                }
                catch (Exception ex)
                {
                    result = BuildAppActionResultError(result, ex.Message);
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
                            //Check valid supplier price detail

                            var supplierPriceDetailDb = await supplierPriceDetailRepository.GetByExpression(s => s.Id == ImportExportInventoryRequest.SupplierPriceDetailId, s => s.SupplierPriceQuotation);
                            if (supplierPriceDetailDb == null || supplierPriceDetailDb.SupplierPriceQuotation == null)
                            {
                                result = BuildAppActionResultError(result, $"The supplier price detail with id: {ImportExportInventoryRequest.SupplierPriceDetailId} does not exist!");
                            }
                            else
                            {
                                var allMaterialSupplierPriceDetailFromSameSupplierDb = await supplierPriceDetailRepository.GetAllDataByExpression(s => s.MaterialId == supplierPriceDetailDb.MaterialId && s.SupplierPriceQuotation.SupplierId == s.SupplierPriceQuotation.SupplierId, s => s.SupplierPriceQuotation);
                                var latestSupplierPriceDetail = allMaterialSupplierPriceDetailFromSameSupplierDb.OrderByDescending(s => s.SupplierPriceQuotation.Date).FirstOrDefault();
                                if (latestSupplierPriceDetail != null && latestSupplierPriceDetail.SupplierPriceQuotation != null && latestSupplierPriceDetail.SupplierPriceQuotation.Date == supplierPriceDetailDb.SupplierPriceQuotation.Date)
                                {
                                    if (ImportExportInventoryRequest.Quantity >= supplierPriceDetailDb.MOQ)
                                    {
                                        //Maybe there better price :>>
                                        var betterPriceSupplierQuotation = await supplierPriceDetailRepository.GetAllDataByExpression(s => s.MaterialId == latestSupplierPriceDetail.MaterialId && s.SupplierPriceQuotationId == latestSupplierPriceDetail.SupplierPriceQuotationId && s.MOQ < ImportExportInventoryRequest.Quantity);
                                        var bestPrice = betterPriceSupplierQuotation.OrderBy(s => s.Price).FirstOrDefault();
                                        if (bestPrice.Id != ImportExportInventoryRequest.SupplierPriceDetailId)
                                        {
                                            result = BuildAppActionResultError(result, $"Please choose supplier price detail: {bestPrice.Id} rather than {ImportExportInventoryRequest.SupplierPriceDetailId} for better price as import quantity is higher than its MOQ!");
                                        }
                                        else
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
                                    }
                                    else
                                    {
                                        result = BuildAppActionResultError(result, $"Import quantity: {ImportExportInventoryRequest.Quantity} is less than MOQ:{supplierPriceDetailDb.MOQ}");
                                    }
                                }
                                else
                                {
                                    result = BuildAppActionResultError(result, $"Supplier price quotation detail with id: {ImportExportInventoryRequest.SupplierPriceDetailId} is out of date");
                                }
                            }
                        }
                        else
                        {
                            result = BuildAppActionResultError(result, $"Supplier price detail Id is null");
                        }
                    }

                    if (!BuildAppActionResultIsError(result))
                    {
                        scope.Complete();
                    }
                }
                catch (Exception ex)
                {
                    result = BuildAppActionResultError(result, ex.Message);
                    _logger.LogError(ex.Message, this);
                }
                return result;
            }
        }

        public async Task<IActionResult> ImportMaterialWithExcel(IFormFile file)
        {
            IActionResult result = new ObjectResult(null) { StatusCode = 200 };
            string message = "";
            if (file == null || file.Length == 0)
            {
                return result;
            }
            else
            {
                bool isSuccessful = true;
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        //Format: ddMMyyyy
                        string dateString = file.FileName;
                        if (dateString.Contains('_'))
                        {
                            return new ObjectResult("Invalid file name. Please follow format: ddMMyyyy") { StatusCode = 200 };
                        }
                        if (file.FileName.Contains("(ErrorColor)"))
                            dateString = dateString.Substring("(ErrorColor)".Length);
                        if (dateString.Length < 8)
                        {
                            return new ObjectResult("Invalid date. Please follow date format: ddMMyyyy") { StatusCode = 200 };
                        }
                        dateString = dateString.Substring(0, 8);
                        if (!DateTime.TryParseExact(dateString, "ddMMyyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                        {
                            isSuccessful = false;
                            _logger.LogError($"{dateString} is not in format: ddMMyyyy", this);
                            message = $"{dateString} is not in format: ddMMyyyy";
                        }
                        else
                        {
                            if (!(await CheckHeader(file, SD.ExcelHeaders.IMPORT_INVENTORY)))
                            {
                                isSuccessful = false;
                                _logger.LogError($"Incompatible header to import material template", this);
                                message = $"Incompatible header to import material template";
                            }
                            else
                            {
                                Dictionary<String, Guid> materials = new Dictionary<String, Guid>();
                                Dictionary<Guid, int> materialImport = new Dictionary<Guid, int>();
                                Dictionary<String, Guid> suppliers = new Dictionary<String, Guid>();
                                List<ImportInventoryRecord> records = await GetImportListFromExcel(file);
                                List<ImportExportInventoryHistory> importInventoryList = new List<ImportExportInventoryHistory>();
                                var materialRepository = Resolve<IMaterialRepository>();
                                var supplierRepository = Resolve<ISupplierRepository>();
                                var supplierPriceDetailRepository = Resolve<ISupplierPriceDetailRepository>();

                                Dictionary<int, string> invalidRowInput = new Dictionary<int, string>();
                                int errorRecordCount = 0;
                                int i = 2;
                                foreach (ImportInventoryRecord record in records)
                                {
                                    StringBuilder error = new StringBuilder();
                                    errorRecordCount = 0;
                                    Guid materialId = Guid.Empty;
                                    Guid supplierId = Guid.Empty;
                                    if (!string.IsNullOrEmpty(record.MaterialName) && !string.IsNullOrEmpty(record.SupplierName))
                                    {

                                        if (materials.ContainsKey(record.MaterialName))
                                        {
                                            materialId = materials[record.MaterialName];
                                            materialImport[materialId] += record.Quantity;
                                        }
                                        else
                                        {
                                            var material = await materialRepository.GetByExpression(m => m.Name.Equals(record.MaterialName));
                                            if (material == null)
                                            {
                                                error.Append($"{errorRecordCount + 1}. Material with name {record.MaterialName} does not exist.\n");
                                                errorRecordCount++;
                                            }
                                            else
                                            {
                                                materialId = material.Id;
                                                materials.Add(record.MaterialName, materialId);
                                                materialImport.Add(material.Id, record.Quantity);
                                            }
                                        }

                                        if (suppliers.ContainsKey(record.SupplierName)) supplierId = suppliers[record.SupplierName];
                                        else
                                        {
                                            var supplier = await supplierRepository.GetByExpression(m => m.SupplierName.Equals(record.SupplierName));
                                            if (supplier == null)
                                            {
                                                error.Append($"{errorRecordCount + 1}. Supplier with name {record.SupplierName} does not exist.\n");
                                                errorRecordCount++;
                                            }
                                            else
                                            {
                                                supplierId = supplier.Id;
                                                suppliers.Add(record.SupplierName, supplierId);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        error.Append($"{errorRecordCount + 1}. Cell Material name or Supplier name is empty.\n");
                                        errorRecordCount++;
                                    }

                                    if (record.Quantity <= 0)
                                    {
                                        error.Append($"{errorRecordCount + 1}. Quantity must be higher than 0.\n");
                                        errorRecordCount++;
                                    }

                                    var supplierPriceDetailId = await GetSupplierQuationDetail(materialId, supplierId, record.Quantity);
                                    if (supplierPriceDetailId == Guid.Empty)
                                    {
                                        error.Append($"{errorRecordCount + 1}. Unable to find compatible supplier price quotation detail.\n");
                                        errorRecordCount++;
                                    }
                                    if (errorRecordCount == 0)
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
                                        error.Append("(Please delete this error message cell before re-uploading!)");
                                        invalidRowInput.Add(i, error.ToString());
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
                                    result = _fileService.ReturnErrorColored<ImportInventoryRecord>(SD.ExcelHeaders.IMPORT_INVENTORY, recordDataString, invalidRowInput, dateString);
                                    isSuccessful = false;
                                }

                                if (isSuccessful)
                                {
                                    await _importExportInventoryHistoryRepository.InsertRange(importInventoryList);
                                    foreach(var materialId in materialImport.Keys)
                                    {
                                        var materialDb = await materialRepository.GetById(materialId);
                                        materialDb.Quantity += materialImport[materialId];
                                        await materialRepository.Update(materialDb);
                                    }
                                    await _unitOfWork.SaveChangeAsync();
                                    result = new ObjectResult(importInventoryList) { StatusCode = 200 };
                                }
                            }

                            if (isSuccessful)
                            {
                                scope.Complete();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message, this);
                    }
                }
            }
            if (!string.IsNullOrEmpty(message)) return new ObjectResult(message) { StatusCode = 200 };
            return result;
        }

        public async Task<AppActionResult> UpdateInventory(ImportExportInventoryRequest ImportExportInventoryRequest)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var inventoryDb = await _importExportInventoryHistoryRepository.GetByExpression(n => n.Id.Equals(ImportExportInventoryRequest.Id));
                    if (inventoryDb == null)
                    {
                        result = BuildAppActionResultError(result, $"The inventory history with {ImportExportInventoryRequest.Id} not found !");
                    }
                    else
                    {
                        inventoryDb.Quantity = ImportExportInventoryRequest.Quantity;
                        var utility = Resolve<Utility>();
                        inventoryDb.Date = utility.GetCurrentDateTimeInTimeZone();
                        result.Result.Data = await _importExportInventoryHistoryRepository.Update(inventoryDb);
                        await _unitOfWork.SaveChangeAsync();
                    }

                    if (!BuildAppActionResultIsError(result))
                    {
                        scope.Complete();
                    }
                }
                catch (Exception ex)
                {
                    result = BuildAppActionResultError(result, ex.Message);
                    _logger.LogError(ex.Message, this);
                }
                return result;
            }
        }

        public async Task<IActionResult> GetImportInventoryTempate()
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                IActionResult result = null;
                try
                {
                    List<ImportInventoryRecord> sampleData = new List<ImportInventoryRecord>();
                    sampleData.Add(new ImportInventoryRecord { MaterialName = "Brick", SupplierName = "inventory history name", Quantity = 999 });
                    result = _fileService.GenerateExcelContent<ImportInventoryRecord>(sampleData, "ImportMaterialTemplate_Format_ddMMyyyy");

                    if (result != null)
                    {
                        scope.Complete();
                    }
                }
                catch (Exception ex)
                {
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
                                MaterialName = (worksheet.Cells[row, 2].Value == null) ? "" : worksheet.Cells[row, 2].Value.ToString(),
                                SupplierName = (worksheet.Cells[row, 3].Value == null) ? "" : worksheet.Cells[row, 3].Value.ToString(),
                                Quantity = (worksheet.Cells[row, 4].Value == null) ? 0 : int.Parse(worksheet.Cells[row, 4].Value.ToString())
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

        private async Task<bool> CheckHeader(IFormFile file, List<string> headerTemplate)
        {
            if (file == null || file.Length == 0)
            {
                return false;
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

                        int colCount = worksheet.Dimension.Columns;
                        if (colCount != headerTemplate.Count) return false;

                        for (int col = 1; col <= colCount; col++) // Assuming header is in the first row
                        {
                            if (!worksheet.Cells[1, col].Value.Equals(headerTemplate[col - 1]))
                                return false;
                        }
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, this);
            }
            return false;
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
                    if (res != null)
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