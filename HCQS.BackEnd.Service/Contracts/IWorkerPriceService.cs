using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.Request;

namespace HCQS.BackEnd.Service.Contracts
{
    public interface IWorkerPriceService
    {
        Task<AppActionResult> GetAll();

        Task<AppActionResult> GetById(Guid id);

        Task<AppActionResult> GetByPositionName(string positionName);

        Task<AppActionResult> CreateWokerPrice(WorkerPriceRequest worker);

        Task<AppActionResult> UpdateWorkerPrice(WorkerPriceRequest worker);

        Task<AppActionResult> DeleteWorkerPriceById(Guid id);
    }
}