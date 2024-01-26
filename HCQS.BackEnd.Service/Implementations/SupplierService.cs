using AutoMapper;
using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.BaseRequest;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Models;
using HCQS.BackEnd.DAL.Util;
using HCQS.BackEnd.Service.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Transactions;

namespace HCQS.BackEnd.Service.Implementations
{
    public class SupplierService : GenericBackendService, ISupplierService
    {
        private ISupplierRepository _supplierRepository;
        private IMapper _mapper;
        private BackEndLogger _logger;
        private IUnitOfWork _unitOfWork;

        public SupplierService(ISupplierRepository repository, IMapper mapper, BackEndLogger logger, IUnitOfWork unitOfWork, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _supplierRepository = repository;
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
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
                        result.Result.Data = await _supplierRepository.DeleteById(id);
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

        public async Task<IActionResult> ImportSupplierWithExcelFile(IFormFile file)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                byte[] excelBytes = null; // Initialize a variable to store the file content

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
                            bool containDuplicated = false;

                            for (int row = 2; row <= rowCount; row++)
                            {
                                string name = worksheet.Cells[row, 1].Text;
                                string type = worksheet.Cells[row, 2].Text;

                                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(type))
                                {
                                    var type2 = int.Parse(type);
                                    if ((SupplierRequest.SupplierType)type2 > SupplierRequest.SupplierType.Both || (SupplierRequest.SupplierType)type2 > SupplierRequest.SupplierType.ConstructionMaterialsSupplier)
                                    {
                                        throw new Exception("ngu ngok");
                                    }
                                    var duplicatedSupplier = await _supplierRepository.GetByExpression(s => s.SupplierName.ToLower().Equals(name.ToLower()));
                                    if (duplicatedSupplier != null)
                                    {
                                        if (!containDuplicated) containDuplicated = true;

                                        worksheet.Cells[row, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        worksheet.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Red);
                                        worksheet.Cells[row, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        worksheet.Cells[row, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Red);
                                    }

                                    if (!containDuplicated)
                                    {
                                        data.Add(new SupplierRequest { Id = Guid.NewGuid(), SupplierName = name, Type = (SupplierRequest.SupplierType)int.Parse(type) });
                                    }
                                }
                            }

                            if (containDuplicated)
                            {
                                using (var modifiedStream = new MemoryStream())
                                {
                                    package.SaveAs(modifiedStream);
                                    package.Save();

                                    // Create a new IFormFile instance with the modified content
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
                                await _unitOfWork.SaveChangeAsync();
                            }

                            scope.Complete();
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
                if (excelBytes != null)
                {
                    return new FileContentResult(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = "template.xlsx"
                    };
                }
                return new OkObjectResult(new AppActionResult { IsSuccess = true, Result = null, Messages = null });
            }
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
    }
}