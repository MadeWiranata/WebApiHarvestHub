namespace WebApiHarvestHub.Repositorys.Interfaces
{
    public interface IBaseShowRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> GetByID(int id);    
    }
}
