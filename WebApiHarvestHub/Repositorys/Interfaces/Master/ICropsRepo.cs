using WebApiHarvestHub.Model.Master;

namespace WebApiHarvestHub.Repositorys.Interfaces.Master
{
    public interface ICropsRepo : IBaseMasterRepository<Crops>
    {
        Task<IEnumerable<Crops>> GetAllFilterBy(CropsListFilterBy obj, string sortBy, bool ascending);
    }
}
