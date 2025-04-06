using WebApiHarvestHub.Model.Master;

namespace WebApiHarvestHub.Repositorys.Interfaces.Master
{
    public interface IFieldRepo : IBaseShowRepository<Field>, IBaseSaveRepository<FieldSave>
    {
        Task<IEnumerable<Field>> GetAllFilterBy(FieldListFilterBy obj, string sortBy, bool ascending);
        Task<bool> Delete(int UserId, int FarmFieldId);
    }
}
