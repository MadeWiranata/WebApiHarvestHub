using WebApiHarvestHub.Repositorys.implements;
using WebApiHarvestHub.Repositorys.Implements.Master;
using WebApiHarvestHub.Repositorys.Interfaces;
using WebApiHarvestHub.Repositorys.Interfaces.Master;

namespace WebApiHarvestHub.Repositorys.Implements
{
    public class UnitOfWork : IUnitOfWork
    {
        private IDapperContext _context;

        #region MASTER
        private ILoginRepo _loginRepo;
        private ISiteRepo _siteRepo;
        private ICropsRepo _cropsRepo;
        private IFieldRepo _fieldRepo;
        private ITaskTypesRepo _tasktypesRepo;
        private IWorkTaskRepo _worktaskRepo;

        #endregion

        public UnitOfWork(IDapperContext context)
        {
            _context = context;
        }

        #region MASTER
      
        public ILoginRepo LoginRepo
        {
            get { return _loginRepo ?? (_loginRepo = new LoginRepo(_context)); }
        }
        public ISiteRepo SiteRepo
        {
            get { return _siteRepo ?? (_siteRepo = new SiteRepo(_context)); }
        }
        public ICropsRepo CropsRepo
        {
            get { return _cropsRepo ?? (_cropsRepo = new CropsRepo(_context)); }
        }
        public IFieldRepo FieldRepo
        {
            get { return _fieldRepo ?? (_fieldRepo = new FieldRepo(_context)); }
        }
        public ITaskTypesRepo TaskTypesRepo
        {
            get { return _tasktypesRepo ?? (_tasktypesRepo = new TaskTypesRepo(_context)); }
        }
        public IWorkTaskRepo WorkTaskRepo
        {
            get { return _worktaskRepo ?? (_worktaskRepo = new WorkTaskRepo(_context)); }
        }
        #endregion      

    }
}