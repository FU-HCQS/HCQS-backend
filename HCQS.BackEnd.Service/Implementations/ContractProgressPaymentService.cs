using AutoMapper;
using Firebase.Auth;
using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.Common.Util;
using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Implementations;
using HCQS.BackEnd.DAL.Models;
using HCQS.BackEnd.Service.Contracts;
using NPOI.POIFS.Properties;
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
                var contractVerificationCodeRepository = Resolve<IContractVerificationCodeRepository>();
                var fileService = Resolve<IFileService>();
                var emailService = Resolve<IEmailService>();
                string code = Guid.NewGuid().ToString("N").Substring(0, 6);

                try
                {
                    List<ContractProgressPayment> listCPP = new List<ContractProgressPayment>();
                    List<Payment> listPM = new List<Payment>();
                    var contractId = list?.First()?.ContractId;
                    var contractDb = await contractRepository.GetByExpression(a => a.Id == contractId, a => a.Project.Account, a => a.Project);
                    if (contractDb == null)
                    {
                        result = BuildAppActionResultError(result, $"The contract with id {list.First().ContractId} is not existed");
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
                        if(contractDb.EndDate < item.EndDate)
                        {
                            result = BuildAppActionResultError(result, $"The payment date of the contract installments must be less than the contract end date");
                        }
                        if(item.EndDate < utility.GetCurrentDateTimeInTimeZone())
                        {
                            result = BuildAppActionResultError(result, $"Dates of payment installments are not allowed to be dates in the past");

                        }
                        total = (double)(total + item.Price);
                    }

                    if (contractDb != null && contractDb.Total != total)
                    {
                        result = BuildAppActionResultError(result, $"The total price for all progress contract payment don't match total in contract");
                    }
                    else
                    {
                        var contractVerificationCode = await contractVerificationCodeRepository.GetByExpression(c => c.ContractId == contractDb.Id);
                        if (contractVerificationCode != null)
                        {
                            result = BuildAppActionResultError(result, $"The verification code is existed");
                        }
                    }

                    var account = await accountRepository.GetByExpression(c => c.Id == contractDb.Project.AccountId);
                    if (account == null)
                    {
                        result = BuildAppActionResultError(result, $"The account ís not existed");
                    }
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
                                Date = (DateTime)item.EndDate,
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
                        ContractTemplateDto templateDto = new ContractTemplateDto
                        {
                            Account = account,
                            Contract = contractDb,
                            ContractProgressPayments = a.ToList(),
                            CreateDate = contractDb.DateOfContract,
                            IsSigned = false,
                            Project = contractDb.Project,
                            SignDate = utility.GetCurrentDateTimeInTimeZone()
                        };
                        var content = TemplateMappingHelper.GetTemplateContract(templateDto);
                        contractDb.ContractStatus = Contract.Status.IN_ACTIVE;
                        contractDb.Deposit = (double)deposit.Price;
                        var upload = await fileService.UploadFileToFirebase(fileService.ConvertHtmlToPdf(content, $"{contractDb.Id}.pdf"), $"contract/{contractDb.Id}");
                        contractDb.ContractUrl = Convert.ToString(upload.Result.Data);
                        contractDb.Content = content;
                        await contractVerificationCodeRepository.Insert(
                                new ContractVerificationCode
                                {
                                    Id = Guid.NewGuid(),
                                    ContractId = contractDb.Id,
                                    VerficationCode = code
                                }
                            );
                        await _unitOfWork.SaveChangeAsync();
                        emailService.SendEmail(account.Email, SD.SubjectMail.SIGN_CONTRACT_VERIFICATION_CODE, TemplateMappingHelper.GetTemplateOTPEmail(TemplateMappingHelper.ContentEmailType.CONTRACT_CODE, code, account.FirstName));

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
                    var contractProgressPaymentDb = await _repository.GetAllDataByExpression(a => a.ContractId == id);
                    if (!contractProgressPaymentDb.Any())
                    {
                        result = BuildAppActionResultError(result, $"The contract progress payment with contract id {id} is not existed!");
                    }
                    if (!BuildAppActionResultIsError(result))
                    {
                        foreach (var contract in contractProgressPaymentDb)
                        {
                            await _repository.DeleteById(contract.Id);
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

        public async Task SendPaymentRemindEmail()
        {
            try
            {
                var utility = Resolve<Utility>();
                var emailService = Resolve<IEmailService>();
                var currentDate = utility.GetCurrentDateTimeInTimeZone().Date;

                // Retrieve contract progress payments with related entities
                var contractProgressPayments = await _repository.GetAllDataByExpression(
                    cp => cp.Date < currentDate,
                    cp => cp.Contract.Project.Account,
                    cp => cp.Payment
                );

                var filteredPayments = contractProgressPayments
                    .Where(p => p.Payment.PaymentStatus == Payment.Status.Pending && p.Contract.ContractStatus == Contract.Status.ACTIVE)
                    .ToList();

                var groupedPayments = filteredPayments
                    .GroupBy(cp => cp.ContractId);

                foreach (var group in groupedPayments)
                {
                    var contractId = group.Key;
                    var paymentsForContract = group.ToList(); 

                    var account = paymentsForContract.FirstOrDefault()?.Contract.Project.Account;

                    if (account != null)
                    {
                        emailService.SendEmail(
                            account.Email,
                            "Love House: Payment Reminder",
                            TemplateMappingHelper.GetTemplatePaymentReminder(paymentsForContract, account.FirstName)
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, this);
            }
        }

    }
}