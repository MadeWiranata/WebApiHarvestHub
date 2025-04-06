using WebApiHarvestHub.Model.Master;

namespace WebApiHarvestHub.Repositorys.Interfaces.Master
{
    public interface ICropsRepo : IBaseShowRepository<Crops>, IBaseSaveRepository<Crops>
    {
        Task<IEnumerable<Crops>> GetAllFilterBy(CropsListFilterBy obj, string sortBy, bool ascending);
        Task<bool> Delete(int UserId, int CropId);
    }
}
