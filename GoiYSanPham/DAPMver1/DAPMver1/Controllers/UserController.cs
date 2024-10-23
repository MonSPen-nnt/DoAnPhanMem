using Microsoft.AspNetCore.Mvc;

namespace DAPMver1.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
