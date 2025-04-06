using Dapper.Contrib.Extensions;
using WebApiHarvestHub.Models.Master;

namespace WebApiHarvestHub.Model.Master
{
    public class Login:BaseModel
    {     
        public int UserId { get; set; }
        public bool IsCustomerUser { get; set; }
        public string Username { get; set; }
        public string UserPassword { get; set; }
        public string UserGivenName { get; set; }
        public string UserEmailAddress { get; set; }     
        public string UserStatus { get; set; }         
        public int FarmSiteId { get; set; }
        public string FarmSiteName { get; set; }

    }
    public class LoginSave : BaseModel
    {
        public int UserId { get; set; }
        public bool IsCustomerUser { get; set; }
        public string Username { get; set; }
        public string UserPassword { get; set; }
        public string UserGivenName { get; set; }
        public string UserEmailAddress { get; set; }
        public string UserStatus { get; set; }
        public int FarmSiteId { get; set; }
        
    }

    public class LoginListFilterBy
    {
        public int UserId { get; set; }
        public bool IsCustomerUser { get; set; }
        public string Username { get; set; }
        public string UserPassword { get; set; }
        public string UserGivenName { get; set; }
        public string UserEmailAddress { get; set; }
        public bool? IsDeleted { get; set; }
    }
    public class ChangePassword
    {
        public int UserId { get; set; }
        public string password_lama { get; set; }
        public string password_baru { get; set; }
    }
    public class ResetPassword
    {
        public int UserId { get; set; }   
        public string password_baru { get; set; }
    }
    public class LoginList:BaseModel
    {
        public int UserId { get; set; }
        public bool IsCustomerUser { get; set; }
        public string Username { get; set; }
        public string UserPassword { get; set; }
        public string UserGivenName { get; set; }
        public string UserEmailAddress { get; set; }
        public bool? IsDeleted { get; set; }
    }
    public class LoginUser
    {
        public string UserEmailAddress { get; set; }
        public string UserPassword { get; set; }

    }
    public class DeleteUser
    {
        public int UserId { get; set; }       

    }
    public class ListUser
    {
        public int UserId { get; set; }
        public bool IsCustomerUser { get; set; }
        public string Username { get; set; }
        public string UserPassword { get; set; }
        public string UserGivenName { get; set; }
        public string UserEmailAddress { get; set; }
        public string UserStatus { get; set; }
        public bool IsDeleted { get; set; }
        public int FarmSiteId { get; set; }
        [Write(false)]
        public string token { get; set; }
    }

}
