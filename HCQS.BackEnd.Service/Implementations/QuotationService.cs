using AutoMapper;
using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Implementations;
using HCQS.BackEnd.DAL.Models;
using HCQS.BackEnd.DAL.Util;
using HCQS.BackEnd.Service.Contracts;
using System.Transactions;

namespace HCQS.BackEnd.Service.Implementations
{
    public class QuotationService : GenericBackendService, IQuotationService
    {
        private IQuotationRepository _quotationRepository;
        private BackEndLogger _logger;
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public QuotationService(IQuotationRepository quotationRepository, BackEndLogger logger, IUnitOfWork unitOfWork, IMapper mapper, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _quotationRepository = quotationRepository;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<AppActionResult> CreateQuotationDealingByStaff(CreateQuotationDeallingStaffRequest request)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var utility = Resolve<Utility>();
                    var exportPriceMaterialRepository = Resolve<IExportPriceMaterialRepository>();
                    foreach (var item in request.ExportPriceMaterialDtos)
                    {
                        item.Id = Guid.NewGuid();
                    }
                    var quotationDetailRepository = Resolve<IQuotationDetailRepository>();
                    var quotationDb = await _quotationRepository.GetById(request.QuotationId);
                    var quotationDetailsDb = await quotationDetailRepository.GetAllDataByExpression(filter: a => a.QuotationId == request.QuotationId);
                    if (quotationDetailsDb == null)
                    {
                        result = BuildAppActionResultError(result, $"The quotation with id {request.QuotationId} is empty");
                    }

                    if (!quotationDetailsDb.Any() && quotationDetailsDb != null)
                    {
                        result = BuildAppActionResultError(result, $"The quotation details with quotation id {request.QuotationId} is empty");
                    }
                    if (!BuildAppActionResultIsError(result))
                    {
                        double total = 0;
                        List<ExportPriceMaterial> exportPriceMaterials = _mapper.Map<List<ExportPriceMaterial>>(request.ExportPriceMaterialDtos);
                        foreach (var item in exportPriceMaterials)
                        {
                            item.Date = utility.GetCurrentDateTimeInTimeZone();
                            item.Id = Guid.NewGuid();
                        }

                        Quotation quotation = new Quotation
                        {
                            Id = Guid.NewGuid(),
                            FurnitureDiscount = request.FurnitureDiscount,
                            LaborDiscount = request.LaborDiscount,
                            MaterialDiscount = request.MaterialDiscount,
                            ProjectId = quotationDb.ProjectId
                        };
                        List<QuotationDetail> quotationDetails = new List<QuotationDetail>();
                        foreach (var quotationDetail in quotationDetailsDb)
                        {
                            var exportPriceMaterial = exportPriceMaterials.SingleOrDefault(s => s.MaterialId == quotationDetail.MaterialId);
                            if (exportPriceMaterial != null)
                            {
                                if (quotationDetail.MaterialId == exportPriceMaterial.MaterialId)
                                {
                                    total = total + (exportPriceMaterial.Price * quotationDetail.Quantity);
                                    quotationDetails.Add(new QuotationDetail { Id = Guid.NewGuid(), MaterialId = quotationDetail.MaterialId, Quantity = quotationDetail.Quantity, Total = quotationDetail.Quantity * exportPriceMaterial.Price, QuotationId = quotation.Id });
                                }
                            }
                        }
                        await _quotationRepository.Insert(quotation);
                        await exportPriceMaterialRepository.InsertRange(exportPriceMaterials);
                        quotation.RawMaterialPrice = total;
                        quotation.QuotationStatus = Quotation.Status.Pending;
                        quotationDb.QuotationStatus = Quotation.Status.Cancel;
                        await quotationDetailRepository.InsertRange(quotationDetails);
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

        public async Task<AppActionResult> CreateQuotationDealingRequest(QuotationDealingDto quotationDealingDto)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var quotationMap = _mapper.Map<QuotationDealing>(quotationDealingDto);
                    quotationMap.Id = Guid.NewGuid();
                    var quotationDb = await _quotationRepository.GetById(quotationDealingDto.QuotationId);
                    if (quotationDb == null)
                    {
                        result = BuildAppActionResultError(result, $"The quotation with id {quotationDealingDto.Id} is not existed");
                    }
                    if (!BuildAppActionResultIsError(result))
                    {
                        var quotationDealingRepository = Resolve<IQuotationDealingRepository>();
                        result.Result.Data = await quotationDealingRepository.Insert(quotationMap);
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

        public async Task<AppActionResult> DealQuotation(Guid quotationId, bool status)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var contractRepository = Resolve<IContractRepository>();
                    var quotationDetailRepository = Resolve<IQuotationDetailRepository>();
                    var paymentRepository = Resolve<IPaymentRepository>();
                    var contractProgressPaymentRepository = Resolve<IContractProgressPaymentRepository>();
                    var projectRepository = Resolve<IProjectRepository>();

                    var accountRepository = Resolve<IAccountRepository>();
                    var quotationDb = await _quotationRepository.GetById(quotationId);
                    var emailService = Resolve<IEmailService>();
                    string code = Guid.NewGuid().ToString("N").Substring(0, 6);
                    var project = await projectRepository.GetById(quotationDb.ProjectId);
                    var account = await accountRepository.GetById(project.AccountId);
                    if (quotationDb == null)
                    {
                        result = BuildAppActionResultError(result, $"The quotation with id {quotationId} is not existed");
                    }
                    if (!BuildAppActionResultIsError(result))
                    {
                       
                        var quotationDetails = await quotationDetailRepository.GetAllDataByExpression(filter: a => a.QuotationId == quotationId);

                        if (!quotationDetails.Any())
                        {
                            result = BuildAppActionResultError(result, $"The list quotation details with id {quotationId} is not existed");
                        }
                        var utility = Resolve<Utility>();
                        double total = 0;
                        foreach (var quotationDetail in quotationDetails)
                        {
                            total = total + quotationDetail.Total;
                        }

                        if (status == true)
                        {
                            quotationDb.QuotationStatus = Quotation.Status.Approved;
                            var contract = new Contract
                            {
                                Id = Guid.NewGuid(),
                                ProjectId = quotationDb.ProjectId,
                                DateOfContract = utility.GetCurrentDateTimeInTimeZone(),
                                Deposit = (30 * total) / 100,
                                EndDate = utility.GetCurrentDateInTimeZone().AddYears(5),
                                MaterialPrice = total,
                                Content = string.Empty,
                                StartDate = utility.GetCurrentDateTimeInTimeZone(),
                                LaborPrice = quotationDb.LaborPrice,
                                FurniturePrice = quotationDb.FurniturePrice,
                                ContractStatus = Contract.Status.IN_ACTIVE
                            };

                            var paymentId = Guid.NewGuid();
                            ContractProgressPayment contractProgressPayment = new ContractProgressPayment
                            {
                                Id = Guid.NewGuid(),
                                ContractId = contract.Id,
                                Date = utility.GetCurrentDateTimeInTimeZone().AddDays(45),
                                Name = "Deposit",
                                PaymentId = paymentId
                            };
                            Payment payment = new Payment
                            {
                                Id = paymentId,
                                Content = "Deposit",
                                PaymentStatus = Payment.Status.Pending,
                                ContractProgressPayment = contractProgressPayment,
                                Price = (30 * total) / 100
                            };

                            result.Result.Data = await contractRepository.Insert(contract);
                            await contractProgressPaymentRepository.Insert(contractProgressPayment);
                            await paymentRepository.Insert(payment);
                      
                            if (account.ContractVerifyCode == null)
                            {
                                account.ContractVerifyCode = code;
                                await accountRepository.Update(account);
                            }
                            else
                            {
                                result = BuildAppActionResultError(result, "The code to sign the contract has been sent via email!");
                            }

                            await _unitOfWork.SaveChangeAsync();
                        }
                        else
                        {
                            quotationDb.QuotationStatus = Quotation.Status.Cancel;
                        }
                        result.Result.Data = await _quotationRepository.Update(quotationDb);
                        await _unitOfWork.SaveChangeAsync();
                    }
                    if (!BuildAppActionResultIsError(result))
                    {
                        scope.Complete();
                        emailService.SendEmail(account.Email, SD.SubjectMail.SIGN_CONTRACT_VERIFICATION_CODE, code);
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

        public async Task<AppActionResult> GetAllQuotationByProjectId(Guid projectId)
        {
            AppActionResult result = new AppActionResult();

            try
            {
                result.Result.Data = await _quotationRepository.GetAllDataByExpression(filter: a => a.ProjectId == projectId);
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