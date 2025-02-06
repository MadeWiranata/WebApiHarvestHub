using WebApiHarvestHub.Repositorys.Interfaces;

namespace WebApiHarvestHub.Repositorys.Interfaces
{
    public interface IBaseMasterRepository<T> : IBaseRepository<T> where T : class
    {
        Task<T> GetByID(int id);
        Task<IEnumerable<T>> GetByName(string name);
        Task<IEnumerable<T>> GetByStatus(bool isActive);
    }
}
