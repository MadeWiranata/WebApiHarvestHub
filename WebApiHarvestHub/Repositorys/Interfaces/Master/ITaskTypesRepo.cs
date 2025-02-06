using WebApiHarvestHub.Model.Master;

namespace WebApiHarvestHub.Repositorys.Interfaces.Master
{
    public interface ITaskTypesRepo : IBaseMasterRepository<TaskTypes>
    {
        Task<IEnumerable<TaskTypes>> GetAllFilterBy(TaskTypesListFilterBy obj, string sortBy, bool ascending);
    }
}
