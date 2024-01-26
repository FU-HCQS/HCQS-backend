using AutoMapper;
using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Util;
using HCQS.BackEnd.Service.Contracts;
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

        public async Task<AppActionResult> SignContract(Guid contractId, Guid accountId, string verificationCode)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var accountRepository = Resolve<IAccountRepository>();
                    var projectRepository = Resolve<IProjectRepository>();
                    var contractDb = await _contractRepository.GetById(contractId);
                    var accountDb = await accountRepository.GetById(accountId);
                    if (contractDb == null || accountDb == null)
                    {
                        result = BuildAppActionResultError(result, $"The account with id{accountId} or contract with id {contractId} is not existed");
                    }
                    else if (verificationCode != accountDb.ContractVerifyCode)
                    {
                        result = BuildAppActionResultError(result, $"The verification code is wrong");
                    }
                    if (!BuildAppActionResultIsError(result))
                    {
                        accountDb.ContractVerifyCode = null;
                        contractDb.ContractStatus = DAL.Models.Contract.Status.ACTIVE;
                        var projectDb = await projectRepository.GetById(contractDb.ProjectId);
                        projectDb.ProjectStatus = DAL.Models.Project.Status.UnderConstruction;
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