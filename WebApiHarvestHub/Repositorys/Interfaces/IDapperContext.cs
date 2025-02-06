using System;
using System.Data;

namespace WebApiHarvestHub.Repositorys.Interfaces
{
    public interface IDapperContext : IDisposable
    {
        IDbConnection db { get; }
        IDbTransaction transaction { get; }
        void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        void Commit();
        void Rollback();
        string GetGUID();
       int GetPagesCount(string sql, int pageSize, object param = null);
    }
}