using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Owner")]
    public class ResourceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
