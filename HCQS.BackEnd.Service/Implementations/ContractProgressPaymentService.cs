using AutoMapper;
using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.Common.Util;
using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Models;
using HCQS.BackEnd.Service.Contracts;
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
                try
                {
                    List<ContractProgressPayment> listCPP = new List<ContractProgressPayment>();
                    List<Payment> listPM = new List<Payment>();
                    var contractDb = await contractRepository.GetById(list?.First()?.ContractId);
                    if (contractDb == null)
                    {
                        result = BuildAppActionResultError(result, $"The contract with id {list.First().ContractId} is existed");
                    }
                    foreach (var item in list)
                    {
                        var contractProgressPaymentDb = await _repository.GetByExpression(a => a.Name == item.Name && a.ContractId == item.ContractId);
                        if (contractProgressPaymentDb != null)
                        {
                            result = BuildAppActionResultError(result, $"The contract progress payment with name {item.Name} and contractId {item.ContractId} is existed");
                        }
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
                        contractDb.ContractStatus = Contract.Status.IN_ACTIVE;
                    }

                    if (!BuildAppActionResultIsError(result))
                    {
                        await _repository.InsertRange(listCPP);
                        await paymentRepository.InsertRange(listPM);
                        await contractRepository.Update(contractDb);
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

        public async Task<AppActionResult> DeleteContractProgressPaymentById(Guid id)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                var paymentRepository = Resolve<IPaymentRepository>();
                try
                {
                    var contractProgressPaymentDb = await _repository.GetById(id);
                    if (contractProgressPaymentDb == null)
                    {
                        result = BuildAppActionResultError(result, $"The contract progress payment with id {id} is not existed!");
                    }
                    if (!BuildAppActionResultIsError(result))
                    {
                        var paymentDb = await paymentRepository.GetById(contractProgressPaymentDb.PaymentId);
                        if (paymentDb != null)
                        {
                            await paymentRepository.DeleteById(paymentDb.Id);
                        }
                        await _repository.DeleteById(id);
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

        public async Task<AppActionResult> UpdateContractProgressPayment(ContractProgressPaymentDto dto)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                var paymentRepository = Resolve<IPaymentRepository>();
                try
                {
                    var contractProgressPaymentDb = await _repository.GetById(dto.Id);
                    if (contractProgressPaymentDb == null)
                    {
                        result = BuildAppActionResultError(result, $"The contract progress payment with id {dto.Id} is not existed");
                    }
                    var paymentDb = await paymentRepository.GetById(contractProgressPaymentDb.PaymentId);
                    if (paymentDb == null)
                    {
                        result = BuildAppActionResultError(result, $"The payment with id {contractProgressPaymentDb.Id} is not existed");
                    }
                    if (!BuildAppActionResultIsError(result))
                    {
                        contractProgressPaymentDb.Name = dto.Name;
                        paymentDb.Price = (double)dto.Price;
                        paymentDb.Content = dto.Content;
                        paymentDb.ContractProgressPayment = contractProgressPaymentDb;
                        contractProgressPaymentDb.Payment = paymentDb;
                    }
                    if (!BuildAppActionResultIsError(result))
                    {
                        await _repository.Update(contractProgressPaymentDb);
                        await paymentRepository.Update(paymentDb);
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
    }
}