using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApiHarvestHub.Repositorys.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        Task<T> Save(T obj);
        Task Update(T obj);     
        Task<IEnumerable<T>> GetAll();
        Task<T> GetByID(int id);
        Task<T> GetByEmail(string email);
    }
}