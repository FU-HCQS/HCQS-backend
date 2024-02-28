using AutoMapper;
using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.Common.Util;
using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Models;
using HCQS.BackEnd.Service.Contracts;
using Microsoft.IdentityModel.Tokens;
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
            try { 
            result.Result.Data = await _contractRepository.GetByExpression(c => c.Id == contractId);
            
            }catch(Exception ex) {
                result = BuildAppActionResultError(result, ex.Message);
                _logger.LogError(ex.Message, this);
            }
            return result;
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

                    var contractDb = await _contractRepository.GetByExpression(c=> c.Id== contractId, c=> c.Project, c=> c.Project);
                    var accountDb = await accountRepository.GetById(accountId);
                    var listCPP = await contractProgressPaymentRepository.GetAllDataByExpression(c => c.ContractId == contractId, c=> c.Payment);
                    if (contractDb == null || accountDb == null)
                    {
                        result = BuildAppActionResultError(result, $"The account with id{accountId} or contract with id {contractId} is not existed");
                    }
                    else if (verificationCode != accountDb.ContractVerifyCode)
                    {
                        result = BuildAppActionResultError(result, $"The verification code is wrong");
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