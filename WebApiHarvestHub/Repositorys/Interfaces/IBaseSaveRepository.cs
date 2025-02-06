namespace WebApiHarvestHub.Repositorys.Interfaces
{
    public interface IBaseSaveRepository<T> where T : class
    {
        Task<T> Save(T obj);
    }
}
