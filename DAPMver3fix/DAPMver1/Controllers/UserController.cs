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

        public IActionResult OrderHistory()
        {

            // Get the email from session
            var email = HttpContext.Session.GetString("email");

            if (email == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Find the user's account and details using their email
            var taiKhoan = db.TaiKhoans.FirstOrDefault(tk => tk.Email == email);
            if (taiKhoan == null)
            {
                return RedirectToAction("CreateUserDetails");
            }

            var nguoiDung = db.NguoiDungs.FirstOrDefault(nd => nd.MaTaiKhoan == taiKhoan.MaTaiKhoan);

            // Retrieve orders related to the user
            var orderHistory = db.DonHangs
                .Where(dh => dh.MaNguoiGui == nguoiDung.MaNguoiDung)
                .Include(dh => dh.ChiTietDonHangs) // Include order details if needed
                .ToList();

            return View(orderHistory);
        }
        [HttpGet]
        public IActionResult OrderDetails(string maDonHang)
        {
            // Kiểm tra tham số đầu vào
            if (string.IsNullOrEmpty(maDonHang))
            {
                return BadRequest("Mã đơn hàng không hợp lệ.");
            }

            // Tìm đơn hàng trong cơ sở dữ liệu
            var donHang = db.DonHangs
                            .Include(dh => dh.ChiTietDonHangs) // Bao gồm chi tiết đơn hàng
                            .ThenInclude(ct => ct.MaKichCoNavigation) // Bao gồm thông tin về kích cỡ
                            .ThenInclude(kc => kc.MaSanPhamNavigation) // Bao gồm thông tin về sản phẩm
                            .FirstOrDefault(dh => dh.MaDonHang == maDonHang);



            // Kiểm tra nếu ChiTietDonHangs có giá trị null (không có chi tiết đơn hàng)
            if (donHang.ChiTietDonHangs == null || !donHang.ChiTietDonHangs.Any())
            {
                ViewBag.Message = "Đơn hàng này không có chi tiết hoặc chưa được xử lý.";
            }

            // Trả về view và truyền dữ liệu đơn hàng vào view
            return View(donHang);
        }
        // Hủy đơn hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelOrder(string maDonHang)
        {
            // Lấy đơn hàng từ cơ sở dữ liệu
            var donHang = db.DonHangs.FirstOrDefault(dh => dh.MaDonHang == maDonHang);
            if (donHang == null)
            {
                return NotFound(); // Nếu không tìm thấy đơn hàng
            }

            // Kiểm tra trạng thái đơn hàng
            if (donHang.TrangThai == "Chờ xử lý")
            {
                // Cập nhật trạng thái đơn hàng thành "Đã hủy"
                donHang.TrangThai = "Đã hủy";
                db.Update(donHang);
                db.SaveChanges();

                // Chuyển hướng về trang lịch sử mua hàng với thông báo thành công
                TempData["SuccessMessage"] = "Đơn hàng đã được hủy thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể hủy đơn hàng vì đơn hàng đã được xử lý.";
            }

            return RedirectToAction("OrderHistory"); // Chuyển hướng về trang lịch sử mua hàng
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult NhanHang(string maDonHang)
        {
            // Lấy đơn hàng từ cơ sở dữ liệu
            var donHang = db.DonHangs.FirstOrDefault(dh => dh.MaDonHang == maDonHang);
            if (donHang == null)
            {
                return NotFound(); // Nếu không tìm thấy đơn hàng
            }

            // Kiểm tra trạng thái đơn hàng
            if (donHang.TrangThai == "Đang giao")
            {
                // Cập nhật trạng thái đơn hàng thành "Đã nhận"
                donHang.TrangThai = "Đã nhận";
                db.Update(donHang);
                db.SaveChanges();

                // Lấy tất cả chi tiết đơn hàng
                var chiTietDonHangList = db.ChiTietDonHangs.Where(ct => ct.MaDonHang == maDonHang).ToList();
                foreach (var chiTiet in chiTietDonHangList)
                {
                    // Tạo bản ghi bảo hành cho từng sản phẩm
                    var baoHanh = new BaoHanh
                    {
                        MaKichCo = chiTiet.MaKichCo,
                        MaNguoiDung = donHang.MaNguoiGui, // Giả định MaNguoiGui là người dùng nhận hàng
                        NgayBaoHanh = DateTime.Now, // Ngày bảo hành là ngày nhận hàng
                        NgayKetThuc = DateTime.Now.AddMonths(1), // Ngày kết thúc bảo hành là sau 1 tháng
                        MoTa = "Bảo hành cho sản phẩm " + chiTiet.MaKichCo,
                        TrangThai = false // Giả định 1 là trạng thái hợp lệ
                    };
                    db.BaoHanhs.Add(baoHanh);
                }

                db.SaveChangesAsync(); // Lưu các thay đổi vào cơ sở dữ liệu
                TempData["SuccessMessage"] = "Nhận hàng thành công và đã tạo bảo hành cho sản phẩm.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể nhận đơn hàng vì đơn hàng đã được xử lý.";
            }

            return RedirectToAction("OrderHistory"); // Chuyển hướng về trang lịch sử mua hàng
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult YeuCauBaoHanh(int maSanPham)
        {
            var donHang = db.DonHangs
                .Include(dh => dh.ChiTietDonHangs)
                .FirstOrDefault(dh => dh.ChiTietDonHangs.Any(ct => ct.MaKichCo == maSanPham && dh.TrangThai == "Đã nhận"));

            if (donHang == null)
            {
                TempData["ErrorMessage"] = "Không thể yêu cầu bảo hành vì đơn hàng không hợp lệ hoặc chưa được nhận.";
                return RedirectToAction("OrderHistory");
            }

            // Tìm bảo hành cho sản phẩm đã tồn tại trong cơ sở dữ liệu
            var baoHanh = db.BaoHanhs
                .FirstOrDefault(bh => bh.MaKichCo == maSanPham && bh.MaNguoiDung == int.Parse(HttpContext.Session.GetString("UserID")));

            if (baoHanh == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy bảo hành cho sản phẩm này.";
                return RedirectToAction("OrderHistory");
            }

            // Kiểm tra xem bảo hành có nằm trong thời gian hợp lệ
            if (baoHanh.NgayKetThuc >= DateTime.Now)
            {
                // Cập nhật trạng thái bảo hành thành true
                baoHanh.TrangThai = true;

                try
                {
                    // Lưu thay đổi vào cơ sở dữ liệu
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Yêu cầu bảo hành thành công!";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Có lỗi xảy ra: {ex.Message}";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Bảo hành cho sản phẩm này đã hết hạn.";
            }

            return RedirectToAction("OrderHistory");
        }

    }
}