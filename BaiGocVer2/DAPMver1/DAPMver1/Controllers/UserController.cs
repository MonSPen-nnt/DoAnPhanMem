using DAPMver1.Data;
using Microsoft.AspNetCore.Mvc;

namespace BanTrangSuc.Controllers
{
    public class UserController : Controller
    {
        private readonly DapmTrangv1Context db;
        public IActionResult CreateUserDetails()
        {
               ViewBag.Email = TempData["email"] as string;
            string email = TempData["email"] as string ?? HttpContext.Session.GetString("Email");

            TaiKhoan taiKhoan = db.TaiKhoans.FirstOrDefault(tk => tk.Email == email);
            NguoiDung nguoiDung = new NguoiDung
            {
                MaTaiKhoan = taiKhoan.MaTaiKhoan,

            };
            return View(nguoiDung);
            
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