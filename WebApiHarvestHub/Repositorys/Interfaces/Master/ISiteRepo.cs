using WebApiHarvestHub.Model.Master;

namespace WebApiHarvestHub.Repositorys.Interfaces.Master
{
    public interface ISiteRepo : IBaseShowRepository<Site>, IBaseSaveRepository<SiteSave>
    {
        Task<IEnumerable<Site>> GetAllFilterBy(SiteListFilterBy obj, string sortBy, bool ascending);
        Task<bool> Delete(int UserId, int FarmSiteId);
    }
}
