using WebApiHarvestHub.Model.Master;
using WebApiHarvestHub.Repositorys.Interfaces;

namespace WebApiHarvestHub.Repositorys.Interfaces.Master
{
    public interface ILoginRepo : IBaseShowRepository<Login>, IBaseSaveRepository<LoginSave>
    {
        Task<IEnumerable<Login>> GetAllFilterBy(LoginListFilterBy obj, string sortBy, bool ascending);    
        Task<bool> ChangePassword(int UserId, string passwordold, string passwordnew);
        Task<bool> ChangePassword(int UserId, string passwordnew);    
        Task<bool> Delete(int UserId);    
        Task<ListUser> Login(string Email, string password);
        Task<Login> GetByEmail(string userEmailAddress);
    }
}
