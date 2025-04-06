using WebApiHarvestHub.Model.Master;

namespace WebApiHarvestHub.Repositorys.Interfaces.Master
{
    public interface ITaskTypesRepo : IBaseShowRepository<TaskTypes>, IBaseSaveRepository<TaskTypes>
    {
        Task<IEnumerable<TaskTypes>> GetAllFilterBy(TaskTypesListFilterBy obj, string sortBy, bool ascending);
        Task<bool> Delete(int UserId, int WorkTaskTypeCode);
    }
}
