using AutoMapper;
using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Util;
using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.Service.Contracts;

namespace HCQS.BackEnd.Service.Implementations
{
    public class WorkerForProjectService : GenericBackendService, IWorkerForProjectService
    {
        private IWorkerForProjectRepository _workerPriceRepository;
        private IMapper _mapper;
        private BackEndLogger _logger;
        private IUnitOfWork _unitOfWork;

        public WorkerForProjectService(IWorkerForProjectRepository repository, IMapper mapper, BackEndLogger logger, IUnitOfWork unitOfWork, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _workerPriceRepository = repository;
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<AppActionResult> GetListWorkerByQuotationId(Guid id)
        {
            AppActionResult result = new AppActionResult();
            try
            {
                result.Result.Data = await _workerPriceRepository.GetAllDataByExpression(filter: a => a.QuotationId == id, a => a.WorkerPrice);
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