using Microsoft.AspNetCore.Mvc;

namespace BanTrangSuc.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult EditUser()
        {
            return View();
        }
        public IActionResult DetailsUser()
        {
            return View();
        }
    }
}
