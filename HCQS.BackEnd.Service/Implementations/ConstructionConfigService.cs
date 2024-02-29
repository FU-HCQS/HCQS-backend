using AutoMapper;
using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.Common.Dto.Response;
using HCQS.BackEnd.Common.Util;
using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Implementations;
using HCQS.BackEnd.DAL.Models;
using HCQS.BackEnd.Service.Contracts;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace HCQS.BackEnd.Service.Implementations
{
    public class ConstructionConfigService : GenericBackendService, IConstructionConfigService
    {
        private BackEndLogger _logger;
        private IUnitOfWork _unitOfWork;
        private IConstructionConfigRepository _constructionConfigRepository;
        public ConstructionConfigService(BackEndLogger logger, IUnitOfWork unitOfWork, IConstructionConfigRepository constructionConfigRepository, IMapper mapper, IServiceProvider serviceProvIder):base(serviceProvIder) 
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
                    var constructionConfigDb = await _constructionConfigRepository.GetByExpression(n => n.Name.Equals(requestString));
                    if (constructionConfigDb != null)
                    {
                        result = BuildAppActionResultError(result, $"The constructionConfig whose name is {requestString} has already existed!");
                    }
                    else
                    {
                        var constructionConfig = new ConstructionConfig()
                        {
                            Name = requestString,
                            Value = request.Value
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

        public async Task<AppActionResult> DeleteConstructionConfig(Guid Id)

        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var constructionConfigDb = await _constructionConfigRepository.GetById(Id);
                    if (constructionConfigDb == null)
                    {
                        result = BuildAppActionResultError(result, $"The constructionConfig with {Id} not found !");
                    }
                    else
                    {
                        result.Result.Data = await _constructionConfigRepository.DeleteById(Id);
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

        public async Task<AppActionResult> GetConstructionConfig(Guid projectId)
        {
            AppActionResult result = new AppActionResult();
            try
            {
                var projectRepository = Resolve<IProjectRepository>();
                var projectDb = await projectRepository.GetByExpression(c => c.Id == projectId);
                if(projectDb == null)
                {
                    result = BuildAppActionResultError(result, $"Project with id: {projectId} does not exist");
                }
                else
                {
                    string searchString = await GetSearchString(projectDb);
                    var constructionConfigDb = await _constructionConfigRepository.GetAllDataByExpression(c => c.Name.Contains(searchString));
                    if (constructionConfigDb != null)
                    {
                        var constructionConfig = new ConstructionConfigResponse();
                        Dictionary<string, bool> checkAvailableConfig = new Dictionary<string, bool>();
                        checkAvailableConfig.Add("SandMixingRatio", false);
                        checkAvailableConfig.Add("CementMixingRatio", false);
                        checkAvailableConfig.Add("StoneMixingRatio", false);
                        checkAvailableConfig.Add("EstimatedTimeOfCompletion", false);
                        checkAvailableConfig.Add("NumberOfLabor", false);
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
                            else if (config.Name.Contains("EstimatedTimeOfCompletion"))
                            {
                                constructionConfig.EstimatedTimeOfCompletion = (int)Math.Round(config.Value);
                                checkAvailableConfig["EstimatedTimeOfCompletion"] = true;
                            }
                            else if (config.Name.Contains("NumberOfLabor"))
                            {
                                constructionConfig.NumberOfLabor = (int)Math.Round(config.Value);
                                checkAvailableConfig["NumberOfLabor"] = true;
                            }
                        }
                        StringBuilder sb = new StringBuilder();
                        foreach(KeyValuePair<string, bool> check in checkAvailableConfig) {
                            if(!check.Value)
                            {
                                if (sb.Length == 0)
                                {
                                    sb.Append("No available config for: ");
                                }
                                sb.Append($"{check.Key}, ");
                            }
                        }

                        if(sb.Length > 0)
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
                        result = BuildAppActionResultError(result, $"The is not config availabl for this project");
                    }
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
                    var constructionConfigDb = await _constructionConfigRepository.GetByExpression(n => n.Name.Equals(requestString));
                    if (constructionConfigDb == null)
                    {
                        result = BuildAppActionResultError(result, $"The constructionConfig whose name is {requestString} does not exist!");
                    }
                    else
                    {
                        constructionConfigDb.Value = request.Value;
                        result.Result.Data = await _constructionConfigRepository.Update(constructionConfigDb);
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

        private async Task<string> GetSearchString(Project project)
        {
            StringBuilder sb = new StringBuilder();
            if(project.ConstructionType == Project.ProjectConstructionType.CompleteConstruction)
            {
                sb.Append("RoughConstruction");
            }
            else
            {
                sb.Append("CompleteConstruction");
            }

            if(project.NumberOfLabor <= 2)
            {
                sb.Append(", 1-2 Floors");
            } else if(project.NumberOfLabor <= 5)
            {
                sb.Append(", 3-5 Floors");
            }
            else
            {
                sb.Append(", 6+ Floors");
            }

            if (project.Area <= 100)
            {
                sb.Append(", 1-100");
            } else if (project.Area <= 300)
            {
                sb.Append(", 100-300");
            } else if (project.Area <= 500)
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
            if(request.constructionType == ConstructionConfigRequest.ConstructionType.RoughConstruction)
            {
                sb.Append("RoughConstruction");
            }
            else
            {
                sb.Append("CompleteConstruction");
            }

            sb.Append($", {request.NumOfFloor}");
            sb.Append($", {request.Area}");
            if(request.configType == ConstructionConfigRequest.ConfigType.SandMixingRatio)
            {
                sb.Append($", SandMixingRatio");
            } else if(request.configType == ConstructionConfigRequest.ConfigType.CementMixingRatio)
            {
                sb.Append($", CementMixingRatio");
            } else if(request.configType == ConstructionConfigRequest.ConfigType.StoneMixingRatio)
            {
                sb.Append($", StoneMixingRatio");
            } else if(request.configType == ConstructionConfigRequest.ConfigType.EstimatedTimeOfCompletion)
            {
                sb.Append($", EstimatedTimeOfCompletion");
            } else
            {
                sb.Append($", NumberOfLabor");
            }

            return sb.ToString() ;
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
    }
}
