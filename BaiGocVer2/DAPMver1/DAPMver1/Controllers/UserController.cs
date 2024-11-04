using Microsoft.AspNetCore.Mvc;
using DAPMver1.Data;
using System.Linq;
using DAPMver1.Models;
using Microsoft.EntityFrameworkCore;

namespace BanTrangSuc.Controllers
{
    public class UserController : Controller
    {
        private readonly DapmTrangv1Context db;
        public UserController(DapmTrangv1Context db)
        {
            this.db = db;
        }

        //

        //
        //Tạo thông tin người dùng
        public IActionResult CreateUserDetails()
        {
            // Lấy email từ TempData nếu có
        
            var email = HttpContext.Session.GetString("email");

            var taiKhoan = db.TaiKhoans.FirstOrDefault(tk => tk.Email == email);
            if (taiKhoan == null)
            {
                return RedirectToAction("Login"); // Hoặc một hành động xử lý khi tài khoản không tồn tại
            }
            var nguoiDung = new NguoiDung
            {
                MaTaiKhoan = taiKhoan.MaTaiKhoan,
            };
            return View(nguoiDung);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUserDetails(NguoiDung nguoiDung, IFormFile ImageFile)
        {
            if (ModelState.IsValid)
            {
                // Handle file upload
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    // Tạo tên file duy nhất để tránh xung đột
                    var fileName = Path.GetFileName(ImageFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                    // Lưu file vào thư mục wwwroot/images
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }

                    // Gán đường dẫn của ảnh cho thuộc tính AnhSp của sản phẩm
                    nguoiDung.AnhDaiDien = "/images/" + fileName; // Đảm bảo đường dẫn hợp lệ
                }


                nguoiDung.MaChucVu = 1;
                db.NguoiDungs.Add(nguoiDung);
                db.SaveChanges();

                // Lưu thông tin vào Session sau khi tạo
                HttpContext.Session.SetString("UserID", nguoiDung.MaNguoiDung.ToString());
                HttpContext.Session.SetString("TenNguoiDung", nguoiDung.TenNguoiDung.ToString());
                HttpContext.Session.SetString("DiaChi", nguoiDung.DiaChi.ToString());
                HttpContext.Session.SetString("SDT", nguoiDung.Sdt.ToString());
                HttpContext.Session.SetString("MaChucVu", nguoiDung.MaChucVu.ToString());
              
                return RedirectToAction("UserDetails", "User");
            }

            return View(nguoiDung);
        }
        //
        //
        // Chỉnh sửa thông tin người dùng
        public IActionResult EditUserDetails()
        {
            var email = HttpContext.Session.GetString("email");

            TaiKhoan taiKhoan = db.TaiKhoans.FirstOrDefault(tk => tk.Email == email);
            if (email == null)
            {
                return RedirectToAction("Login", "Account");
            }

               NguoiDung nguoiDung = db.NguoiDungs.FirstOrDefault(nd => nd.MaTaiKhoan == taiKhoan.MaTaiKhoan);

            return View(nguoiDung);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditUserDetails(NguoiDung nguoiDung, IFormFile ImageFile)
        {
            var tk = db.TaiKhoans.FirstOrDefault(n => n.MaTaiKhoan == int.Parse(HttpContext.Session.GetString("MaTaiKhoan")));
            nguoiDung = db.NguoiDungs.FirstOrDefault(k=>k.MaTaiKhoan == tk.MaTaiKhoan);
            if (ModelState.IsValid)
            {
                 // Kiểm tra xem có tệp hình ảnh mới không
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    // Tạo tên file duy nhất để tránh xung đột
                    var fileName = Path.GetFileName(ImageFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                 
                    // Lưu file vào thư mục wwwroot/images
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }

                    // Gán đường dẫn của ảnh mới cho thuộc tính AnhSp
                    nguoiDung.AnhDaiDien = "/images/" + fileName; // Đảm bảo đường dẫn hợp lệ
                }

                // Cập nhật thông tin người dùng
                db.Update(nguoiDung);
                await db.SaveChangesAsync();

                // Tìm người dùng trong cơ sở dữ liệu
                var existingUser = db.NguoiDungs.FirstOrDefault(u => u.MaNguoiDung == nguoiDung.MaNguoiDung);
                if (existingUser != null)
                {
                    existingUser.TenNguoiDung = nguoiDung.TenNguoiDung;
                    existingUser.DiaChi = nguoiDung.DiaChi;
                    existingUser.Sdt = nguoiDung.Sdt;
                    existingUser.AnhDaiDien = nguoiDung.AnhDaiDien;

                    db.Update(existingUser);
                    db.SaveChanges();
                }
                //Chuyển hướng đến trang thông tin người dùng
                return RedirectToAction("UserDetails", new { id = nguoiDung.MaNguoiDung });
            }

            // Nếu có lỗi, trả lại view
            return View(nguoiDung);
        }
       
        //
        //
        //Thông tin chi tiết người dùng
        public IActionResult UserDetails()
        {
             var email = HttpContext.Session.GetString("email");

               if (email == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var taiKhoan = db.TaiKhoans.FirstOrDefault(tk => tk.Email == email);
            var nguoiDung = db.NguoiDungs.FirstOrDefault(nd => nd.MaTaiKhoan == taiKhoan.MaTaiKhoan);
            if (nguoiDung == null)
            {
                return RedirectToAction("CreateUserDetails");
            }

            return View(nguoiDung);
        }
    }
}