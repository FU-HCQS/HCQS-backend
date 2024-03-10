using AutoMapper;
using FirebaseAdmin.Messaging;
using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.BaseRequest;
using HCQS.BackEnd.Common.Dto.Record;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.Common.Dto.Response;
using HCQS.BackEnd.Common.Util;
using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Models;
using HCQS.BackEnd.Service.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
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
                        result.Result.Data = supplierPriceDetailDb.OrderByDescending(i => i.Date);
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
                        result.Result.Data = supplierPriceDetailDb.OrderByDescending(i => i.Date); ;
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
                        result.Result.Data = supplierPriceDetailDb.OrderByDescending(i => i.Date);
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
                        string datestring = file.FileName;
                        if (datestring.Contains('_'))
                        {
                            return new ObjectResult("Invalid file name. Please follow format: ddMMyyyy") { StatusCode = 200 };
                        }
                        if (file.FileName.Contains("(ErrorColor)"))
                            datestring = datestring.Substring("(ErrorColor)".Length);
                        if (datestring.Length < 8)
                        {
                            return new ObjectResult("Invalid date. Please follow date format: ddMMyyyy") { StatusCode = 200 };
                        }
                        datestring = datestring.Substring(0, 8);
                        if (!DateTime.TryParseExact(datestring, "ddMMyyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                        {
                            isSuccessful = false;
                            _logger.LogError($"{datestring} is not in format: ddMMyyyy", this);
                            message = $"{datestring} is not in format: ddMMyyyy";
                        }
                        else
                        {
                            string errorHeader = await CheckHeader(file, SD.ExcelHeaders.IMPORT_INVENTORY);
                            if (!string.IsNullOrEmpty(errorHeader))
                            {
                                isSuccessful = false;
                                _logger.LogError(errorHeader, this);
                                message = errorHeader;
                            }
                            else
                            {
                                Dictionary<string, Guid> materials = new Dictionary<string, Guid>();
                                Dictionary<Guid, int> materialImport = new Dictionary<Guid, int>();
                                Dictionary<string, Guid> suppliers = new Dictionary<string, Guid>();
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
                                    List<List<string>> recordDatastring = new List<List<string>>();
                                    int j = 1;
                                    foreach (var record in records)
                                    {
                                        recordDatastring.Add(new List<string>
                                        {
                                            j++.ToString(), record.MaterialName, record.SupplierName, record.Quantity.ToString()
                                        });
                                    }
                                    result = _fileService.ReturnErrorColored<ImportInventoryRecord>(SD.ExcelHeaders.IMPORT_INVENTORY, recordDatastring, invalidRowInput, datestring);
                                    isSuccessful = false;
                                }

                                if (isSuccessful)
                                {
                                    await _importExportInventoryHistoryRepository.InsertRange(importInventoryList);
                                    foreach (var materialId in materialImport.Keys)
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
                                No = (worksheet.Cells[row, 1].Value == null) ? 0 : int.Parse(worksheet.Cells[row, 1].Value.ToString()),
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

        private async Task<string> CheckHeader(IFormFile file, List<string> headerTemplate)
        {
            if (file == null || file.Length == 0)
            {
                return "File not found";
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

                        int colCount = worksheet.Columns.Count();
                        if(colCount != headerTemplate.Count && worksheet.Cells[1, colCount].Value != null)
                        {
                            return "Difference in column names";
                        }
                        StringBuilder sb = new StringBuilder();
                        sb.Append("Incorrect column names: ");
                        bool containsError = false;
                        for (int col = 1; col <= Math.Min(4, worksheet.Columns.Count()); col++) // Assuming header is in the first row
                        {
                            if (!worksheet.Cells[1, col].Value.Equals(headerTemplate[col - 1]))
                            {
                                if (!containsError) containsError = true;
                                sb.Append($"{worksheet.Cells[1, col].Value}(Correct: {headerTemplate[col - 1]}), ");
                            }
                        }
                        if (containsError)
                        {
                            return sb.Remove(sb.Length - 2, 2).ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, this);
            }
            return string.Empty;
        }

        private async Task<Guid> GetSupplierQuationDetail(Guid materialId, Guid supplierId, int quantity)
        {
            try
            {
                var supplierPriceDetailRepository = Resolve<ISupplierPriceDetailRepository>();
                var utility = Resolve<Utility>();
                DateTime currentDate = utility.GetCurrentDateTimeInTimeZone();
                var supplierPriceDetails = await supplierPriceDetailRepository
                    .GetAllDataByExpression(s => s.MaterialId == materialId && s.SupplierPriceQuotation.SupplierId == supplierId && s.SupplierPriceQuotation.Date <= currentDate, s => s.SupplierPriceQuotation);
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

        public async Task<AppActionResult> ValidateExcel(IFormFile file)
        {
            AppActionResult result = new AppActionResult();
            ExcelValidatingResponse data = new ExcelValidatingResponse();
            if (file == null || file.Length == 0)
            {
                data.IsValidated = false;
                data.HeaderError = "Unable to validate excel file. Please restart!";
                result.Result.Data = data;
                return result;
            }
            try
            {
                //Format: Name_ddmmyyy
                //Format: ddMMyyyy
                string nameDatestring = file.FileName;
                if (file.FileName.Contains("(ErrorColor)"))
                    nameDatestring = nameDatestring.Substring("(ErrorColor)".Length);
                if (nameDatestring.Contains('_') || nameDatestring.Length < 8)
                {
                    data.IsValidated = false;
                    data.HeaderError = "Invalid file name. Please follow format: ddMMyyyy";
                    result.Result.Data = data;
                    return result;
                }
                nameDatestring = nameDatestring.Substring(0, 8);
                if (!DateTime.TryParseExact(nameDatestring, "ddMMyyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                {                  
                    data.IsValidated = false;
                    data.HeaderError = $"{nameDatestring} is not in format: ddMMyyyy";
                    result.Result.Data = data;
                    return result;
                }

                string errorHeader = await CheckHeader(file, SD.ExcelHeaders.IMPORT_INVENTORY);
                if (!string.IsNullOrEmpty(errorHeader))
                {
                    data.IsValidated = false;
                    data.HeaderError = errorHeader;
                    result.Result.Data = data;
                    return result;
                }


                Dictionary<string, Guid> materials = new Dictionary<string, Guid>();
                Dictionary<Guid, int> materialImport = new Dictionary<Guid, int>();
                Dictionary<string, int> duplicated = new Dictionary<string, int>();
                Dictionary<string, Guid> suppliers = new Dictionary<string, Guid>();
                List<ImportInventoryRecord> records = await GetImportListFromExcel(file);
                var materialRepository = Resolve<IMaterialRepository>();
                var supplierRepository = Resolve<ISupplierRepository>();
                int invalidRowInput = 0;
                int errorRecordCount = 0;
                int i = 2;
                data.Errors = new string[records.Count];
                foreach (ImportInventoryRecord record in records)
                {
                    StringBuilder error = new StringBuilder();
                    errorRecordCount = 0;
                    Guid materialId = Guid.Empty;
                    Guid supplierId = Guid.Empty;
                    if (record.No != i - 1)
                    {
                        error.Append($"{errorRecordCount + 1}. No should be {i - 1}.\n");
                        errorRecordCount++;
                    }
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

                    string duplicatedKey = $"{record.MaterialName}-{record.SupplierName}";
                    if (duplicated.ContainsKey(duplicatedKey))
                    {
                        error.Append($"{errorRecordCount + 1}. Duplicated material from supplier with row {duplicated[duplicatedKey]}.\n");
                        errorRecordCount++;
                    }
                    else
                    {
                        duplicated.Add(duplicatedKey, i - 1);
                    }

                    if (errorRecordCount > 0)
                    {
                    
                        data.Errors[i - 2] = error.ToString();
                        invalidRowInput++;
                    }
                    i++;
                }

                if (invalidRowInput > 0)
                {
                    data.IsValidated = false;
                    result.Result.Data = data;
                    return result;
                }

                data.IsValidated = true;
                data.Errors = null;
                data.HeaderError = null;
                result.Result.Data = data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, this);
                data.IsValidated = false;
            }
            return result;
        }
    }
}