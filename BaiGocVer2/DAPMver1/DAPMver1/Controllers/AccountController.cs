using Microsoft.AspNetCore.Mvc;
using DAPMver1.Data;
using System.Linq;
using DAPMver1.Models;
using DAPMver1.Common;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public class AccountController : Controller
{
    private readonly DapmTrangv1Context _context;
    private readonly Common _common;

    public AccountController(DapmTrangv1Context context, Common common)
    {
        _context = context;
        _common = common;
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
            HttpContext.Session.SetString("email", check.Email);
            HttpContext.Session.SetString("TenNguoiDung", nguoiDung.TenNguoiDung);
            HttpContext.Session.SetString("DiaChi", nguoiDung.DiaChi);
            HttpContext.Session.SetString("SDT", nguoiDung.Sdt);
            HttpContext.Session.SetString("userLogin", check.Email);
            ViewData["TenNguoiDung"] = HttpContext.Session.GetString("TenNguoiDung");  
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
            HttpContext.Session.Remove("UserID");
            HttpContext.Session.Remove("MaTaiKhoan");
            HttpContext.Session.Remove("MaChucVu");
            HttpContext.Session.Remove("TenNguoiDung");
            HttpContext.Session.Remove("DiaChi");
            HttpContext.Session.Remove("SDT");
            HttpContext.Session.Remove("email");
       

            return RedirectToAction("Index", "Home");
        }
        TempData["Message"] = "Bạn đã đăng xuất. Vui lòng đăng nhập lại để tiếp tục.";

        return RedirectToAction("Index", "Home");
    }
    //
    //
    //
    //================================================================================
    // Quên mật khẩu
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(string email)
    {
        if (ModelState.IsValid)
        {
            // Kiểm tra xem email có tồn tại trong hệ thống không
            var account = _context.TaiKhoans.FirstOrDefault(u => u.Email == email);
            var user = _context.NguoiDungs.FirstOrDefault(u => u.MaTaiKhoan == account.MaTaiKhoan);

            if (user != null)
            {
                // Tạo mã khôi phục
                string recoveryCode = Guid.NewGuid().ToString(); // Tạo mã khôi phục

                // Gửi mã khôi phục qua email
                string subject = "Khôi phục mật khẩu";
                string content = $"Mã khôi phục của bạn là: {recoveryCode}";

                if (Common.SendMail(user.TenNguoiDung, subject, content, account.Email))
                {
                    HttpContext.Session.SetString("RecoveryCode", recoveryCode);
                    // Lưu mã khôi phục vào session để kiểm tra sau này
                    HttpContext.Session.SetString("email", account.Email); // Lưu email để khôi phục sau
                    ViewBag.Message = "Mã khôi phục đã được gửi tới email của bạn.";
                    return RedirectToAction("VerifyRecoveryCode");
                }
                else
                {
                    ViewBag.Error = "Có lỗi xảy ra trong việc gửi email.";
                }
            }
            else
            {
                ViewBag.Error = "Email không tồn tại.";
            }
        }
        return View();
    }

    // Xác nhận mã khôi phục
    [HttpGet]
    public IActionResult VerifyRecoveryCode()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult VerifyRecoveryCode(string recoveryCode)
    {
        if (ModelState.IsValid)
        {
            string sessionRecoveryCode = HttpContext.Session.GetString("RecoveryCode");
            string email = HttpContext.Session.GetString("email");

            if (sessionRecoveryCode == recoveryCode)
            {
                // Mã khôi phục hợp lệ, chuyển sang trang đặt lại mật khẩu
                return RedirectToAction("ResetPassword");
            }
            else
            {
                ViewBag.Error = "Mã khôi phục không hợp lệ.";
            }
        }
        return View();
    }


    // Đặt lại mật khẩu
    [HttpGet]
    public IActionResult ResetPassword()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(string newPassword, string confirmPassword)
    {
        string email = HttpContext.Session.GetString("email");
        if (newPassword != confirmPassword)
        {
            ViewBag.Error = "Mật khẩu không khớp.";
            return View();
        }
        var account = await _context.TaiKhoans.FirstOrDefaultAsync(u => u.Email == email);
        if (account != null)
        {
            // Mã hóa mật khẩu mới trước khi lưu
            // Mã hóa mật khẩu
            //string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
            string hashedPassword = newPassword;

            // Cắt ngắn mật khẩu đã mã hóa về 10 ký tự
            //if (hashedPassword.Length > 10)
            //{
            //    account.MatKhau = hashedPassword.Substring(0, 10);
            //}
            //else
            //{
            //    account.MatKhau = hashedPassword; // Nếu mật khẩu đã mã hóa nhỏ hơn hoặc bằng 10 ký tự
            //}
            account.MatKhau = hashedPassword;
            // Cập nhật tài khoản
            _context.TaiKhoans.Update(account);
            await _context.SaveChangesAsync();

         
            ViewBag.Message = "Mật khẩu đã được đặt lại thành công.";
            return RedirectToAction("Login");
        }
        //var user = _context.NguoiDungs.FirstOrDefault(u => u.MaTaiKhoan == account.MaTaiKhoan);

        //if (user != null && newPassword == confirmPassword)
        //{
        //    // Cập nhật mật khẩu mới
        //    account.MatKhau = newPassword;
        //    _context.Entry(user).State = System.Data.Entity.EntityState.Modified;
        //    _context.SaveChanges();

        //    // Xóa session RecoveryCode và Email
        //    Session["RecoveryCode"] = null;
        //    Session["email"] = null;

        //    ViewBag.Message = "Mật khẩu đã được đặt lại thành công.";
        //    return RedirectToAction("Login");
        //}
        else
        {
            ViewBag.Error = "Mật khẩu không khớp hoặc có lỗi xảy ra.";
        }

        return View();
    }
}
