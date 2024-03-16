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
using OfficeOpenXml.Style;
using System.Text;
using System.Transactions;

namespace HCQS.BackEnd.Service.Implementations
{
    public class SupplierService : GenericBackendService, ISupplierService
    {
        private ISupplierRepository _supplierRepository;
        private IMapper _mapper;
        private BackEndLogger _logger;
        private IUnitOfWork _unitOfWork;
        private IFileService _fileService;

        public SupplierService(ISupplierRepository repository, IMapper mapper, BackEndLogger logger, IFileService fileService, IUnitOfWork unitOfWork, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _supplierRepository = repository;
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }

        public async Task<AppActionResult> CreateSupplier(SupplierRequest supplierRequest)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var supplierDb = await _supplierRepository.GetByExpression(n => n.SupplierName.Equals(supplierRequest.SupplierName));
                    if (supplierDb != null)
                    {
                        result = BuildAppActionResultError(result, $"The supplier whose name is {supplierRequest.SupplierName} has already existed!");
                    }
                    else
                    {
                        var supplier = _mapper.Map<Supplier>(supplierRequest);
                        supplier.Id = Guid.NewGuid();
                        result.Result.Data = await _supplierRepository.Insert(supplier);
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

        public async Task<AppActionResult> DeleteSupplierById(Guid id)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var supplierDb = await _supplierRepository.GetById(id);
                    if (supplierDb == null)
                    {
                        result = BuildAppActionResultError(result, $"The supplier with {id} not found !");
                    }
                    else
                    {
                        supplierDb.IsDeleted = true;
                        await _supplierRepository.Update(supplierDb);
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

        public async Task<AppActionResult> GetAll(int pageIndex, int pageSize, IList<SortInfo> sortInfos)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var supplierList = await _supplierRepository.GetAllDataByExpression(null, null);

                    if (supplierList.Any())
                    {
                        if (pageIndex <= 0) pageIndex = 1;
                        if (pageSize <= 0) pageSize = SD.MAX_RECORD_PER_PAGE;
                        int totalPage = DataPresentationHelper.CalculateTotalPageSize(supplierList.Count(), pageSize);

                        if (sortInfos != null)
                        {
                            supplierList = DataPresentationHelper.ApplySorting(supplierList, sortInfos);
                        }
                        if (pageIndex > 0 && pageSize > 0)
                        {
                            supplierList = DataPresentationHelper.ApplyPaging(supplierList, pageIndex, pageSize);
                        }
                        result.Result.Data = supplierList;
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
                    result = BuildAppActionResultError(result, ex.Message);
                    _logger.LogError(ex.Message, this);
                }
                return result;
            }
        }

        public async Task<AppActionResult> GetSupplierById(Guid id)
        {
            AppActionResult result = new AppActionResult();
            try
            {
                var supplierDb = await _supplierRepository.GetById(id);
                if (supplierDb != null)
                {
                    result.Result.Data = supplierDb;
                }
            }
            catch (Exception ex)
            {
                result = BuildAppActionResultError(result, ex.Message);
                _logger.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<AppActionResult> GetSupplierByName(string name)
        {
            AppActionResult result = new AppActionResult();
            try
            {
                var supplierDb = await _supplierRepository.GetByExpression(s => s.SupplierName.ToLower().Equals(name.ToLower()));
                if (supplierDb != null)
                {
                    result.Result.Data = supplierDb;
                }
            }
            catch (Exception ex)
            {
                result = BuildAppActionResultError(result, ex.Message);
                _logger.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<IActionResult> ImportSupplierWithExcelFile(IFormFile file)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                IActionResult result = new ObjectResult(null) { StatusCode = 200 };
                string message = "";
                byte[] excelBytes = null;

                try
                {
                    using (var stream = new MemoryStream())
                    {
                        file.CopyTo(stream);
                        using (var package = new ExcelPackage(stream))
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                            int rowCount = worksheet.Dimension.Rows;

                            List<SupplierRequest> data = new List<SupplierRequest>();
                            HashSet<string> inputtedSupplierName = new HashSet<string>();
                            if (!(await CheckHeader(file, SD.ExcelHeaders.SUPPLIER)))
                            {
                                _logger.LogError($"Incompatible header to supplier template", this);
                                message = $"Incompatible header to supplier template";
                            } else if(rowCount < 2)
                            {
                                _logger.LogError($"Empty record list!", this);
                                message = $"Empty record list!";
                            }
                            else
                            {
                                bool containsError = false;
                                bool containDuplicated = false;

                                StringBuilder rowError = new StringBuilder();
                                int i = 1;
                                for (int row = 2; row <= rowCount; row++)
                                {
                                    i = 1;
                                    rowError = new StringBuilder();
                                    string name = worksheet.Cells[row, 2].Text;
                                    string type = worksheet.Cells[row, 3].Text;
                                    if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(type))
                                    {
                                        bool containsUnit = SD.EnumType.SupplierType.TryGetValue(type, out int index);
                                        if (containsUnit)
                                        {
                                            var duplicatedSupplier = await _supplierRepository.GetByExpression(s => s.SupplierName.ToLower().Equals(name.ToLower()));
                                            if (duplicatedSupplier != null)
                                            {
                                                if (!containDuplicated) containDuplicated = true;
                                                containsError = true;
                                                rowError.Append($"{i++}. Duplicated Supplier name in database.\n");
                                            }

                                            if (!inputtedSupplierName.Add(name.ToLower()))
                                            {
                                                if (!containDuplicated) containDuplicated = true;
                                                containsError = true;
                                                rowError.Append($"{i++}. Duplicated Supplier name in current excel.\n");
                                            }

                                            if (!containDuplicated)
                                            {
                                                bool validType = Enum.TryParse(type, out SupplierRequest.SupplierType supplierType);
                                                if (validType)
                                                {
                                                    data.Add(new SupplierRequest { Id = Guid.NewGuid(), SupplierName = name, Type = supplierType });
                                                }
                                                else
                                                {
                                                    containsError = true;
                                                    rowError.Append($"{i++}. Supplier type: {type} does not exist.\n");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            containsError = true;
                                            rowError.Append($"{i++}. Supplier type: {type} does not exist.\n");
                                        }
                                    }
                                    else
                                    {
                                        containsError = true;
                                        rowError.Append($"{i++}. Empty field!");
                                    }
                                    if (i > 1)
                                    {
                                        worksheet.Cells[row, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        worksheet.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Red);
                                        worksheet.Cells[row, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        worksheet.Cells[row, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Red);
                                        worksheet.Cells[row, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        worksheet.Cells[row, 3].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Red);
                                        rowError.Append("(Please delete this error message cell before re-uploading!)");
                                        worksheet.Cells[row, 4].Value = rowError.ToString();
                                        worksheet.Cells[row, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        worksheet.Cells[row, 4].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Red);
                                    }
                                }

                                if (containsError)
                                {
                                    using (var modifiedStream = new MemoryStream())
                                    {
                                        package.SaveAs(modifiedStream);
                                        package.Save();

                                        var modifiedFile = new FormFile(modifiedStream, 0, modifiedStream.Length, null, file.FileName)
                                        {
                                            Headers = file.Headers
                                        };

                                        excelBytes = modifiedStream.ToArray();
                                    }
                                }
                                else
                                {
                                    await _supplierRepository.InsertRange(_mapper.Map<List<Supplier>>(data));
                                    result = new ObjectResult(data)
                                    { StatusCode = 200 };
                                    await _unitOfWork.SaveChangeAsync();
                                }

                                if (containsError)
                                {
                                    result = new FileContentResult(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                                    {
                                        FileDownloadName = $"(ErrorColored){file.Name}"
                                    };
                                }
                                scope.Complete();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, this);

                    using (var originalStream = new MemoryStream())
                    {
                        file.CopyTo(originalStream);
                        excelBytes = originalStream.ToArray();
                    }
                }

                if (!string.IsNullOrEmpty(message)) return new ObjectResult(message) { StatusCode = 200 };
                return result;
            }
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

        public async Task<AppActionResult> UpdateSupplier(SupplierRequest supplierRequest)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var supplierDb = await _supplierRepository.GetByExpression(n => n.Id.Equals(supplierRequest.Id));
                    if (supplierDb == null)
                    {
                        result = BuildAppActionResultError(result, $"The supplier with {supplierRequest.Id} not found !");
                    }
                    else
                    {
                        var supplier = _mapper.Map<Supplier>(supplierRequest);
                        supplierDb.SupplierName = supplier.SupplierName;
                        supplierDb.Type = supplier.Type;
                        result.Result.Data = await _supplierRepository.Update(supplierDb);
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

        public async Task<IActionResult> GetSupplierTemplate()
        {
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    IActionResult result = null;
                    try
                    {
                        List<SupplierRecord> sampleData = new List<SupplierRecord>();
                        sampleData.Add(new SupplierRecord
                        { SupplierName = "Supplier1", Type = "Both" });
                        result = _fileService.GenerateExcelContent<SupplierRecord>(sampleData, "SupplierTemplate");
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
        }
    }
}