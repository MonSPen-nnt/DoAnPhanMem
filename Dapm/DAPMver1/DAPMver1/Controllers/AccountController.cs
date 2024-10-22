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
    [ValidateAntiForgeryToken]
    public IActionResult Login(string email, string matkhau)
    {
        if (ModelState.IsValid)
        {
            // Tìm kiếm tài khoản dựa trên email
            var check = _context.TaiKhoans.FirstOrDefault(s => s.Email == email);
            if (check == null || check.MatKhau != matkhau)
            {
                // Nếu không tìm thấy tài khoản hoặc mật khẩu sai
                ViewBag.error = "Sai email đăng nhập hoặc mật khẩu";
                return View();
            }

            // Kiểm tra thông tin người dùng
            NguoiDung nguoiDung = _context.NguoiDungs.FirstOrDefault(nd => nd.MaTaiKhoan == check.MaTaiKhoan);

            // Nếu không có thông tin người dùng, chuyển hướng đến trang tạo thông tin người dùng
            if (nguoiDung == null)
            {
                TempData["email"] = email; // Lưu email vào TempData để sử dụng trên trang tạo thông tin
                HttpContext.Session.SetString("email", email);
                return RedirectToAction("CreateUserDetails", "User");
            }

            // Lưu thông tin người dùng vào Session
            HttpContext.Session.SetString("MaTaiKhoan", check.MaTaiKhoan.ToString());
            HttpContext.Session.SetString("UserID", nguoiDung.MaNguoiDung.ToString());
            HttpContext.Session.SetString("MaChucVu", nguoiDung.MaChucVu.ToString());
            HttpContext.Session.SetString("ChucVu", nguoiDung.MaChucVuNavigation.TenChucVu);
            HttpContext.Session.SetString("email", check.Email);
            HttpContext.Session.SetString("TenNguoiDung", nguoiDung.TenNguoiDung);
            HttpContext.Session.SetString("DiaChi", nguoiDung.DiaChi);
            HttpContext.Session.SetString("SDT", nguoiDung.Sdt);
            HttpContext.Session.SetString("userLogin", check.Email);

            // Kiểm tra quyền truy cập
            if (nguoiDung.MaChucVu == 2 || nguoiDung.MaChucVu == 3)
            {
                return RedirectToAction("AdminHome", "Admin");
            }
            // Nếu người dùng đã có thông tin, điều hướng đến trang chủ
            return RedirectToAction("Index", "Home");
        }

        return View();
    }


    // Đăng xuất
    public IActionResult DangXuat()
    {
        if (HttpContext.Session.GetString("userLogin") != null)
        {
            // Xóa tất cả thông tin trong session
            HttpContext.Session.Remove("userLogin");
            HttpContext.Session.Remove("hoTen");
            HttpContext.Session.Remove("email");
            HttpContext.Session.Remove("sdt");

            return RedirectToAction("Index", "Home");
        }
        return RedirectToAction("Index", "Home");
    }

}
