using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class ClassesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
