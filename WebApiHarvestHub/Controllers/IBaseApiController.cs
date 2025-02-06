using Microsoft.AspNetCore.Mvc;

namespace WebApiHarvestHub.Controllers
{
    public interface IBaseApiController<T>
        where T : class
    {
        Task<IActionResult> Save(T obj);
        Task<IActionResult> Update(T obj);      
        Task<IActionResult> GetAll();
    }

}
