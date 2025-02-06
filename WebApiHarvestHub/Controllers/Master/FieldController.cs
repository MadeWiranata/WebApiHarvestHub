using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApiHarvestHub.Model.Master;
using WebApiHarvestHub.Models;
using WebApiHarvestHub.Repositorys.Implements;
using WebApiHarvestHub.Repositorys.Interfaces;

namespace WebApiHarvestHub.Controllers.Master
{
    public interface IFieldController : IBaseApiController<FieldSave>
    {
        Task<IActionResult> GetByID(int id);
        Task<IActionResult> GetAllFilterBy(string FarmFieldName,
                                bool? IsDeleted,
                                string sortBy, bool ascending);
    }

    [Route("HarvestHubApi/[controller]")]
    [ApiController]
    public class FieldController : ControllerBase, IFieldController
    {
        private readonly IConfiguration _config;
        private IHttpContextAccessor _httpContext;
        private IDapperContext _context;
        private IUnitOfWork _uow;

        public FieldController(IConfiguration config)
        {
            _config = config;
            _httpContext = (IHttpContextAccessor)new HttpContextAccessor();
        }

        [Authorize(Policy = "RequireAdmin")]
        [HttpGet("get_by_id")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Field))]
        public async Task<IActionResult> GetByID(int id)
        {

            try
            {
                var results = new Field();
                using (_context = new DapperContext())
                {
                    _uow = new UnitOfWork(_context);
                    var output = await _uow.FieldRepo.GetByID(id);
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Field>))]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var results = new List<Field>();
                using (_context = new DapperContext())
                {
                    _uow = new UnitOfWork(_context);
                    var output = await _uow.FieldRepo.GetAll();
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

        [AllowAnonymous]
        [HttpGet("get_all_filter_by")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Field>))]
        public async Task<IActionResult> GetAllFilterBy(string FarmFieldName, bool? IsDeleted, string sortBy, bool ascending)
        {
            try
            {
                var results = new List<Field>();
                using (_context = new DapperContext())
                {
                    FieldListFilterBy obj = new FieldListFilterBy
                    {
                        FarmFieldName = FarmFieldName,
                        IsDeleted = IsDeleted
                    };

                    _uow = new UnitOfWork(_context);
                    var output = await _uow.FieldRepo.GetAllFilterBy(obj, sortBy, ascending);
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
        public async Task<IActionResult> Save(FieldSave obj)
        {
            try
            {
                var results = new FieldSave();
                using (_context = new DapperContext())
                {
                    _uow = new UnitOfWork(_context);
                    results = await _uow.FieldRepo.Save(obj);
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
        [HttpPost("update")]
        public async Task<IActionResult> Update(FieldSave obj)
        {
            string userby = _httpContext.HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            try
            {
                var results = new FieldSave();
                using (_context = new DapperContext())
                {
                    _uow = new UnitOfWork(_context);
                    results = await _uow.FieldRepo.Save(obj);
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
    }
}
