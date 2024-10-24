using DAPMver1.Data;
using Microsoft.AspNetCore.Mvc;

namespace BanTrangSuc.Controllers
{
    public class UserController : Controller
    {
        private readonly DapmTrangv1Context db;
        public IActionResult CreateUserDetails()
        {
              return View();
            
        }

        public IActionResult EditUser()
        {
            return View();
        }
        public IActionResult UserDetails()
        {
            return View();
        }
    }
}