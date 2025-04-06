
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebApiHarvestHub.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiHarvestHub.Model.Master;
using WebApiHarvestHub.Repositorys.Implements;
using WebApiHarvestHub.Repositorys.Interfaces;
using Microsoft.AspNetCore.Http;

namespace WebApiHarvestHub.Controllers.Master
{
   
    public interface ILoginController : IBaseApiController<LoginSave>
    {
        Task<IActionResult> GetByID(int id);
        Task<IActionResult> GetAllFilterBy(string statusName,
                                bool? isActive,
                                string sortBy, bool ascending);  
    }

    [Route("HarvestHubApi/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase, ILoginController
    {
        private readonly IConfiguration _config;
        private IHttpContextAccessor _httpContext;
        private IDapperContext _context;
        private IUnitOfWork _uow;

        public LoginController(IConfiguration config)
        {
            _config = config;
            _httpContext = (IHttpContextAccessor)new HttpContextAccessor();
        }



        [Authorize(Policy = "RequireAdmin")]
        [HttpPost("Save")]
        public async Task<IActionResult> Save(LoginSave obj)
        {       
            try
            {
                var results = new LoginSave();
                using (_context = new DapperContext())
                {
                    _uow = new UnitOfWork(_context);                 
                    results = await _uow.LoginRepo.Save(obj);
                }

                var st = StTrans.SetSt(200, 0, "OK");
                return Ok(new { Status = st, Results = results });

            }
            catch (System.Exception e)
            {
                var st = StTrans.SetSt(400, 0, e.Message);
                return BadRequest(new { Status = st });
            }
        }


        [Authorize(Policy = "RequireAdmin")]
        [HttpPut("update")]
        public async Task<IActionResult> Update(LoginSave obj)
        {         
            try
            {
                var results = new LoginSave();
                using (_context = new DapperContext())
                {
                    _uow = new UnitOfWork(_context);                
                    results = await _uow.LoginRepo.Save(obj);
                }

                var st = StTrans.SetSt(200, 0, "OK");
                return Ok(new { Status = st, Results = results });

            }
            catch (System.Exception e)
            {
                var st = StTrans.SetSt(400, 0, e.Message);
                return BadRequest(new { Status = st });
            }
        }

        [Authorize(Policy = "RequireAdmin")]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePassword cur)
        {         
            try
            {

                using (_context = new DapperContext())
                {
                    _uow = new UnitOfWork(_context);
                    await _uow.LoginRepo.ChangePassword(cur.UserId, cur.password_lama, cur.password_baru);
                }

                var st = StTrans.SetSt(200, 0, "Succes");
                return Ok(new { Status = st, Results = cur });
            }
            catch (System.Exception e)
            {
                var st = StTrans.SetSt(400, 0, e.Message);
                return Ok(new { Status = st });
            }
        }


        [Authorize(Policy = "RequireAdmin")]
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPassword cur)
        {     
            try
            {

                using (_context = new DapperContext())
                {
                    _uow = new UnitOfWork(_context);
                    await _uow.LoginRepo.ChangePassword(cur.UserId, cur.password_baru);
                }

                var st = StTrans.SetSt(200, 0, "Succes");
                return Ok(new { Status = st, Results = cur });
            }
            catch (System.Exception e)
            {
                var st = StTrans.SetSt(400, 0, e.Message);
                return Ok(new { Status = st });
            }
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginUser userDto)
        {
            try
            {
                var dt = new ListUser();
                using (_context = new DapperContext())
                {
                    _uow = new UnitOfWork(_context);
                    dt = await _uow.LoginRepo.Login(userDto.UserEmailAddress.ToLower(), userDto.UserPassword);
                    var ds = await _uow.LoginRepo.GetByEmail(userDto.UserEmailAddress);               
                    LoginList users = new LoginList
                    {
                        UserId = ds.UserId,
                        IsCustomerUser = ds.IsCustomerUser,
                        Username = ds.Username.ToLower(),
                        UserGivenName = ds.UserGivenName,
                        IsDeleted = false
                    };
               
                    if (dt == null)
                        return Unauthorized();
                
                    dt.token = GenerateJwtToken(dt);
                    
                }

                var st = StTrans.SetSt(200, 0, "User Berhasil Login");
                return Ok(new { Status = st, Results = dt });

            }
            catch (System.Exception e)
            {
                var st = StTrans.SetSt(400, 0, e.Message);
                return Ok(new { Status = st });

            }
        }

        [AllowAnonymous]
        [HttpGet("GetUserByID")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Login))]
        public async Task<IActionResult> GetByID(int id)
        {

            try
            {
                var results = new Login();
                using (_context = new DapperContext())
                {
                    _uow = new UnitOfWork(_context);
                    var output = await _uow.LoginRepo.GetByID(id);
                    results = output is null ? null : output;
                }

                if (results == null)
                {
                    var st = StTrans.SetSt(404, 0, "Not Found");
                    return NotFound(new { Status = st });
                }
                else
                {

                    var st = StTrans.SetSt(200, 0, "OK");
                    return Ok(new { Status = st, Results = results });
                }
            }
            catch (System.Exception e)
            {
                var st = StTrans.SetSt(400, 0, e.Message);
                return BadRequest(new { Status = st });
            }
        }
        
        
        private string GenerateJwtToken(ListUser role)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, role.Username),
            new Claim(ClaimTypes.GivenName, role.UserGivenName),
            new Claim(ClaimTypes.Email, role.UserEmailAddress)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }


        [Authorize(Policy = "RequireAdmin")]
        [HttpGet("get_all_filter_by")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Login>))]
        public async Task<IActionResult> GetAllFilterBy(string username, bool? IsDeleleted, string sortBy, bool ascending)
        {
            try
            {
                var results = new List<Login>();
                using (_context = new DapperContext())
                {
                    LoginListFilterBy obj = new LoginListFilterBy
                    {
                        Username = username,
                        IsDeleted = IsDeleleted
                    };

                    _uow = new UnitOfWork(_context);
                    var output = await _uow.LoginRepo.GetAllFilterBy(obj, sortBy, ascending);
                    results = output is null ? null : output.Select(obj => obj).ToList();
                }

                if (results.Count == 0)
                {
                    var st = StTrans.SetSt(404, 0, "Not Found");
                    return NotFound(new { Status = st });
                }
                else
                {
                    var st = StTrans.SetSt(200, 0, "OK");
                    return Ok(new { Status = st, Results = results });
                }
            }
            catch (System.Exception e)
            {
                var st = StTrans.SetSt(400, 0, e.Message);
                return BadRequest(new { Status = st });
            }
        }

        [Authorize(Policy = "RequireAdmin")]
        [HttpGet("get_all")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Login>))]
        public async Task<IActionResult> GetAll()
        {
             try
                {
                    var results = new List<Login>();
                    using (_context = new DapperContext())
                    {
                        _uow = new UnitOfWork(_context);
                        var output = await _uow.LoginRepo.GetAll();
                        results = output is null ? null : output.Select(obj => obj).ToList();
                    }

                    if (results.Count == 0)
                    {
                        var st = StTrans.SetSt(404, 0, "Not Found");
                        return NotFound(new { Status = st });
                    }
                    else
                    {
                        var st = StTrans.SetSt(200, 0, "OK");
                        return Ok(new { Status = st, Results = results });
                    }
                }
            catch (System.Exception e)
            {
                var st = StTrans.SetSt(400, 0, e.Message);

                return BadRequest(new { Status = st });
            }
        }

        [Authorize(Policy = "RequireAdmin")]
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(DeleteUser cur)
        {      
            try
            {

                using (_context = new DapperContext())
                {
                    _uow = new UnitOfWork(_context);
                    await _uow.LoginRepo.Delete(cur.UserId);
                }

                var st = StTrans.SetSt(200, 0, "Succes");
                return Ok(new { Status = st, Results = cur });
            }
            catch (System.Exception e)
            {
                var st = StTrans.SetSt(400, 0, e.Message);
                return Ok(new { Status = st });
            }
        }
    }

}
