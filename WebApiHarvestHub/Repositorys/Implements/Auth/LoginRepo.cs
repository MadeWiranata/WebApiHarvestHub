using Dapper;
using Dapper.Contrib.Extensions;
using System.Data;
using WebApiHarvestHub.Helper;
using WebApiHarvestHub.Model.Master;
using WebApiHarvestHub.Repositorys.Interfaces;
using WebApiHarvestHub.Repositorys.Interfaces.Master;


namespace WebApiHarvestHub.Repositorys.Implements.Master
{
    public class LoginRepo : ILoginRepo
    {
        private const string SQL_TEMPLATE = @"SELECT *
                                              FROM [dbo].vwUsers A
                                              {WHERE}
                                              {ORDER BY}
                                              {OFFSET}";

        
        private IDapperContext _context;
        private IDbTransaction _transaction;

        private string _sql;

        public LoginRepo(IDapperContext context)
        {
            _context = context;
            _transaction = _context.transaction;
        }

        private string GetCondition(LoginListFilterBy obj)
        {
            string sql = string.Empty;

            if (obj.Username != null)
                sql = obj.Username.Length == 0 ? sql : SqlQuery.SetCondition(sql, string.Format("{0} LIKE '%{1}%'", "A.Username", obj.Username));

            sql = obj.IsDeleted == null ? sql : SqlQuery.SetCondition(sql, string.Format("{0} = {1}", "A.IsDeleted", (bool)obj.IsDeleted ? 1 : 0));

            return sql;
        }

        private string GetSortBy(string sortBy, bool ascending)
        {
            string sortByDefault = string.Empty;

            switch (sortBy)
            {
                case nameof(LoginListFilterBy.UserId):
                case nameof(LoginListFilterBy.Username):
                case nameof(LoginListFilterBy.UserGivenName):
                    sortBy = string.Format("A.{0}", sortBy);
                    break;
                default:
                    sortByDefault = "A.Username";
                    break;
            }

            if (sortByDefault.Length > 0)
                sortBy = string.Format("{0} {1}", sortByDefault, "ASC");
            else
                sortBy = string.Format("{0} {1}", sortBy, ascending ? "ASC" : "DESC");

            return string.Format("ORDER BY {0}", sortBy);
        }

        private async Task<IEnumerable<Login>> MappingRecordToObject(string sql, object param = null)
        {
            IEnumerable<Login> oList = await _context.db.QueryAsync<Login>(sql, param);

            return oList;
        }

        public async Task<Login> GetByID(int UserId)
        {
            Login obj = new Login();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE A.UserId = @UserId And A.IsDeleted = 0 ");
                _sql = _sql.Replace("{ORDER BY}", "");
                _sql = _sql.Replace("{OFFSET}", "");
                var oList = await MappingRecordToObject(_sql, new { UserId });
                obj = oList.SingleOrDefault();
            }
            catch (System.Exception ex)
            {
                throw new System.Exception(ex.Message);
            }

            return obj;
        }

     
        public async Task<IEnumerable<Login>> GetAllFilterBy(LoginListFilterBy obj, string sortBy, bool ascending)
        {
            IEnumerable<Login> oList = Enumerable.Empty<Login>();

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

        public async Task<LoginSave> Save(LoginSave obj)
        {
            try
            {
                _context.BeginTransaction();
                _transaction = _context.transaction;
                var pass = CreatePasswordHash(obj.UserPassword, obj.UserPassword);
                var data = await _context.db.QueryFirstOrDefaultAsync<LoginSave>("[dbo].sp_login_save", new
                {
                    UserId = obj.UserId,
                    IsCustomerUser = obj.IsCustomerUser,
                    Username = obj.Username,
                    UserPassword = pass,
                    UserGivenName = obj.UserGivenName,
                    UserEmailAddress = obj.UserEmailAddress,
                    CreatedDate = obj.CreatedDate,
                    CreatedUserId = obj.CreatedUserId,
                    ModifiedDate = obj.ModifiedDate,
                    ModifiedUserId = obj.ModifiedUserId,
                    UserStatus = obj.UserStatus,
                    IsDeleted = obj.IsDeleted,
                    FarmSiteId = obj.FarmSiteId
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

        public async Task<bool> ChangePassword(int UserId, string passwordold, string passwordnew)
        {
            var user = await _context.db.QueryFirstOrDefaultAsync<Login>("Select UserId,IsCustomerUser,Username,UserPassword,UserGivenName,UserEmailAddress, IsDeleted FROM Users WHERE UserId = @userid", new { userid = UserId });
            var psold = CreatePasswordHash(passwordold, passwordold);
            if (user == null || user.UserPassword != psold)
                throw new Exception("Password Lama Salah !!!");

            var psnew = CreatePasswordHash(passwordnew, passwordnew);
            await _context.db.QueryAsync("Update Users SET UserPassword = @psnew, ModifiedUserId = @userid, ModifiedDate = @tgl Where UserId = @userid",
                    new { psnew = psnew, tgl = DateTime.Now, userid = UserId });

            return true;
        }


        public async Task<bool> ChangePassword(int UserId, string passwordnew)
        {        
            var psnew = CreatePasswordHash(passwordnew, passwordnew);
            await _context.db.QueryAsync("Update Users SET UserPassword = @psnew, ModifiedUserId = @userid, ModifiedDate = @tgl Where UserId = @userid",
                    new { psnew = psnew, tgl = DateTime.Now, userid = UserId });

            return true;
        }

        public async Task<bool> Delete(int UserId)
        {
            await _context.db.QueryAsync("Update Users set IsDeleted = 1, ModifiedUserId = @userid, ModifiedDate = @tgl Where UserId = @userid",
                    new { tgl = DateTime.Now, userid = UserId });

            return true;
        }     

        private bool VerifyPassword(string pass, string password)
        {
            var passUser = CreatePasswordHash(pass, pass);

            if (passUser != password)
                return false;

            return true;
        }
   
        private string CreatePasswordHash(string plainText, string key)
        {
            if (key.Length > 0)
                plainText += key;

            var x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            var bs = System.Text.Encoding.UTF8.GetBytes(plainText);
            bs = x.ComputeHash(bs);

            var s = new System.Text.StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            string password = s.ToString();
            return password;
        }


        public async Task<ListUser> Login(string UserEmailAddress, string password)
        {
            var user = await _context.db.QueryFirstOrDefaultAsync<ListUser>("Select  UserId,IsCustomerUser,Username,UserPassword,UserGivenName,UserEmailAddress, IsDeleted FROM Users where UserEmailAddress = @UserEmailAddress ", new { UserEmailAddress = UserEmailAddress });
            if (user == null)
                throw new Exception("User Tidak terdaftar !!!");

            if (!VerifyPassword(password, user.UserPassword))
                throw new Exception("Password Salah, Silahkan Periksa Password Anda !!!");
           

            return user;
        }

        public async Task<IEnumerable<Login>> GetAll()
        {
            IEnumerable<Login> oList = Enumerable.Empty<Login>();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "  WHERE A.IsDeleted = 0 ");
                _sql = _sql.Replace("{ORDER BY}", "ORDER BY A.Username");
                _sql = _sql.Replace("{OFFSET}", "");

                oList = await MappingRecordToObject(_sql);
            }
            catch (System.Exception ex)
            {
                throw new System.Exception(ex.Message);
            }

            return oList;
        }

        public async Task<Login> GetByEmail(string email)
        {
            Login obj = new Login();

            try
            {
                _sql = SQL_TEMPLATE.Replace("{WHERE}", "WHERE A.UserEmailAddress = @email");
                _sql = _sql.Replace("{ORDER BY}", "");
                _sql = _sql.Replace("{OFFSET}", "");
                var oList = await MappingRecordToObject(_sql, new { email });
                obj = oList.SingleOrDefault();
            }
            catch (System.Exception ex)
            {
                throw new System.Exception(ex.Message);
            }

            return obj;
        }
    }
}
