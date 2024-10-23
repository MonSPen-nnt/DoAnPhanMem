using DAPMver1.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace DAPMver1.Controllers
{
    public class UserController : Controller
    {
        private readonly DapmTrangv1Context _context;
        public IActionResult Index(string email)
        {
            // Giả sử người dùng đăng nhập thành công
            HttpContext.Session.SetString("email", email);
            return RedirectToAction("Index", "Home");
        }
            //return View();
        }
        //public IActionResult Login(string email)
        //{
        //    // Giả sử người dùng đăng nhập thành công
        //    HttpContext.Session.SetString("email", email);
        //    return RedirectToAction("Index", "Home");
        //}

    
}
