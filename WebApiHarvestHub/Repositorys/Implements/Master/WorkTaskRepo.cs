using System.Data;
using WebApiHarvestHub.Helper;
using WebApiHarvestHub.Model.Master;
using WebApiHarvestHub.Repositorys.Interfaces.Master;
using WebApiHarvestHub.Repositorys.Interfaces;
using Dapper;

namespace WebApiHarvestHub.Repositorys.Implements.Master
{
    public class WorkTaskRepo : IWorkTaskRepo
    {
        private const string SQL_TEMPLATE = @"SELECT * 
                                              FROM [dbo].vwWorkTask A
                                              {WHERE}
                                              {ORDER BY}
                                              {OFFSET}";

        private IDapperContext _context;
        private IDbTransaction _transaction;

        private string _sql;

        public WorkTaskRepo(IDapperContext context)
        {
            _context = context;
            _transaction = _context.transaction;
        }

        private string GetCondition(WorkTaskListFilterBy obj)
        {
            string sql = string.Empty;

            if (obj.FarmFieldCode != null)
                sql = obj.FarmFieldCode.Length == 0 ? sql : SqlQuery.SetCondition(sql, string.Format("{0} LIKE '%{1}%'", "A.FarmFieldCode", obj.FarmFieldCode));

            sql = obj.IsDeleted == null ? sql : SqlQuery.SetCondition(sql, string.Format("{0} = {1}", "A.IsDeleted", (bool)obj.IsDeleted ? 1 : 0));

            return sql;
        }

        private string GetSortBy(string sortBy, bool ascending)
        {
            string sortByDefault = string.Empty;

            switch (sortBy)
            {
                case nameof(WorkTask.WorkTaskId):
                case nameof(WorkTask.WorkTaskTypeCode):
                    sortBy = string.Format("A.{0}", sortBy);
                    break;
                default:
                    sortByDefault = "A.FarmFieldCode";
                    break;
            }

            if (sortByDefault.Length > 0)
                sortBy = string.Format("{0} {1}", sortByDefault, "ASC");
            else
                sortBy = string.Format("{0} {1}", sortBy, ascending ? "ASC" : "DESC");

            return string.Format("ORDER BY {0}", sortBy);
        }

        private async Task<IEnumerable<WorkTask>> MappingRecordToObject(string sql, object param = null)
        {
            IEnumerable<WorkTask> oList = await _context.db.QueryAsync<WorkTask>(sql, param);

            return oList;
        }

        public async Task<WorkTask> GetByID(int id)
        {
            WorkTask obj = new WorkTask();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE A.WorkTaskId = @id");
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

        public async Task<IEnumerable<WorkTask>> GetByName(string name)
        {
            IEnumerable<WorkTask> oList = Enumerable.Empty<WorkTask>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE A.FarmFieldCode = @name");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY A.FarmFieldCode");
                _sql = _sql.Replace("{OFFSET}", "");

                oList = await MappingRecordToObject(_sql, new { name });
            }
            catch (System.Exception ex)
            {
                throw new System.Exception(ex.Message);
            }

            return oList;
        }

        public async Task<IEnumerable<WorkTask>> GetByStatus(bool IsDeleted)
        {
            IEnumerable<WorkTask> oList = Enumerable.Empty<WorkTask>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE A.IsDeleted = @IsDeleted");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY A.FarmFieldCode");
                _sql = _sql.Replace("{OFFSET}", "");

                oList = await MappingRecordToObject(_sql, new { IsDeleted });
            }
            catch (System.Exception ex)
            {
                throw new System.Exception(ex.Message);
            }

            return oList;
        }

        public async Task<IEnumerable<WorkTask>> GetAll()
        {
            IEnumerable<WorkTask> oList = Enumerable.Empty<WorkTask>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY A.FarmFieldCode");
                _sql = _sql.Replace("{OFFSET}", "");

                oList = await MappingRecordToObject(_sql);
            }
            catch (System.Exception ex)
            {
                throw new System.Exception(ex.Message);
            }

            return oList;
        }


        public async Task<IEnumerable<WorkTask>> GetAllFilterBy(WorkTaskListFilterBy obj, string sortBy, bool ascending)
        {
            IEnumerable<WorkTask> oList = Enumerable.Empty<WorkTask>();

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

        public async Task<WorkTaskSave> Save(WorkTaskSave obj)
        {
            try
            {
                _context.BeginTransaction();
                _transaction = _context.transaction;
                var data = await _context.db.QueryFirstOrDefaultAsync<WorkTaskSave>("[dbo].sp_worktask_save", new
                {
                    WorkTaskId = obj.WorkTaskId,
                    FarmFieldId = obj.FarmFieldId,
                    WorkTaskTypeCode = obj.WorkTaskTypeCode,
                    StartedDate = obj.StartedDate,
                    CanceledDate = obj.CanceledDate,
                    DueDate = obj.DueDate,
                    CreatedDate = obj.CreatedDate,
                    CreatedUserId = obj.CreatedUserId,
                    ModifiedDate = obj.ModifiedDate,
                    ModifiedUserId = obj.ModifiedUserId,
                    IsDeleted = obj.IsDeleted,
                    IsCompleted = obj.IsCompleted,
                    IsStarted = obj.IsStarted,
                    IsCancelled = obj.IsCancelled

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
    }
}
