using AutoMapper;
using HCQS.BackEnd.Common.Dto.BaseRequest;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Models;
using HCQS.BackEnd.DAL.Util;
using HCQS.BackEnd.Service.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using HCQS.BackEnd.DAL.Implementations;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.DataValidation;

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
                        await _supplierRepository.Insert(supplier);
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
                        await _supplierRepository.DeleteById(id);
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

        public async Task<AppActionResult> GetAll(int pageIndex, int pageSize, IList<SortInfo> sortInfos)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var supplierList = await _supplierRepository.GetAll();

                    var supplier = Utility.ConvertIOrderQueryAbleToList(supplierList);

                    supplierList = Utility.ConvertListToIOrderedQueryable(supplier);

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
                    result = BuildAppActionResultError(result, SD.ResponseMessage.INTERNAL_SERVER_ERROR, true);
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
                result = BuildAppActionResultError(result, SD.ResponseMessage.INTERNAL_SERVER_ERROR, true);
                _logger.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<AppActionResult> ImportSupplierWithExcelFile(IFormFile file)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled)) {
                AppActionResult result = new AppActionResult();
                try
                {
                    using (var stream = new MemoryStream())
                    {
                        file.CopyTo(stream);
                        using (var package = new ExcelPackage(stream))
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                            int rowCount = worksheet.Dimension.Rows;

                            List<string> data = new List<string>();
                            bool containDuplicated = false;
                            for (int row = 2; row <= rowCount; row++)
                            {
                                    string cellValue = worksheet.Cells[row, 1].Text;
                                    if(!string.IsNullOrEmpty(cellValue))
                                    {
                                        var duplicatedSupplier = _supplierRepository.GetByExpression(s => s.SupplierName.ToLower().Equals(cellValue.ToLower()));
                                        if(duplicatedSupplier != null)
                                        {

                                            if(!containDuplicated) containDuplicated = true;

                                            worksheet.Cells[row, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                            worksheet.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Red);
                                        }
                                        if (!containDuplicated)
                                        {
                                            data.Add(cellValue);
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

                                    result.Result.Data = modifiedFile;
                                    result.Messages.Add("The inputted file contains already exist suppplier name(s)");
                                }
                            }
                            else
                            {
                                List<SupplierRequest> supplierRequests = new List<SupplierRequest>();
                                foreach(var name in data) supplierRequests.Add(new SupplierRequest() { SupplierName = name });
                                await _supplierRepository.InsertRange(_mapper.Map<List<Supplier>>(supplierRequests));
                                result.IsSuccess = true;
                            }

                            if (!BuildAppActionResultIsError(result))
                            {
                                scope.Complete();
                            }
                        }
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
                    result = BuildAppActionResultError(result, SD.ResponseMessage.INTERNAL_SERVER_ERROR, true);
                    _logger.LogError(ex.Message, this);
                }
                return result;
            }

        }
    }

}
