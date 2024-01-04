using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class ClassesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
