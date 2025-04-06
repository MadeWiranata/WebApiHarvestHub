using Dapper;
using Dapper.Contrib.Extensions;
using System.Data;
using WebApiHarvestHub.Helper;
using WebApiHarvestHub.Model.Master;
using WebApiHarvestHub.Repositorys.Interfaces;
using WebApiHarvestHub.Repositorys.Interfaces.Master;

namespace WebApiHarvestHub.Repositorys.Implements.Master
{
    public class SiteRepo:ISiteRepo
    {
        private const string SQL_TEMPLATE = @"SELECT *
                                              FROM [dbo].vwFarmSites A
                                              {WHERE}
                                              {ORDER BY}
                                              {OFFSET}";

        private IDapperContext _context;
        private IDbTransaction _transaction;

        private string _sql;

        public SiteRepo(IDapperContext context)
        {
            _context = context;
            _transaction = _context.transaction;
        }

        private string GetCondition(SiteListFilterBy obj)
        {
            string sql = string.Empty;

            if (obj.FarmSiteName != null)
                sql = obj.FarmSiteName.Length == 0 ? sql : SqlQuery.SetCondition(sql, string.Format("{0} LIKE '%{1}%'", "A.FarmSiteName", obj.FarmSiteName));

            sql = obj.IsDeleted == null ? sql : SqlQuery.SetCondition(sql, string.Format("{0} = {1}", "A.IsDeleted", (bool)obj.IsDeleted ? 1 : 0));

            return sql;
        }

        private string GetSortBy(string sortBy, bool ascending)
        {
            string sortByDefault = string.Empty;

            switch (sortBy)
            {
                case nameof(Site.FarmSiteId):
                case nameof(Site.FarmSiteName):
                    sortBy = string.Format("A.{0}", sortBy);
                    break;
                default:
                    sortByDefault = "A.FarmSiteName";
                    break;
            }

            if (sortByDefault.Length > 0)
                sortBy = string.Format("{0} {1}", sortByDefault, "ASC");
            else
                sortBy = string.Format("{0} {1}", sortBy, ascending ? "ASC" : "DESC");

            return string.Format("ORDER BY {0}", sortBy);
        }

        private async Task<IEnumerable<Site>> MappingRecordToObject(string sql, object param = null)
        {
            IEnumerable<Site> oList = await _context.db.QueryAsync<Site>(sql, param);

            return oList;
        }

        public async Task<Site> GetByID(int id)
        {
            Site obj = new Site();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE A.FarmSiteId = @id and A.IsDeleted = 0 ");
                _sql = _sql.Replace("{ORDER BY}", "");
                _sql = _sql.Replace("{OFFSET}", "");
                var oList = await MappingRecordToObject(_sql, new { id });
                obj = oList.SingleOrDefault();
            }
            catch (System.Exception ex)
            {
                throw new System.Exception(ex.Message);
            }

            return obj;
        }

        public async Task<IEnumerable<Site>> GetAll()
        {
            IEnumerable<Site> oList = Enumerable.Empty<Site>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", " WHERE A.IsDeleted = 0 ");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY A.FarmSiteName");
                _sql = _sql.Replace("{OFFSET}", "");

                oList = await MappingRecordToObject(_sql);
            }
            catch (System.Exception ex)
            {
                throw new System.Exception(ex.Message);
            }

            return oList;
        }

      
        public async Task<IEnumerable<Site>> GetAllFilterBy(SiteListFilterBy obj, string sortBy, bool ascending)
        {
            IEnumerable<Site> oList = Enumerable.Empty<Site>();

            try
            {
                string param = GetCondition(obj);
                string condition = param.Length == 0 ? "" : string.Format("WHERE {0}", param);

                sortBy = GetSortBy(sortBy, ascending);

                _sql = SQL_TEMPLATE.Replace("{WHERE}", condition);
                _sql = _sql.Replace("{ORDER BY}", sortBy);
                _sql = _sql.Replace("{OFFSET}", "");

                oList = await MappingRecordToObject(_sql);
            }
            catch (System.Exception ex)
            {
                throw new System.Exception(ex.Message);
            }

            return oList;
        }     

        public async Task<SiteSave> Save(SiteSave obj)
        {
            try
            {
                _context.BeginTransaction();
                _transaction = _context.transaction;
                var data = await _context.db.QueryFirstOrDefaultAsync<SiteSave>("[dbo].sp_site_save", new
                {
                    FarmSiteId = obj.FarmSiteId,
                    FarmSiteName = obj.FarmSiteName,
                    CreatedDate = obj.CreatedDate,
                    CreatedUserId = obj.CreatedUserId,
                    ModifiedDate = obj.ModifiedDate,
                    ModifiedUserId = obj.ModifiedUserId,
                    DefaultPrimaryCropId = obj.DefaultPrimaryCropId,
                    IsDeleted = obj.IsDeleted

                }, commandType: CommandType.StoredProcedure, transaction: _transaction);
                _context.Commit();

                return data;
            }
            catch (System.Exception)
            {
                _context.Rollback();
                throw;
            }
        }
     
        public async Task<bool> Delete(int Userid, int FarmSiteId)
        {
            await _context.db.QueryAsync("Update FarmSites set IsDeleted = 1, ModifiedUserId = @userid, ModifiedDate = @tgl Where FarmSiteId = @FarmSiteId",
                    new { tgl = DateTime.Now, userid = Userid, FarmSiteId = FarmSiteId });

            return true;
        }
    }
}
