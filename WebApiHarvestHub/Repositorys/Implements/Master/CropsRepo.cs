using System.Data;
using WebApiHarvestHub.Helper;
using WebApiHarvestHub.Model.Master;
using WebApiHarvestHub.Repositorys.Interfaces.Master;
using WebApiHarvestHub.Repositorys.Interfaces;
using Dapper;

namespace WebApiHarvestHub.Repositorys.Implements.Master
{
    public class CropsRepo : ICropsRepo
    {
        private const string SQL_TEMPLATE = @"SELECT A.CropId, A.CropCode, A.CreatedDate, A.CreatedUserId, A.ModifiedDate, A.ModifiedUserId, A.IsDeleted
                                              FROM [dbo].Crops A
                                              {WHERE}
                                              {ORDER BY}
                                              {OFFSET}";

        private IDapperContext _context;
        private IDbTransaction _transaction;

        private string _sql;

        public CropsRepo(IDapperContext context)
        {
            _context = context;
            _transaction = _context.transaction;
        }

        private string GetCondition(CropsListFilterBy obj)
        {
            string sql = string.Empty;

            if (obj.CropCode != null)
                sql = obj.CropCode.Length == 0 ? sql : SqlQuery.SetCondition(sql, string.Format("{0} LIKE '%{1}%'", "A.CropCode", obj.CropCode));

            sql = obj.IsDeleted == null ? sql : SqlQuery.SetCondition(sql, string.Format("{0} = {1}", "A.IsDeleted", (bool)obj.IsDeleted ? 1 : 0));

            return sql;
        }

        private string GetSortBy(string sortBy, bool ascending)
        {
            string sortByDefault = string.Empty;

            switch (sortBy)
            {
                case nameof(Crops.CropId):
                case nameof(Crops.CropCode):
                    sortBy = string.Format("A.{0}", sortBy);
                    break;
                default:
                    sortByDefault = "A.CropCode";
                    break;
            }

            if (sortByDefault.Length > 0)
                sortBy = string.Format("{0} {1}", sortByDefault, "ASC");
            else
                sortBy = string.Format("{0} {1}", sortBy, ascending ? "ASC" : "DESC");

            return string.Format("ORDER BY {0}", sortBy);
        }

        private async Task<IEnumerable<Crops>> MappingRecordToObject(string sql, object param = null)
        {
            IEnumerable<Crops> oList = await _context.db.QueryAsync<Crops>(sql, param);

            return oList;
        }

        public async Task<Crops> GetByID(int id)
        {
            Crops obj = new Crops();
            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE A.CropId = @id And A.IsDeleted = 0 ");
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

        public async Task<IEnumerable<Crops>> GetAll()
        {
            IEnumerable<Crops> oList = Enumerable.Empty<Crops>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", " WHERE A.IsDeleted = 0 ");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY A.CropCode");
                _sql = _sql.Replace("{OFFSET}", "");

                oList = await MappingRecordToObject(_sql);
            }
            catch (System.Exception ex)
            {
                throw new System.Exception(ex.Message);
            }

            return oList;
        }


        public async Task<IEnumerable<Crops>> GetAllFilterBy(CropsListFilterBy obj, string sortBy, bool ascending)
        {
            IEnumerable<Crops> oList = Enumerable.Empty<Crops>();

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

        public async Task<Crops> Save(Crops obj)
        {
            try
            {
                _context.BeginTransaction();
                _transaction = _context.transaction;
                var data = await _context.db.QueryFirstOrDefaultAsync<Crops>("[dbo].sp_crops_save", new
                {
                    CropId = obj.CropId,
                    CropCode = obj.CropCode,
                    CreatedDate = obj.CreatedDate,
                    CreatedUserId = obj.CreatedUserId,
                    ModifiedDate = obj.ModifiedDate,
                    ModifiedUserId = obj.ModifiedUserId,
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
        public async Task<bool> Delete(int Userid, int CropId)
        {
            await _context.db.QueryAsync("Update Crops set IsDeleted = 1, ModifiedUserId = @userid, ModifiedDate = @tgl Where CropId = @CropId",
                    new { tgl = DateTime.Now, userid = Userid, CropId = CropId });

            return true;
        }
    }
}
