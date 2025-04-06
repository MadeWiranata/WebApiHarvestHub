using System.Data;
using WebApiHarvestHub.Helper;
using WebApiHarvestHub.Model.Master;
using WebApiHarvestHub.Repositorys.Interfaces.Master;
using WebApiHarvestHub.Repositorys.Interfaces;
using Dapper;

namespace WebApiHarvestHub.Repositorys.Implements.Master
{ 
    public class FieldRepo : IFieldRepo
    {
        private const string SQL_TEMPLATE = @"SELECT * 
                                              FROM [dbo].vwFarmFields A
                                              {WHERE}
                                              {ORDER BY}
                                              {OFFSET}";

        private IDapperContext _context;
        private IDbTransaction _transaction;

        private string _sql;

        public FieldRepo(IDapperContext context)
        {
            _context = context;
            _transaction = _context.transaction;
        }

        private string GetCondition(FieldListFilterBy obj)
        {
            string sql = string.Empty;

            if (obj.FarmFieldName != null)
                sql = obj.FarmFieldName.Length == 0 ? sql : SqlQuery.SetCondition(sql, string.Format("{0} LIKE '%{1}%'", "A.FarmFieldName", obj.FarmFieldName));

            sql = obj.IsDeleted == null ? sql : SqlQuery.SetCondition(sql, string.Format("{0} = {1}", "A.IsDeleted", (bool)obj.IsDeleted ? 1 : 0));

            return sql;
        }

        private string GetSortBy(string sortBy, bool ascending)
        {
            string sortByDefault = string.Empty;

            switch (sortBy)
            {
                case nameof(Field.FarmFieldId):
                case nameof(Field.FarmFieldName):
                    sortBy = string.Format("A.{0}", sortBy);
                    break;
                default:
                    sortByDefault = "A.FarmFieldName";
                    break;
            }

            if (sortByDefault.Length > 0)
                sortBy = string.Format("{0} {1}", sortByDefault, "ASC");
            else
                sortBy = string.Format("{0} {1}", sortBy, ascending ? "ASC" : "DESC");

            return string.Format("ORDER BY {0}", sortBy);
        }

        private async Task<IEnumerable<Field>> MappingRecordToObject(string sql, object param = null)
        {
            IEnumerable<Field> oList = await _context.db.QueryAsync<Field>(sql, param);

            return oList;
        }

        public async Task<Field> GetByID(int id)
        {
            Field obj = new Field();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE A.FarmFieldId = @id and A.IsDeleted = 0 ");
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

        public async Task<IEnumerable<Field>> GetAll()
        {
            IEnumerable<Field> oList = Enumerable.Empty<Field>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", " WHERE A.IsDeleted = 0 ");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY A.FarmFieldName");
                _sql = _sql.Replace("{OFFSET}", "");

                oList = await MappingRecordToObject(_sql);
            }
            catch (System.Exception ex)
            {
                throw new System.Exception(ex.Message);
            }

            return oList;
        }


        public async Task<IEnumerable<Field>> GetAllFilterBy(FieldListFilterBy obj, string sortBy, bool ascending)
        {
            IEnumerable<Field> oList = Enumerable.Empty<Field>();

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

        public async Task<FieldSave> Save(FieldSave obj)
        {
            try
            {
                _context.BeginTransaction();
                _transaction = _context.transaction;
                var data = await _context.db.QueryFirstOrDefaultAsync<FieldSave>("[dbo].sp_field_save", new
                {
                    FarmSiteId = obj.FarmSiteId,
                    FarmFieldId = obj.FarmFieldId,
                    FarmFieldName = obj.FarmFieldName,
                    FarmFieldCode = obj.FarmFieldCode,
                    RowWidth = obj.RowWidth,
                    FarmFieldRowDirection = obj.FarmFieldRowDirection,
                    FarmFieldColorHexCode = obj.FarmFieldColorHexCode,
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
        public async Task<bool> Delete(int Userid, int FarmFieldId)
        {
            await _context.db.QueryAsync("Update FarmFields set IsDeleted = 1, ModifiedUserId = @userid, ModifiedDate = @tgl Where FarmFieldId = @FarmFieldId",
                    new { tgl = DateTime.Now, userid = Userid, FarmFieldId = FarmFieldId });

            return true;
        }
    }
}
