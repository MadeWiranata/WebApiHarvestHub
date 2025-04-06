using System.Data;
using WebApiHarvestHub.Helper;
using WebApiHarvestHub.Model.Master;
using WebApiHarvestHub.Repositorys.Interfaces.Master;
using WebApiHarvestHub.Repositorys.Interfaces;
using Dapper;

namespace WebApiHarvestHub.Repositorys.Implements.Master
{
    public class TaskTypesRepo : ITaskTypesRepo
    {
        private const string SQL_TEMPLATE = @"SELECT A.WorkTaskTypeCode, A.WorkTaskSatusCode, A.CreatedDate, A.CreatedUserId, A.ModifiedDate, A.ModifiedUserId, A.IsDeleted
                                              FROM [dbo].WorkTaskTypes A
                                              {WHERE}
                                              {ORDER BY}
                                              {OFFSET}";

        private IDapperContext _context;
        private IDbTransaction _transaction;

        private string _sql;

        public TaskTypesRepo(IDapperContext context)
        {
            _context = context;
            _transaction = _context.transaction;
        }

        private string GetCondition(TaskTypesListFilterBy obj)
        {
            string sql = string.Empty;

            if (obj.WorkTaskSatusCode != null)
                sql = obj.WorkTaskSatusCode.Length == 0 ? sql : SqlQuery.SetCondition(sql, string.Format("{0} LIKE '%{1}%'", "A.WorkTaskSatusCode", obj.WorkTaskSatusCode));

            sql = obj.IsDeleted == null ? sql : SqlQuery.SetCondition(sql, string.Format("{0} = {1}", "A.IsDeleted", (bool)obj.IsDeleted ? 1 : 0));

            return sql;
        }

        private string GetSortBy(string sortBy, bool ascending)
        {
            string sortByDefault = string.Empty;

            switch (sortBy)
            {
                case nameof(TaskTypes.WorkTaskTypeCode):
                case nameof(TaskTypes.WorkTaskSatusCode):
                    sortBy = string.Format("A.{0}", sortBy);
                    break;
                default:
                    sortByDefault = "A.WorkTaskSatusCode";
                    break;
            }

            if (sortByDefault.Length > 0)
                sortBy = string.Format("{0} {1}", sortByDefault, "ASC");
            else
                sortBy = string.Format("{0} {1}", sortBy, ascending ? "ASC" : "DESC");

            return string.Format("ORDER BY {0}", sortBy);
        }

        private async Task<IEnumerable<TaskTypes>> MappingRecordToObject(string sql, object param = null)
        {
            IEnumerable<TaskTypes> oList = await _context.db.QueryAsync<TaskTypes>(sql, param);

            return oList;
        }

        public async Task<TaskTypes> GetByID(int id)
        {
            TaskTypes obj = new TaskTypes();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE A.WorkTaskTypeCode = @id And A.IsDeleted = 0 ");
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

        public async Task<IEnumerable<TaskTypes>> GetAll()
        {
            IEnumerable<TaskTypes> oList = Enumerable.Empty<TaskTypes>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", " WHERE A.IsDeleted = 0 ");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY A.WorkTaskSatusCode");
                _sql = _sql.Replace("{OFFSET}", "");

                oList = await MappingRecordToObject(_sql);
            }
            catch (System.Exception ex)
            {
                throw new System.Exception(ex.Message);
            }

            return oList;
        }


        public async Task<IEnumerable<TaskTypes>> GetAllFilterBy(TaskTypesListFilterBy obj, string sortBy, bool ascending)
        {
            IEnumerable<TaskTypes> oList = Enumerable.Empty<TaskTypes>();

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

        public async Task<TaskTypes> Save(TaskTypes obj)
        {
            try
            {
                _context.BeginTransaction();
                _transaction = _context.transaction;
                var data = await _context.db.QueryFirstOrDefaultAsync<TaskTypes>("[dbo].sp_tasktypes_save", new
                {
                    WorkTaskTypeCode = obj.WorkTaskTypeCode,
                    WorkTaskSatusCode = obj.WorkTaskSatusCode,
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
        public async Task<bool> Delete(int Userid, int WorkTaskTypeCode)
        {
            await _context.db.QueryAsync("Update WorkTaskTypes set IsDeleted = 1, ModifiedUserId = @userid, ModifiedDate = @tgl Where WorkTaskTypeCode = @WorkTaskTypeCode",
                    new { tgl = DateTime.Now, userid = Userid, WorkTaskTypeCode = WorkTaskTypeCode });

            return true;
        }
    }
}
