

using WebApiHarvestHub.Repositorys.Interfaces.Master;

namespace WebApiHarvestHub.Repositorys.Interfaces
{
    public interface IUnitOfWork
    {      
        #region MASTER
        
        ILoginRepo LoginRepo { get; }
        ISiteRepo SiteRepo { get; }
        ICropsRepo CropsRepo { get; }
        IFieldRepo FieldRepo { get; }
        ITaskTypesRepo TaskTypesRepo { get; }
        IWorkTaskRepo WorkTaskRepo { get; }
        #endregion       
    }
}