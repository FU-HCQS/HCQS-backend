using AutoMapper;
using Hangfire.Server;
using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.Common.Util;
using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Models;
using HCQS.BackEnd.Service.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace HCQS.BackEnd.Service.Implementations
{
    public class WorkerPriceService : GenericBackendService, IWorkerPriceService
    {
        private IWorkerPriceRepository _workerPriceRepository;
        private IMapper _mapper;
        private BackEndLogger _logger;
        private IUnitOfWork _unitOfWork;

        public WorkerPriceService(IWorkerPriceRepository repository, IMapper mapper, BackEndLogger logger, IUnitOfWork unitOfWork, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _workerPriceRepository = repository;
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<AppActionResult> CreateWokerPrice(WorkerPriceRequest worker)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {

                    worker.Id = Guid.NewGuid();
                    WorkerPrice workerMapper = _mapper.Map<WorkerPrice>(worker);
                    var workerDb = await _workerPriceRepository.GetByExpression(a => a.PositionName.ToLower() == worker.PositionName.ToLower() && a.IsDeleted==false);
                    if (workerDb != null)
                    {
                        result = BuildAppActionResultError(result, $"The labor with position name is existed");
                    }
                    if (!BuildAppActionResultIsError(result))
                    {
                        await _workerPriceRepository.Insert(workerMapper);
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

        public async Task<AppActionResult> DeleteWorkerPriceById(Guid id)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {

                    var workerDb = await _workerPriceRepository.GetById(id);
                    if (workerDb == null)
                    {
                        result = BuildAppActionResultError(result, $"The labor with id {id} is not existed");
                    }
                    if (!BuildAppActionResultIsError(result))
                    {
                        workerDb.IsDeleted = true;
                        await _workerPriceRepository.Update(workerDb);
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

        public async Task<AppActionResult> GetAll()
        {
            AppActionResult result = new AppActionResult();
            try
            {
                result.Result.Data = await _workerPriceRepository.GetAllDataByExpression( a=> a.IsDeleted == false, null);
            }
            catch (Exception ex)
            {
                result = BuildAppActionResultError(result, ex.Message);
                _logger.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<AppActionResult> GetById(Guid id)
        {
            AppActionResult result = new AppActionResult();
            try
            {
                result.Result.Data = await _workerPriceRepository.GetById(id);
            }
            catch (Exception ex)
            {
                result = BuildAppActionResultError(result, ex.Message);
                _logger.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<AppActionResult> GetByPositionName(string positionName)
        {
            AppActionResult result = new AppActionResult();
            try
            {
                result.Result.Data = await _workerPriceRepository.GetAllDataByExpression(a => a.PositionName.Contains(positionName) && a.IsDeleted == false, null);
            }
            catch (Exception ex)
            {
                result = BuildAppActionResultError(result, ex.Message);
                _logger.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<AppActionResult> UpdateWorkerPrice(WorkerPriceRequest worker)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var workerDb = await _workerPriceRepository.GetById(worker.Id);
                    if (workerDb == null)
                    {
                        result = BuildAppActionResultError(result, $"The labor with id {worker.Id} is not existed");
                    }
                    if (!BuildAppActionResultIsError(result))
                    {
                        _mapper.Map(worker, workerDb);
                        await _workerPriceRepository.Update(workerDb);
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
    }
}
