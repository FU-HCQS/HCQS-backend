namespace HCQS.BackEnd.DAL.Contracts
{
    public interface IUnitOfWork
    {
        Task SaveChangeAsync();
         void Dispose();

    }
}