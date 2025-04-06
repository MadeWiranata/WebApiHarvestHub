using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApiHarvestHub.Model.Master;
using WebApiHarvestHub.Models;
using WebApiHarvestHub.Repositorys.Implements;
using WebApiHarvestHub.Repositorys.Interfaces;

namespace WebApiHarvestHub.Controllers.Master
{
   
        public interface ISiteController : IBaseApiController<SiteSave>
        {
            Task<IActionResult> GetByID(int id);        
            Task<IActionResult> GetAllFilterBy(string FarmSiteName,
                                    bool? IsDeleted,
                                    string sortBy, bool ascending);        
        }

        [Route("HarvestHubApi/[controller]")]
        [ApiController]
        public class SiteController : ControllerBase, ISiteController
    {
            private readonly IConfiguration _config;
            private IHttpContextAccessor _httpContext;
            private IDapperContext _context;
            private IUnitOfWork _uow;

            public SiteController(IConfiguration config)
            {
                _config = config;
                _httpContext = (IHttpContextAccessor)new HttpContextAccessor();
            }

            [Authorize(Policy = "RequireAdmin")]
            [HttpGet("get_by_id")]
            [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Site))]
            public async Task<IActionResult> GetByID(int id)
            {

                try
                {
                    var results = new Site();
                    using (_context = new DapperContext())
                    {
                        _uow = new UnitOfWork(_context);
                        var output = await _uow.SiteRepo.GetByID(id);
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

            [Authorize(Policy = "RequireAdmin")]
            [HttpGet("get_all")]
            [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Site>))]
            public async Task<IActionResult> GetAll()
            {
                try
                {
                    var results = new List<Site>();
                    using (_context = new DapperContext())
                    {
                        _uow = new UnitOfWork(_context);
                        var output = await _uow.SiteRepo.GetAll();
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
        [HttpGet("get_all_filter_by")]
            [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Site>))]
            public async Task<IActionResult> GetAllFilterBy(string FarmSiteName, bool? IsDeleted, string sortBy, bool ascending)
            {
                try
                {
                    var results = new List<Site>();
                    using (_context = new DapperContext())
                    {
                        SiteListFilterBy obj = new SiteListFilterBy
                        {
                            FarmSiteName = FarmSiteName,
                            IsDeleted = IsDeleted
                        };

                        _uow = new UnitOfWork(_context);
                        var output = await _uow.SiteRepo.GetAllFilterBy(obj, sortBy, ascending);
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
            [HttpPost("save")]
            public async Task<IActionResult> Save(SiteSave obj)
            {           
                try
                {
                    var results = new SiteSave();
                    using (_context = new DapperContext())
                    {
                        _uow = new UnitOfWork(_context);                   
                        results = await _uow.SiteRepo.Save(obj);
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
            public async Task<IActionResult> Update(SiteSave obj)
            {            
                try
                {
                    var results = new SiteSave();
                    using (_context = new DapperContext())
                    {
                        _uow = new UnitOfWork(_context);                   
                        results = await _uow.SiteRepo.Save(obj);
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
            [HttpDelete("delete")]
            public async Task<IActionResult> Delete(DeleteSite del)
            {           
                try
                {
                    using (_context = new DapperContext())
                    {
                        _uow = new UnitOfWork(_context);
                        await _uow.SiteRepo.Delete(del.UserId, del.FarmSiteId);
                    }

                    var st = StTrans.SetSt(200, 0, "Succes");
                    return Ok(new { Status = st, Results = del });
                }
                catch (System.Exception e)
                {
                    var st = StTrans.SetSt(400, 0, e.Message);
                    return Ok(new { Status = st });
                }
            }
        }
}
