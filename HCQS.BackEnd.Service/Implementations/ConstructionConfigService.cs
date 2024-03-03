using AutoMapper;
using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.Common.Dto.Response;
using HCQS.BackEnd.Common.Util;
using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Models;
using HCQS.BackEnd.Service.Contracts;
using ICSharpCode.SharpZipLib.Zip;
using NPOI.HSSF.Record;
using NPOI.Util.ArrayExtensions;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Diagnostics;
using System.Linq;
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
                        bool validRange = true;
                        var allContructionConfig = await _constructionConfigRepository.GetAllDataByExpression(null);
                        StringBuilder sb = new StringBuilder();
                        sb.Append("Invalid range of ");
                        if(allContructionConfig.Count != 0)
                        {
                            List<string>[] configsSet = await getThreeRange(allContructionConfig.Select(c => c.Name).ToList());
                            validRange &= await IsValidRange(configsSet[0], $"{request.NumOfFloorMin}-{request.NumOfFloorMax}");
                            if (!validRange)
                            {
                                sb.Append($"number of floor: {request.NumOfFloorMin}-{request.NumOfFloorMax}, ");
                            }
                            validRange &= await IsValidRange(configsSet[1], $"{request.AreaMin}-{request.AreaMax}");
                            if (!validRange)
                            {
                                sb.Append($"area: {request.AreaMin}-{request.AreaMax}, ");
                            }
                            validRange &= await IsValidRange(configsSet[2], $"{request.TiledAreaMin}-{request.TiledAreaMax}");
                            if (!validRange)
                            {
                                sb.Append($"tiled area: {request.TiledAreaMin}-{request.TiledAreaMax}");
                            }
                        }


                        if (validRange)
                        {
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
                        else
                        {
                            result = BuildAppActionResultError(result, sb.ToString());
                        }
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
        
        public async Task<AppActionResult> DeleteConstructionConfig(DeleteConstructionConfigRequest request)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
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
                stopwatch.Stop();
                result.Messages.Add($"{stopwatch.Elapsed}");
                return result;
            }
        }

        public async Task<AppActionResult> GetAll()
        {
            AppActionResult result = new AppActionResult();
            try
            {                
                var constructionConfigDb = await _constructionConfigRepository.GetAllDataByExpression(null, null);
                var groupedRecords = constructionConfigDb
            .GroupBy(record => GetGroupKey(record.Name))
            .Select(group => new
            {
                GroupKey = group.Key,
                Records = group.ToList()
            }).ToList();
                List<ConstructionConfigRequest> constructionConfigRequests = new List<ConstructionConfigRequest>();
                // Print the grouped records
                foreach (var group in groupedRecords)
                {
                    var constructionConfigRequest = new ConstructionConfigRequest();
                    StringToConfig(ref constructionConfigRequest, group.GroupKey);
                    foreach(var value in group.Records) {
                        if (value.Name.Contains("Sand"))
                        {
                            constructionConfigRequest.SandMixingRatio = value.Value;
                        } else if (value.Name.Contains("Cement"))
                        {
                            constructionConfigRequest.CementMixingRatio = value.Value;
                        }
                        else
                        {
                            constructionConfigRequest.StoneMixingRatio = value.Value;
                        }
                    }
                    constructionConfigRequests.Add(constructionConfigRequest);
                }


                result.Result.Data = constructionConfigRequests;
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
            try
            {
                var allConstructionConfig = await _constructionConfigRepository.GetAllDataByExpression(null);

                if (allConstructionConfig == null || allConstructionConfig.Count == 0)
                {
                    return BuildAppActionResultError(new AppActionResult(), "There is no config available for this project");
                }

                var searchString = await GetSearchString(request.ConstructionType, request.NumOfFloor, request.Area, request.TiledArea);

                var constructionConfigDb = await _constructionConfigRepository.GetAllDataByExpression(c => c.Name.Contains(searchString));

                if (constructionConfigDb == null || constructionConfigDb.Count == 0)
                {
                    return BuildAppActionResultError(new AppActionResult(), "There is no config available for this project");
                }

                var constructionConfig = new ConstructionConfigResponse();
                var checkAvailableConfig = new Dictionary<string, bool>
        {
            {"SandMixingRatio", false},
            {"CementMixingRatio", false},
            {"StoneMixingRatio", false}
        };

                Parallel.ForEach(constructionConfigDb, config =>
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
                });

                var missingConfigs = checkAvailableConfig.Where(x => !x.Value).Select(x => x.Key);
                if (missingConfigs.Any())
                {
                    return BuildAppActionResultError(new AppActionResult(), $"No available config for: {string.Join(", ", missingConfigs)}");
                }

                var result = new AppActionResult { Result = { Data = constructionConfig } };
                return result;
            }
            catch (Exception ex) // Specify the actual exception type
            {
                _logger.LogError(ex.Message, this);
                return BuildAppActionResultError(new AppActionResult(), ex.Message);
            }
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

        public async Task<AppActionResult> GetConstructionConfigById(Guid Id)
        {
            AppActionResult result = new AppActionResult();
            try
            {
                var constructionConfigDb = await _constructionConfigRepository.GetById(Id);
                result.Result.Data = constructionConfigDb;
            }
            catch (Exception ex)
            {
                result = BuildAppActionResultError(result, ex.Message);
                _logger.LogError(ex.Message, this);
            }
            return result;
        }
        public async Task<bool> IsValidRange(List<string> currentRanges, string inputRange)
        {
            var currentRangesSet = new HashSet<string>(currentRanges);

            if (currentRangesSet.Contains(inputRange))
            {
                return true; // Exact match found
            }

            var intervals = new List<(int, int)>();

            foreach (var currentRange in currentRangesSet)
            {
                var rangeParts = currentRange.Split('-');

                if (rangeParts.Length == 2)
                {
                    if (int.TryParse(rangeParts[0], out int start) && int.TryParse(rangeParts[1], out int end))
                    {
                        intervals.Add((start, end));
                    }
                    else
                    {
                        return false; // Invalid range format
                    }
                }
                else
                {
                    intervals.Add((int.Parse(rangeParts[0].TrimEnd('+')), int.MaxValue));
                }
            }

            var inputParts = inputRange.Split("-");
            if (inputParts.Length == 2)
            {
                if (int.TryParse(inputParts[0], out int start) && int.TryParse(inputParts[1], out int end))
                {
                    intervals.Add((start, end));
                }
                else
                {
                    return false; // Invalid input range format
                }
            }
            else
            {
                intervals.Add((int.Parse(inputParts[0].TrimEnd('+')), int.MaxValue));
            }
            var orderedInterval = intervals.OrderBy(x => x.Item1);
            int maxEnd = int.MinValue;

            foreach (var interval in orderedInterval)
            {
                if (interval.Item1 < maxEnd)
                {
                    return false; // Overlapping intervals
                }

                maxEnd = Math.Max(maxEnd, interval.Item2);
            }

            return true;
        }
        public async Task<List<string>[]> getThreeRange(List<string> configNames)
        {
            HashSet<string>[] result = new HashSet<string>[3];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new HashSet<string>();
            }

            string[] lineConfig = null;
            foreach (string configName in configNames)
            {
                lineConfig = configName.Split(", ");
                result[0].Add(lineConfig[1].Substring(0, lineConfig[1].Length - 7));
                result[1].Add(lineConfig[2]);
                result[2].Add(lineConfig[3]);
            }
            return new List<string>[]{
                result[0].ToList(),
                result[1].ToList(),
                result[2].ToList()
            };
        }
        public async Task<string> GetSearchRange(List<string> ranges, double value)
        {
            var intervals = new List<Tuple<int, int>>();

            foreach (var currentRange in ranges)
            {
                var range = currentRange.Split('-');

                if (range.Length == 2)
                {
                    intervals.Add(Tuple.Create(int.Parse(range[0]), int.Parse(range[1])));
                }
                else
                {
                    intervals.Add(Tuple.Create(int.Parse(range[0].Substring(0, range[0].Length - 1)), int.MaxValue));
                }
            }

            foreach (var interval in intervals)
            {
                if (interval.Item1 <= value && interval.Item2 >= value)
                {
                    return $"{interval.Item1}-{interval.Item2}";
                }
            }

            return string.Empty;
        }
        public string GetGroupKey(string name)
        {
            // Extract the first 4 values from the "Name" field
            string[] values = name.Split(", ").Take(4).ToArray();
            return string.Join(", ", values);
        }
        public void StringToConfig(ref ConstructionConfigRequest constructionConfig, string configName)
        {
            string[] configAttributes = configName.Split(", ");
            if(configAttributes.Length != 4)
            {
                return ;
            }

            constructionConfig.ConstructionType = configAttributes[0].Equals("RoughConstruction") ? 
                                        ProjectConstructionType.RoughConstruction : 
                                        ProjectConstructionType.CompleteConstruction;

            string[] numOfFloors = configAttributes[1].Substring(0, configAttributes[1].Length - 7).Split('-');
            string[] area = configAttributes[2].Split('-');
            string[] tiledArea = configAttributes[3].Split('-');
            constructionConfig.NumOfFloorMin = int.Parse(numOfFloors[0]);
            constructionConfig.NumOfFloorMax = int.Parse(numOfFloors[1]);
            constructionConfig.AreaMin = int.Parse(area[0]);
            constructionConfig.AreaMax = int.Parse(area[1]);
            constructionConfig.TiledAreaMin = int.Parse(tiledArea[0]);
            constructionConfig.TiledAreaMax = int.Parse(tiledArea[1]);
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
            var allContructionConfig = await _constructionConfigRepository.GetAllDataByExpression(null);
            List<string>[] configsSet = await getThreeRange(allContructionConfig.Select(c => c.Name).ToList());
            var numOfFloorSearch = await GetSearchRange(configsSet[0], numOfFloor);
            sb.Append($", {numOfFloorSearch} Floors");
            var areaSearch = await GetSearchRange(configsSet[1], area);
            sb.Append($", {areaSearch}");

            var tiledAreaSearch = await GetSearchRange(configsSet[2], tiledArea);
            sb.Append($", {tiledAreaSearch}");

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
            sb.Append($", {$"{request.NumOfFloorMin}-{request.NumOfFloorMax}"} Floors");
            sb.Append($", {$"{request.AreaMin}-{request.AreaMax}"}");
            sb.Append($", {$"{request.TiledAreaMin}-{request.TiledAreaMax}"}");

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

            sb.Append($", {$"{request.NumOfFloorMin}-{request.NumOfFloorMax}"} Floors");
            sb.Append($", {$"{request.AreaMin}-{request.AreaMax}"}");
            sb.Append($", {$"{request.TiledAreaMin}-{request.TiledAreaMax}"}");

            return sb.ToString();
        }

    }
}