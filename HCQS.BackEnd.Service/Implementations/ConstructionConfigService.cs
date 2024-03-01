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
using static HCQS.BackEnd.Common.Dto.Request.ProjectDto;
using static HCQS.BackEnd.DAL.Models.Project;

namespace HCQS.BackEnd.Service.Implementations
{
    public class ConstructionConfigService : GenericBackendService, IConstructionConfigService
    {
        private BackEndLogger _logger;
        private IUnitOfWork _unitOfWork;
        private IConstructionConfigRepository _constructionConfigRepository;

        public ConstructionConfigService(BackEndLogger logger, IUnitOfWork unitOfWork, IConstructionConfigRepository constructionConfigRepository, IMapper mapper, IServiceProvider serviceProvIder) : base(serviceProvIder)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _constructionConfigRepository = constructionConfigRepository;
        }

        public async Task<AppActionResult> CreateConstructionConfig(ConstructionConfigRequest request)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var requestString = await GetRequestString(request);
                    var constructionConfigDb = await _constructionConfigRepository.GetAllDataByExpression(n => n.Name.Contains(requestString));
                    if (constructionConfigDb != null && constructionConfigDb.Count > 0)
                    {
                        result = BuildAppActionResultError(result, $"The constructionConfig of {requestString} has already existed!");
                    }
                    else
                    {
                        //fix validator
                        List<ConstructionConfig> configs = new List<ConstructionConfig>();
                        configs.Add(new ConstructionConfig()
                        {
                            Id = Guid.NewGuid(),
                            Name = $"{requestString}, SandMixingRatio",
                            Value = (float)request.SandMixingRatio
                        });
                        configs.Add(new ConstructionConfig()
                        {
                            Id = Guid.NewGuid(),
                            Name = $"{requestString}, CementMixingRatio",
                            Value = (float)request.CementMixingRatio
                        });

                        configs.Add(new ConstructionConfig()
                        {
                            Id = Guid.NewGuid(),
                            Name = $"{requestString}, StoneMixingRatio",
                            Value = (float)request.StoneMixingRatio
                        });

                        result.Result.Data = await _constructionConfigRepository.InsertRange(configs);
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

        //
        public async Task<AppActionResult> DeleteConstructionConfig(DeleteConstructionConfigRequest request)

        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    string searchString = await GetDeleteString(request);
                    var constructionConfigDb = await _constructionConfigRepository.GetAllDataByExpression(c => c.Name.Contains(searchString), null);
                    if (constructionConfigDb == null || constructionConfigDb.Count == 0)
                    {
                        result = BuildAppActionResultError(result, $"The constructionConfig of {searchString} not found !");
                    }
                    else
                    {
                        await _constructionConfigRepository.DeleteRange(constructionConfigDb);
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
                var constructionConfigDb = await _constructionConfigRepository.GetAllDataByExpression(null, null);
                result.Result.Data = constructionConfigDb;
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
                string searchString = await GetSearchString(request.ConstructionType, request.NumOfFloor, request.Area, request.TiledArea);
                var constructionConfigDb = await _constructionConfigRepository.GetAllDataByExpression(c => c.Name.Contains(searchString));
                if (constructionConfigDb != null)
                {
                    var constructionConfig = new ConstructionConfigResponse();
                    Dictionary<string, bool> checkAvailableConfig = new Dictionary<string, bool>();
                    checkAvailableConfig.Add("SandMixingRatio", false);
                    checkAvailableConfig.Add("CementMixingRatio", false);
                    checkAvailableConfig.Add("StoneMixingRatio", false);
                    foreach (var config in constructionConfigDb)
                    {
                        if (config.Name.Contains("SandMixingRatio"))
                        {
                            constructionConfig.SandMixingRatio = (int)Math.Round(config.Value);
                            checkAvailableConfig["SandMixingRatio"] = true;
                        }
                        else if (config.Name.Contains("CementMixingRatio"))
                        {
                            constructionConfig.CementMixingRatio = (int)Math.Round(config.Value);
                            checkAvailableConfig["CementMixingRatio"] = true;
                        }
                        else if (config.Name.Contains("StoneMixingRatio"))
                        {
                            constructionConfig.StoneMixingRatio = (int)Math.Round(config.Value);
                            checkAvailableConfig["StoneMixingRatio"] = true;
                        }
                    }
                    StringBuilder sb = new StringBuilder();
                    foreach (KeyValuePair<string, bool> check in checkAvailableConfig)
                    {
                        if (!check.Value)
                        {
                            if (sb.Length == 0)
                            {
                                sb.Append("No available config for: ");
                            }
                            sb.Append($"{check.Key}, ");
                        }
                    }

                    if (sb.Length > 0)
                    {
                        result = BuildAppActionResultError(result, sb.ToString());
                    }
                    else
                    {
                        result.Result.Data = constructionConfig;
                    }
                }
                else
                {
                    result = BuildAppActionResultError(result, $"The is not config available for this project");
                }
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
                    var requestString = await GetRequestString(request);
                    var constructionConfigDb = await _constructionConfigRepository.GetAllDataByExpression(n => n.Name.Contains(requestString));
                    if (constructionConfigDb == null || constructionConfigDb.Count == 0)
                    {
                        result = BuildAppActionResultError(result, $"The constructionConfig of {requestString} does not existed!");
                    }
                    else
                    {
                        //fix validator
                        var sandConfig = constructionConfigDb.FirstOrDefault(c => c.Name.Contains("SandMixingRatio"));
                        sandConfig.Value = (float)request.SandMixingRatio;
                        var stoneConfig = constructionConfigDb.FirstOrDefault(c => c.Name.Contains("StoneMixingRatio"));
                        stoneConfig.Value = (float)request.StoneMixingRatio;
                        var cementConfig = constructionConfigDb.FirstOrDefault(c => c.Name.Contains("CementMixingRatio"));
                        cementConfig.Value = (float)request.CementMixingRatio;

                        await _constructionConfigRepository.Update(sandConfig);
                        await _constructionConfigRepository.Update(stoneConfig);
                        await _constructionConfigRepository.Update(cementConfig);
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

        private async Task<string> GetSearchString(Project.ProjectConstructionType constructionType, int numOfFloor, double area, double tiledArea)
        {
            StringBuilder sb = new StringBuilder();
            if (constructionType == ProjectConstructionType.CompleteConstruction)
            {
                sb.Append("CompleteConstruction");
            }
            else
            {
                sb.Append("RoughConstruction");
            }

            if (numOfFloor <= 2)
            {
                sb.Append(", 1-2 Floors");
            }
            else if (numOfFloor <= 5)
            {
                sb.Append(", 3-5 Floors");
            }
            else
            {
                sb.Append(", 6+ Floors");
            }

            if (area <= 50)
            {
                sb.Append(", 1-50");
            }
            else if (area <= 100)
            {
                sb.Append(", 50-100");
            }
            else if (area <= 300)
            {
                sb.Append(", 100-300");
            }
            else if (area <= 500)
            {
                sb.Append(", 300-500");
            }
            else
            {
                sb.Append(", 500+");
            }

            if (tiledArea <= 50)
            {
                sb.Append(", 1-50");
            }
            else if (tiledArea <= 100)
            {
                sb.Append(", 50-100");
            }
            else if (tiledArea <= 300)
            {
                sb.Append(", 100-300");
            }
            else if (tiledArea <= 500)
            {
                sb.Append(", 300-500");
            }
            else
            {
                sb.Append(", 500+");
            }
            return sb.ToString();
        }

        private async Task<string> GetRequestString(ConstructionConfigRequest request)
        {
            StringBuilder sb = new StringBuilder();
            if (request.ConstructionType == ProjectConstructionType.RoughConstruction)
            {
                sb.Append("RoughConstruction");
            }
            else
            {
                sb.Append("CompleteConstruction");
            }
            sb.Append($", {request.NumOfFloor} Floors");
            sb.Append($", {request.Area}");
            sb.Append($", {request.TiledArea}");

            return sb.ToString();
        }

        private async Task<string> GetDeleteString(DeleteConstructionConfigRequest request)
        {
            StringBuilder sb = new StringBuilder();
            if (request.ConstructionType == Project.ProjectConstructionType.RoughConstruction)
            {
                sb.Append("RoughConstruction");
            }
            else
            {
                sb.Append("CompleteConstruction");
            }
            sb.Append($", {request.NumOfFloor} Floors");
            sb.Append($", {request.Area}");
            sb.Append($", {request.TiledArea}");

            return sb.ToString();
        }

        public async Task<AppActionResult> CreateConstructionConfig(string name, float value)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var constructionConfigDb = await _constructionConfigRepository.GetByExpression(n => n.Name.Equals(name));
                    if (constructionConfigDb != null)
                    {
                        result = BuildAppActionResultError(result, $"The constructionConfig whose name is {name} has already existed!");
                    }
                    else
                    {
                        var constructionConfig = new ConstructionConfig()
                        {
                            Name = name,
                            Value = value
                        };

                        constructionConfig.Id = Guid.NewGuid();
                        result.Result.Data = await _constructionConfigRepository.Insert(constructionConfig);
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

        public async Task<AppActionResult> SearchConstructionConfig(string keyword)
        {
            AppActionResult result = new AppActionResult();
            try
            {
                var constructionConfigDb = await _constructionConfigRepository.GetAllDataByExpression(c => c.Name.Contains(keyword), null);
                result.Result.Data = constructionConfigDb;
            }
            catch (Exception ex)
            {
                result = BuildAppActionResultError(result, ex.Message);
                _logger.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<AppActionResult> DeleteAllConstructionConfig()
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var constructionConfigDb = await _constructionConfigRepository.GetAllDataByExpression(null, null);
                    if (constructionConfigDb == null)
                    {
                        result = BuildAppActionResultError(result, $"There is no construction config to delete!");
                    }
                    else
                    {
                        await _constructionConfigRepository.DeleteRange(constructionConfigDb);
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