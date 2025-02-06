using Dapper;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using WebApiHarvestHub.Repositorys.Interfaces;

namespace WebApiHarvestHub.Repositorys.Implements
{
    public class DapperContext : IDapperContext
    {
        private IDbConnection _db;
        private IDbTransaction _transaction;
        private readonly string _providerName;
        private readonly string _connectionString;

        public DapperContext()
        {
            _providerName = "Microsoft.Data.SqlClient";
            _connectionString = "Data Source=127.0.0.1; Initial Catalog=db_madewiranata; User ID=sa; Password=123Defi@; MultipleActiveResultSets=True;";

            if (_db == null)
            {
                _db = GetOpenConnection(_providerName, _connectionString);
            }
        }

        private IDbConnection GetOpenConnection(string providerName, string connectionString)
        {
            DbConnection conn = null;

            try
            {
          
                SqlClientFactory provider = SqlClientFactory.Instance;
                conn = provider.CreateConnection();
                conn.ConnectionString = connectionString;
                conn.Open();
            }
            catch
            {
            }

            return conn;
        }

        public IDbConnection db
        {
            get { return _db ?? (_db = GetOpenConnection(_providerName, _connectionString)); }
        }

        public IDbTransaction transaction
        {
            get { return _transaction; }
        }

        public void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            if (_transaction == null)
                _transaction = _db.BeginTransaction(isolationLevel);
        }

        public void Commit()
        {
            if (_transaction != null)
            {
                _transaction.Commit();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            if (_db != null)
            {
                try
                {
                    if (_db.State != ConnectionState.Closed)
                    {
                        if (_transaction != null)
                        {
                            _transaction.Rollback();
                        }

                        _db.Close();
                    }
                }
                finally
                {
                    _db.Dispose();
                }
            }

            GC.SuppressFinalize(this);
        }

        public string GetGUID()
        {
            var result = string.Empty;

            try
            {
                result = Guid.NewGuid().ToString();
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }

        public void Rollback()
        {
            if (_transaction != null)
            {
                _transaction.Rollback();
                _transaction = null;
            }
        }

        public int GetPagesCount(string sql, int pageSize, object param = null)
        {
            var pagesCount = 0;

            try
            {
                var recordCount = _db.QuerySingleOrDefault<int>(sql, param);
                pagesCount = (int)Math.Ceiling(recordCount / (decimal)pageSize);
            }
            catch (Exception)
            {               
                throw;
            }

            return pagesCount;
        }
    }
}