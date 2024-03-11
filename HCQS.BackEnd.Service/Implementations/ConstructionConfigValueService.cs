using AutoMapper;
using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.Common.Dto.Response;
using HCQS.BackEnd.Common.Util;
using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Models;
using HCQS.BackEnd.Service.Contracts;
using System.Text;
using System.Transactions;
using static HCQS.BackEnd.DAL.Models.Project;

namespace HCQS.BackEnd.Service.Implementations
{
    public class ConstructionConfigValueService : GenericBackendService, IConstructionConfigValueService
    {
        private BackEndLogger _logger;
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        private IConstructionConfigValueRepository _constructionConfigValueRepository;

        public ConstructionConfigValueService(BackEndLogger logger, IUnitOfWork unitOfWork, IConstructionConfigValueRepository constructionConfigValueRepository, IMapper mapper, IServiceProvider serviceProvIder) : base(serviceProvIder)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _constructionConfigValueRepository = constructionConfigValueRepository;
            _mapper = mapper;
        }

        public async Task<AppActionResult> CreateConstructionConfig(ConstructionConfigRequest request)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var configDb = await _constructionConfigValueRepository.GetByExpression(c =>
                                                                c.NumOfFloorMin == request.NumOfFloorMin
                                                             && c.NumOfFloorMax == request.NumOfFloorMax
                                                             && c.AreaMin == request.AreaMin
                                                             && c.AreaMax == request.AreaMax
                                                             && c.TiledAreaMin == request.TiledAreaMin
                                                             && c.TiledAreaMax == request.TiledAreaMax
                                                             && c.ConstructionType == request.ConstructionType);
                    if (configDb != null)
                    {
                        return BuildAppActionResultError(result, "There exists a config with same parameter!");
                    }

                    StringBuilder sb = new StringBuilder();
                    sb.Append("Collided range of ");
                    bool isCollided = false;
                    var colidedConfig = await _constructionConfigValueRepository.GetAllDataByExpression(c =>
                                                                c.NumOfFloorMax > request.NumOfFloorMin
                                                             && c.NumOfFloorMin < request.NumOfFloorMax
                                                             && !(c.NumOfFloorMin == request.NumOfFloorMin
                                                             && c.NumOfFloorMax == request.NumOfFloorMax));
                    if (colidedConfig != null && colidedConfig.Count > 0)
                    {
                        isCollided = true;
                        sb.Append($"Number of floors: {request.NumOfFloorMin}-{request.NumOfFloorMax}, ");
                    }

                    colidedConfig = await _constructionConfigValueRepository.GetAllDataByExpression(c =>
                                                                c.AreaMax > request.AreaMin
                                                             && c.AreaMin < request.AreaMax
                                                             && !(c.AreaMax == request.AreaMax
                                                             && c.AreaMin == request.AreaMin));
                    if (colidedConfig != null && colidedConfig.Count > 0)
                    {
                        isCollided = true;
                        sb.Append($"Area: {request.AreaMin}-{request.AreaMax}, ");
                    }

                    colidedConfig = await _constructionConfigValueRepository.GetAllDataByExpression(c =>
                                                                c.TiledAreaMax > request.TiledAreaMin
                                                             && c.TiledAreaMin < request.TiledAreaMax
                                                             && !(c.TiledAreaMin == request.TiledAreaMin
                                                             && c.TiledAreaMax == request.TiledAreaMax));
                    if (colidedConfig != null && colidedConfig.Count > 0)
                    {
                        isCollided = true;
                        sb.Append($"Tiled area: {request.TiledAreaMin}-{request.TiledAreaMax}, ");
                    }
                    if (isCollided)
                    {
                        result = BuildAppActionResultError(result, sb.Remove(sb.Length - 2, 2).ToString());
                    }
                    else
                    {
                        var constructionConfigValue = _mapper.Map<ConstructionConfigValue>(request);
                        constructionConfigValue.Id = Guid.NewGuid();
                        await _constructionConfigValueRepository.Insert(constructionConfigValue);
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

        public async Task<AppActionResult> DeleteAllConstructionConfig()
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var constructionConfigDb = await _constructionConfigValueRepository.GetAllDataByExpression(null, null);
                    if (constructionConfigDb == null)
                    {
                        result = BuildAppActionResultError(result, $"There is no construction config to delete!");
                    }
                    else
                    {
                        await _constructionConfigValueRepository.DeleteRange(constructionConfigDb);
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

        public async Task<AppActionResult> DeleteConstructionConfig(FilterConstructionConfigRequest request)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var configDb = await _constructionConfigValueRepository.GetByExpression(c =>
                                                                c.NumOfFloorMin == request.NumOfFloorMin
                                                             && c.NumOfFloorMax == request.NumOfFloorMax
                                                             && c.AreaMin == request.AreaMin
                                                             && c.AreaMax == request.AreaMax
                                                             && c.TiledAreaMin == request.TiledAreaMin
                                                             && c.TiledAreaMax == request.TiledAreaMax
                                                             && c.ConstructionType == request.ConstructionType);
                    if (configDb == null)
                    {
                        return BuildAppActionResultError(result, "There does not exist a config with same parameter!");
                    }
                    else
                    {
                        await _constructionConfigValueRepository.DeleteById(configDb.Id);
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

        public async Task<AppActionResult> DeleteConstructionConfigById(Guid Id)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var configDb = await _constructionConfigValueRepository.GetById(Id);
                    if (configDb == null)
                    {
                        return BuildAppActionResultError(result, "There does not exist a config with same parameter!");
                    }
                    else
                    {
                        await _constructionConfigValueRepository.DeleteById(configDb.Id);
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

        public async Task<AppActionResult> GetAll()
        {
            AppActionResult result = new AppActionResult();
            try
            {
                var configDb = await _constructionConfigValueRepository.GetAllDataByExpression(null);
                result.Result.Data = configDb;
            }
            catch (Exception ex)
            {
                result = BuildAppActionResultError(result, ex.Message);
                _logger.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<AppActionResult> GetConstructionConfig(SearchConstructionConfigRequest request)
        {
            AppActionResult result = new AppActionResult();
            try
            {
                var configDb = await _constructionConfigValueRepository.GetByExpression(c =>
                                                            c.NumOfFloorMin <= request.NumOfFloor
                                                         && c.NumOfFloorMax >= request.NumOfFloor
                                                         && c.AreaMin <= request.Area
                                                         && c.AreaMax >= request.Area
                                                         && c.TiledAreaMin <= request.TiledArea
                                                         && c.TiledAreaMax >= request.TiledArea
                                                         && c.ConstructionType == request.ConstructionType);
                if (configDb == null)
                {
                    return BuildAppActionResultError(result, "There does not exist a config with similar parameter!");
                }
                result.Result.Data = _mapper.Map<ConstructionConfigResponse>(configDb);
            }
            catch (Exception ex)
            {
                result = BuildAppActionResultError(result, ex.Message);
                _logger.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<AppActionResult> GetMaxConfig(ProjectConstructionType ConstructionType)
        {
            AppActionResult result = new AppActionResult();
            try
            {
                var configDb = await _constructionConfigValueRepository.GetAllDataByExpression(c => c.ConstructionType == ConstructionType);
                if (configDb == null)
                {
                    return BuildAppActionResultError(result, "There does not exist a config with same parameter!");
                }
                result.Result.Data = new MaxContructionConfig()
                {
                    NumOfFloorMax = configDb.MaxBy(c => c.NumOfFloorMax).NumOfFloorMax,
                    AreaMax = configDb.MaxBy(c => c.AreaMax).AreaMax,
                    TiledAreaMax = configDb.MaxBy(c => c.TiledAreaMax).TiledAreaMax
                };
            }
            catch (Exception ex)
            {
                result = BuildAppActionResultError(result, ex.Message);
                _logger.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<AppActionResult> SearchConstructionConfig(FilterConstructionConfigRequest request)
        {
            AppActionResult result = new AppActionResult();
            try
            {
                var configDb = await _constructionConfigValueRepository.GetAllDataByExpression(c =>
                                                                ((request.NumOfFloorMin + request.NumOfFloorMax == 0)
                                                             || (c.NumOfFloorMin >= request.NumOfFloorMin
                                                             && c.NumOfFloorMax <= request.NumOfFloorMax))

                                                             && ((request.AreaMin + request.AreaMax == 0)
                                                             || (c.AreaMin >= request.AreaMin
                                                             && c.AreaMax <= request.AreaMax))

                                                             && ((request.TiledAreaMin + request.TiledAreaMax == 0)
                                                             || (c.TiledAreaMin >= request.TiledAreaMin
                                                             && c.TiledAreaMax <= request.TiledAreaMax))

                                                             && c.ConstructionType == request.ConstructionType);
                result.Result.Data = configDb;
            }
            catch (Exception ex)
            {
                result = BuildAppActionResultError(result, ex.Message);
                _logger.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<AppActionResult> UpdateConstructionConfig(ConstructionConfigRequest request)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var configDb = await _constructionConfigValueRepository.GetByExpression(c =>
                                                                c.NumOfFloorMin == request.NumOfFloorMin
                                                             && c.NumOfFloorMax == request.NumOfFloorMax
                                                             && c.AreaMin == request.AreaMin
                                                             && c.AreaMax == request.AreaMax
                                                             && c.TiledAreaMin == request.TiledAreaMin
                                                             && c.TiledAreaMax == request.TiledAreaMax
                                                             && c.ConstructionType == request.ConstructionType);
                    if (configDb == null)
                    {
                        return BuildAppActionResultError(result, "There does not exist a config with same parameter!");
                    }
                    configDb.SandMixingRatio = request.SandMixingRatio;
                    configDb.StoneMixingRatio = request.StoneMixingRatio;
                    configDb.CementMixingRatio = request.CementMixingRatio;
                    await _constructionConfigValueRepository.Update(configDb);
                    await _unitOfWork.SaveChangeAsync();
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