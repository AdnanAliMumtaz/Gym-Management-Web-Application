using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class SettingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
