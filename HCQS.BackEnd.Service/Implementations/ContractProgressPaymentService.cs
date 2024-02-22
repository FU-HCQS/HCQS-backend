using AutoMapper;
using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.Common.Util;
using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Models;
using HCQS.BackEnd.Service.Contracts;
using NPOI.SS.Formula.Functions;
using System.Transactions;

namespace HCQS.BackEnd.Service.Implementations
{
    public class ContractProgressPaymentService : GenericBackendService, IContractProgressPaymentService
    {
        private BackEndLogger _logger;
        private IUnitOfWork _unitOfWork;
        private IContractProgressPaymentRepository _repository;
        private IMapper _mapper;

        public ContractProgressPaymentService(BackEndLogger logger, IMapper mapper, IUnitOfWork unitOfWork, IContractProgressPaymentRepository repository, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<AppActionResult> CreateContractProgressPayment(List<ContractProgressPaymentDto> list)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                var utility = Resolve<Utility>();
                var paymentRepository = Resolve<IPaymentRepository>();
                var contractRepository = Resolve<IContractRepository>();
                var accountRepository = Resolve<IAccountRepository>();
                var fileService = Resolve<IFileService>();
                var emailService = Resolve<IEmailService>();
                string code = Guid.NewGuid().ToString("N").Substring(0, 6);

                try
                {
                    List<ContractProgressPayment> listCPP = new List<ContractProgressPayment>();
                    List<Payment> listPM = new List<Payment>();
                    var contractId = list?.First()?.ContractId;
                    var contractDb = await contractRepository.GetByExpression(a => a.Id == contractId, a => a.Project.Account);
                    if (contractDb == null)
                    {
                        result = BuildAppActionResultError(result, $"The contract with id {list.First().ContractId} is existed");
                    }
                    var deposit = list.Where(a => a.Content.ToLower().Equals("Deposit".ToLower()) || a.Name.ToLower().Equals("Deposit".ToLower())).SingleOrDefault();
                    if (deposit == null)
                    {
                        result = BuildAppActionResultError(result, $"The list contract progress payment must have deposit");

                    }
                   
                    double total = 0;
                    foreach (var item in list)
                    {
                        var contractProgressPaymentDb = await _repository.GetByExpression(a => a.Name == item.Name && a.ContractId == item.ContractId);
                        if (contractProgressPaymentDb != null)
                        {
                            result = BuildAppActionResultError(result, $"The contract progress payment with name {item.Name} and contractId {item.ContractId} is existed");
                        }
                        total = (double)(total + item.Price);
                    }
                    if (contractDb.Total != total)
                    {

                        result = BuildAppActionResultError(result, $"The total price for all progress contract payment don't match total in contract");

                    }
                    var account = await accountRepository.GetByExpression(c => c.Id == contractDb.Project.AccountId);

                    if (!BuildAppActionResultIsError(result))
                    {
                        foreach (var item in list)
                        {
                            var paymentId = Guid.NewGuid();
                            ContractProgressPayment cpm = new ContractProgressPayment
                            {
                                Id = Guid.NewGuid(),
                                ContractId = (Guid)item.ContractId,
                                PaymentId = paymentId,
                                Date = utility.GetCurrentDateTimeInTimeZone(),
                                Name = item.Name
                            };
                            listCPP.Add(cpm);
                            listPM.Add(new Payment
                            {
                                Content = item.Content,
                                Price = (double)item.Price,
                                Id = paymentId,
                                PaymentStatus = Payment.Status.Pending,
                                ContractProgressPayment = cpm
                            });
                        }
                    }

                    if (!BuildAppActionResultIsError(result))
                    {
                        var a = await _repository.InsertRange(listCPP);
                        await paymentRepository.InsertRange(listPM);
                        await contractRepository.Update(contractDb);
                        await _unitOfWork.SaveChangeAsync();
                        var content = TemplateMappingHelper.GetTemplateContract(contractDb.DateOfContract, utility.GetCurrentDateTimeInTimeZone(), contractDb.Project.Account, a.ToList(), false);
                        contractDb.Content = content;
                        contractDb.ContractStatus = Contract.Status.IN_ACTIVE;
                        contractDb.Deposit = (double)deposit.Price;
                        var upload = await fileService.UploadImageToFirebase(fileService.ConvertHtmlToPdf(content, $"{contractDb.Id}.pdf"), $"contract/{contractDb.Id}");
                        contractDb.ContractUrl = Convert.ToString(upload.Result.Data);
                        await _unitOfWork.SaveChangeAsync();
                        emailService.SendEmail(account.Email, SD.SubjectMail.SIGN_CONTRACT_VERIFICATION_CODE, code);

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

        public async Task<AppActionResult> DeleteContractProgressPaymentByContractId(Guid id)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                var paymentRepository = Resolve<IPaymentRepository>();
                try
                {
                    var contractProgressPaymentDb = await _repository.GetAllDataByExpression(a => a.ContractId == id && a.Name != "Deposit");
                    if (!contractProgressPaymentDb.Any())
                    {
                        result = BuildAppActionResultError(result, $"The contract progress payment with contract id {id} is not existed!");
                    }
                    if (!BuildAppActionResultIsError(result))
                    {
                        foreach (var contract in contractProgressPaymentDb)
                        {
                            await _repository.DeleteById(id);

                        }
                    }
                    if (!BuildAppActionResultIsError(result))
                    {
                        await _unitOfWork.SaveChangeAsync();
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

        public async Task<AppActionResult> GetContractProgressPaymentByContractId(Guid contractId)
        {
            AppActionResult result = new AppActionResult();
            try
            {
                result.Result.Data = await _repository.GetAllDataByExpression(c => c.ContractId == contractId, c => c.Payment);
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