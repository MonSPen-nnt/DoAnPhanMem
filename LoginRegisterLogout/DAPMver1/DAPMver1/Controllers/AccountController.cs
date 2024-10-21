using Microsoft.AspNetCore.Mvc;
using DAPMver1.Data;
using System.Linq;
using DAPMver1.Models;

public class AccountController : Controller
{
    private readonly DapmTrangv1Context _context;

    public AccountController(DapmTrangv1Context context)
    {
        _context = context;
    }

    // Hiển thị form đăng ký

    public IActionResult Register()
    {
        return View("~/Views/Account/Register.cshtml");
    }


    // Xử lý khi người dùng submit form
    [HttpPost]
    public IActionResult Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Kiểm tra xem Email đã tồn tại chưa
            var existingUser = _context.TaiKhoans.FirstOrDefault(u => u.Email == model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Email đã tồn tại.");
                return View(model);
            }

            // Tạo tài khoản mới
            var taiKhoan = new TaiKhoan
            {
                Email = model.Email,
                MatKhau = model.MatKhau, // Bạn có thể sử dụng hashing cho mật khẩu
                VaiTro = false // Mặc định vai trò người dùng
            };

            _context.TaiKhoans.Add(taiKhoan);
            _context.SaveChanges();

            return RedirectToAction("Login", "Account");
        }

        return View(model);
    }
    //=========================================
   
    public IActionResult Login()
    {
        return View();
    }

    // Xử lý khi người dùng submit form đăng nhập
    [HttpPost]
    public IActionResult Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Kiểm tra xem tài khoản có tồn tại hay không
            var user = _context.TaiKhoans.FirstOrDefault(u => u.Email == model.Email && u.MatKhau == model.MatKhau);

            if (user != null)
            {
                // Đăng nhập thành công
                // Có thể lưu thông tin vào session hoặc cookie
                     return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Sai thông tin đăng nhập.");
            }
        }

        return View(model);
    }

    // Đăng xuất
    public IActionResult Logout()
    {
        return RedirectToAction("Login", "Account");
    }
}
