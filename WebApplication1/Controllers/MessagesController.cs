using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class MessagesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
