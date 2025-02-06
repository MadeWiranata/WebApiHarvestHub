using WebApiHarvestHub.Model.Master;

namespace WebApiHarvestHub.Repositorys.Interfaces.Master
{
    public interface ISiteRepo : IBaseMasterRepository<Site>
    {
        Task<IEnumerable<Site>> GetAllFilterBy(SiteListFilterBy obj, string sortBy, bool ascending);
    }
}
