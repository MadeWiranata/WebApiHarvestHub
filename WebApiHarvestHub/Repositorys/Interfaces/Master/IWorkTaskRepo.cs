using WebApiHarvestHub.Model.Master;

namespace WebApiHarvestHub.Repositorys.Interfaces.Master
{  
    public interface IWorkTaskRepo : IBaseShowRepository<WorkTask>, IBaseSaveRepository<WorkTaskSave>
    {
        Task<IEnumerable<WorkTask>> GetAllFilterBy(WorkTaskListFilterBy obj, string sortBy, bool ascending);
    }
}
