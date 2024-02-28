using AutoMapper;
using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.Common.Util;
using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Implementations;
using HCQS.BackEnd.DAL.Models;
using HCQS.BackEnd.Service.Contracts;
using Microsoft.IdentityModel.Tokens;
using NPOI.SS.Formula.Functions;
using System.Transactions;

namespace HCQS.BackEnd.Service.Implementations
{
    public class ContractService : GenericBackendService, IContractService
    {
        private BackEndLogger _logger;
        private IUnitOfWork _unitOfWork;
        private IContractRepository _contractRepository;
        private IMapper _mapper;

        public ContractService(BackEndLogger logger, IMapper mapper, IUnitOfWork unitOfWork, IContractRepository contractRepository, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _contractRepository = contractRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<AppActionResult> GetContractById(Guid contractId)
        {
            AppActionResult result = new AppActionResult();
            try
            {
                result.Result.Data = await _contractRepository.GetByExpression(c => c.Id == contractId);

            }
            catch (Exception ex)
            {
                result = BuildAppActionResultError(result, ex.Message);
                _logger.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<AppActionResult> ReSendVerificationCode(Guid contractId)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var emailService = Resolve<IEmailService>();
                    var accountRepository = Resolve<IAccountRepository>();
                    var contractDb = await _contractRepository.GetById(contractId);
                    string code = Guid.NewGuid().ToString("N").Substring(0, 6);
                    if (contractDb == null)
                    {
                        result = BuildAppActionResultError(result, $"The contract with id {contractId} is not existed");

                    }else if(contractDb.ContractStatus != Contract.Status.IN_ACTIVE)
                    {
                        result = BuildAppActionResultError(result, $"Resend code is only valid when the contract status is in active");

                    }
                    var account = await accountRepository.GetByExpression(c => c.Id == contractDb.Project.AccountId);
                    if (account == null)
                    {
                        result = BuildAppActionResultError(result, $"The account with id {account.Id} is not existed");
                    }
                    if (!BuildAppActionResultIsError(result))
                    {
                        account.ContractVerifyCode = code;
                        emailService.SendEmail(account.Email, SD.SubjectMail.SIGN_CONTRACT_VERIFICATION_CODE, code);
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

        public async Task<AppActionResult> SignContract(Guid contractId, string accountId, string verificationCode)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var accountRepository = Resolve<IAccountRepository>();
                    var projectRepository = Resolve<IProjectRepository>();
                    var contractProgressPaymentRepository = Resolve<IContractProgressPaymentRepository>();
                    var fileService = Resolve<IFileService>();
                    var utility = Resolve<Common.Util.Utility>();

                    var contractDb = await _contractRepository.GetByExpression(c => c.Id == contractId, c => c.Project, c => c.Project);
                    var accountDb = await accountRepository.GetById(accountId);
                    var listCPP = await contractProgressPaymentRepository.GetAllDataByExpression(c => c.ContractId == contractId, c => c.Payment);
                    if (contractDb == null || accountDb == null)
                    {
                        result = BuildAppActionResultError(result, $"The account with id{accountId} or contract with id {contractId} is not existed");
                    }
                    else if (verificationCode != accountDb.ContractVerifyCode)
                    {
                        result = BuildAppActionResultError(result, $"The verification code is wrong");
                    }
                    else if (string.IsNullOrEmpty(accountDb.PhoneNumber))
                    {
                        result = BuildAppActionResultError(result, $"The account {accountDb.Id} don't have phone number. You must update phone number to sign this contract");

                    }
                    if (!listCPP.Any())
                    {
                        result = BuildAppActionResultError(result, $"The list contract progress payment is empty");

                    }

                    if (!BuildAppActionResultIsError(result))
                    {
                        accountDb.ContractVerifyCode = null;
                        contractDb.ContractStatus = DAL.Models.Contract.Status.ACTIVE;
                        ContractTemplateDto templateDto = new ContractTemplateDto
                        {
                            Account = accountDb,
                            Contract = contractDb,
                            ContractProgressPayments = listCPP,
                            CreateDate = contractDb.DateOfContract,
                            IsSigned = true,
                            Project = contractDb.Project,
                            SignDate = utility.GetCurrentDateTimeInTimeZone()
                        };
                        var content = TemplateMappingHelper.GetTemplateContract(templateDto);
                        var projectDb = await projectRepository.GetById(contractDb.ProjectId);
                        projectDb.ProjectStatus = DAL.Models.Project.Status.UnderConstruction;
                        var delete = await fileService.DeleteImageFromFirebase($"contract/{contractDb.Id}");
                        var upload = await fileService.UploadImageToFirebase(fileService.ConvertHtmlToPdf(contractDb.Content, $"{contractDb.Id}.pdf"), $"contract/{contractDb.Id}");
                        contractDb.ContractUrl = Convert.ToString(upload.Result.Data);
                        await _contractRepository.Update(contractDb);
                        await projectRepository.Update(projectDb);
                        await accountRepository.Update(accountDb);
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