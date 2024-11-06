using DAPMver1.Data;
using DAPMver1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;

namespace DAPMver1.Controllers
{
    public class CartController : Controller
    {
        private readonly DapmTrangv1Context _context;
        public CartController(DapmTrangv1Context context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Lấy MaNguoiDung từ Session
          
            // Kiểm tra xem người dùng đã đăng nhập hay chưa
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserID")))
            {
                TempData["Message"] = "Bạn cần đăng nhập để xem giỏ hàng.";
                return RedirectToAction("Login", "Account");
            }
            var userId = HttpContext.Session.GetString("UserID");

            // Chuyển đổi userId sang int
            int parsedUserId;
            if (!int.TryParse(userId, out parsedUserId))
            {
                TempData["Message"] = "Có lỗi xảy ra, vui lòng thử lại.";
                return RedirectToAction("Index", "Home");
            }

            // Lấy giỏ hàng và ánh xạ vào danh sách chi tiết giỏ hàng
            var cartItems = _context.ChiTietGioHangs
                .Include(c => c.MaKichCoNavigation)
                .ThenInclude(k => k.MaSanPhamNavigation) // Bao gồm thông tin sản phẩm
                .Where(c => c.MaGioHangNavigation.MaNguoiDung == parsedUserId
                   && c.MaGioHangNavigation.TrangThai == "Chưa thanh toán")
                 .ToList();

            // Kiểm tra nếu giỏ hàng rỗng
            if (!cartItems.Any())
            {
                TempData["Message"] = "Giỏ hàng của bạn đang trống.";
            }

            // Trả về giỏ hàng với các thông tin cần thiết
            return View(cartItems);
        }

        // Thêm sản phẩm vào giỏ hàng

        public IActionResult AddToCart(int idsp, int size, int soLuong)
        {
            // Lấy UserID từ Session
             if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserID")))
            {
                // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
                return RedirectToAction("Login", "Account");
            }
            var userId = HttpContext.Session.GetString("UserID");

            // Tìm sản phẩm dựa trên id
            var product = _context.SanPhams.Find(idsp);
            var kichCo = _context.KichCos.FirstOrDefault(n => n.MaSanPham == idsp && n.SoKichCo == size);
            if (product == null || kichCo == null)
            {
                return NotFound();
            }

            // Lấy giỏ hàng hiện tại của người dùng hoặc tạo mới nếu chưa có
            var cart = _context.GioHangs
                               .FirstOrDefault(g => g.MaNguoiDung == int.Parse(userId) && g.TrangThai == "Chưa thanh toán");
            if (cart == null)
            {
                cart = new GioHang
                {
                    MaNguoiDung = int.Parse(userId),
                    NgayTao = DateTime.Now,
                    TrangThai = "Chưa thanh toán",
                    TongTien = 0
                };
                _context.GioHangs.Add(cart);
                _context.SaveChanges();
            }

            // Kiểm tra sản phẩm đã có trong chi tiết giỏ hàng chưa
            var cartItem = _context.ChiTietGioHangs
                                   .FirstOrDefault(c => c.MaGioHang == cart.MaGioHang
                                   && c.MaKichCo == kichCo.MaKichCo);
            if (cartItem != null)
            {
                // Tăng số lượng nếu sản phẩm đã có trong giỏ hàng
                cartItem.SoLuong += soLuong;
            }
            else
            {
                // Thêm sản phẩm mới vào chi tiết giỏ hàng
                cartItem = new ChiTietGioHang
                {
                    MaGioHang = cart.MaGioHang,
                    MaKichCo = kichCo.MaKichCo, // sử dụng kích cỡ được truyền vào
                    SoLuong = soLuong,
                    GiaBan = product.GiaTienMoi
                };
                _context.ChiTietGioHangs.Add(cartItem);
            }

            // Cập nhật tổng tiền của giỏ hàng
            cart.TongTien = cart.ChiTietGioHangs.Sum(c => c.SoLuong * c.GiaBan);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        // Xóa sản phẩm khỏi giỏ hàng

        public IActionResult RemoveFromCart(int idsp, int size)
        {
            // Lấy UserID từ Session
            var userId = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy giỏ hàng của người dùng
            var cart = _context.GioHangs
                .FirstOrDefault(g => g.MaNguoiDung == int.Parse(userId) 
                && g.TrangThai == "Chưa thanh toán");
            if (cart == null) return RedirectToAction("Index");

            // Tìm sản phẩm trong chi tiết giỏ hàng và xóa
            var cartItem = _context.ChiTietGioHangs
                .FirstOrDefault(c => c.MaGioHang == cart.MaGioHang 
                && c.MaKichCo == size && c.MaKichCoNavigation.MaSanPham == idsp); // Sửa dòng này
            if (cartItem != null)
            {
                _context.ChiTietGioHangs.Remove(cartItem);
                _context.SaveChanges();
            }

            // Cập nhật lại tổng tiền của giỏ hàng
            cart.TongTien = cart.ChiTietGioHangs.Sum(c => c.SoLuong * c.GiaBan);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }



        // Cập nhật số lượng sản phẩm trong giỏ hàng
        public IActionResult UpdateQuantity(int idsp, int size, int quantity)
        {
            // Lấy UserID từ Session
            var userId = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy giỏ hàng của người dùng
            var cart = _context.GioHangs.FirstOrDefault(g => g.MaNguoiDung == int.Parse(userId) && g.TrangThai == "Chưa thanh toán");
            if (cart == null) return RedirectToAction("Index");

            // Tìm sản phẩm trong chi tiết giỏ hàng và cập nhật số lượng
            var cartItem = _context.ChiTietGioHangs.FirstOrDefault(c => c.MaGioHang == cart.MaGioHang && c.MaKichCo == size && c.MaKichCoNavigation.MaSanPham == idsp);
            if (cartItem != null && quantity > 0)
            {
                cartItem.SoLuong = quantity;
                _context.SaveChanges();
            }

            // Cập nhật lại tổng tiền của giỏ hàng
            cart.TongTien = cart.ChiTietGioHangs.Sum(c => c.SoLuong * c.GiaBan);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }


     

        public IActionResult Checkout()
        {
            if (HttpContext.Session.GetString("UserID") == null)
            {
                TempData["Message"] = "Bạn cần đăng nhập để xem giỏ hàng.";
                return RedirectToAction("Login", "User");
            }
            // Lấy MaNguoiDung từ Session
            var userId = HttpContext.Session.GetString("UserID");
            int parsedUserId;
            int.TryParse(userId, out parsedUserId);
            var cartItems = _context.ChiTietGioHangs
             .Include(c => c.MaKichCoNavigation)
          .ThenInclude(k => k.MaSanPhamNavigation) // Bao gồm thông tin sản phẩm
             .Where(c => c.MaGioHangNavigation.MaNguoiDung == parsedUserId
                  && c.MaGioHangNavigation.TrangThai == "Chưa thanh toán")
         .ToList();
            if (cartItems == null || !cartItems.Any())
            {
                return RedirectToAction("Index", "Cart"); // Nếu giỏ hàng rỗng, chuyển về giỏ hàng
            }

            // Tạo danh sách các phương thức thanh toán
            ViewBag.PaymentMethods = new List<string> { "Thanh toán khi nhận hàng", "Chuyển khoản ngân hàng", "Ví điện tử" };
            ViewData["TenNguoiDung"] = HttpContext.Session.GetString("TenNguoiDung");
            ViewData["DiaChi"] = HttpContext.Session.GetString("DiaChi");
            ViewData["SDT"] = HttpContext.Session.GetString("SDT");
            return View(cartItems); // Truyền giỏ hàng vào View
        }



        [HttpPost]
        public IActionResult ConfirmOrder(string paymentMethod, string orderOption, string TenNguoiNhanManual, string DiaChiNguoiNhanManual, string SDTNguoiNhanManual)
        {
            if (HttpContext.Session.GetString("UserID") == null)
            {
                TempData["Message"] = "Bạn cần đăng nhập để xem giỏ hàng.";
                return RedirectToAction("Login", "User");
            }
            // Lấy MaNguoiDung từ Session
            var userId = HttpContext.Session.GetString("UserID");
            var parseuserId = int.Parse(userId);
            var cartItems = _context.ChiTietGioHangs
         .Include(c => c.MaKichCoNavigation)
             .ThenInclude(k => k.MaSanPhamNavigation) // Bao gồm thông tin sản phẩm
         .Where(c => c.MaGioHangNavigation.MaNguoiDung == parseuserId
                     && c.MaGioHangNavigation.TrangThai == "Chưa thanh toán")
         .ToList();

            if (cartItems == null || !cartItems.Any())
            {
                return RedirectToAction("Index", "Home");  // Giỏ hàng rỗng
            }

            // Kiểm tra xem người dùng đã đăng nhập chưa
            if (HttpContext.Session.GetString("UserID") == null)
            {
                // Nếu chưa, yêu cầu đăng nhập
                return RedirectToAction("Login", "User");
            }

            var UserID = HttpContext.Session.GetString("UserID");
            var parseUserID = int.Parse(UserID);
            var nguoiDung = _context.NguoiDungs.FirstOrDefault(nd => nd.MaNguoiDung == parseUserID);

            if (nguoiDung == null)
            {
                // Nếu người dùng không tồn tại, yêu cầu đăng nhập lại
                return RedirectToAction("Login", "User");
            }

            // Tính tổng tiền giỏ hàng
            double tongTien = cartItems.Sum(item => item.SoLuong * item.GiaBan);

            string tenNguoiNhan, diaChiNguoiNhan, sdtNguoiNhan;

            if (orderOption == "true")
            {
                // Sử dụng thông tin từ tài khoản
                tenNguoiNhan = nguoiDung.TenNguoiDung;
                diaChiNguoiNhan = nguoiDung.DiaChi;
                sdtNguoiNhan = nguoiDung.Sdt;
            }
            else
            {
                // Sử dụng thông tin người nhận từ form
                tenNguoiNhan = TenNguoiNhanManual;
                diaChiNguoiNhan = DiaChiNguoiNhanManual;
                sdtNguoiNhan = SDTNguoiNhanManual;
            }

            // Lấy thông tin người nhận từ biểu mẫu
            //string tenNguoiNhan = Request.Form["TenNguoiNhan"];
            //string diaChiNguoiNhan = Request.Form["DiaChiNguoiNhan"];
            //string sdtNguoiNhan = Request.Form["SDTNguoiNhan"];
            var matk = HttpContext.Session.GetString("MaTaiKhoan");
            int MaTaiKhoan = int.Parse(matk);
            TaiKhoan taiKhoan = _context.TaiKhoans.FirstOrDefault(tk => tk.MaTaiKhoan == MaTaiKhoan);
            var macv = HttpContext.Session.GetString("MaChucVu");

            int MaChucVu = int.Parse(macv);
            ChucVu chucVu = _context.ChucVus.FirstOrDefault(tk => tk.MaChucVu == MaChucVu);
            nguoiDung = new NguoiDung
            {
                MaTaiKhoan = taiKhoan.MaTaiKhoan,
                MaChucVu = chucVu.MaChucVu
            };

            // Tạo đơn hàng mới
            DonHang donHang = new DonHang
            {
                MaDonHang = Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper(),  // Tạo mã đơn hàng duy nhất
                NgayDatHang = DateOnly.FromDateTime(DateTime.Now),
                TrangThai = "Chờ xử lý",
                PhiVanChuyen = 30000,  // Giả định phí vận chuyển
                TongTien = cartItems.Sum(c => c.GiaBan * c.SoLuong), // Tính tổng tiền từ giỏ hàng

                // Gán giá trị cho các thuộc tính mới
                TongSl = cartItems.Sum(c => c.SoLuong),  // Tổng số lượng sản phẩm trong giỏ hàng
                TongSoTien = (int)(cartItems.Sum(c => c.GiaBan * c.SoLuong) + 30000),  // Tổng số tiền bao gồm phí vận chuyển
                TienPhaiTra = (int)(cartItems.Sum(c => c.GiaBan * c.SoLuong) + 30000), // Số tiền phải trả cũng bao gồm phí vận chuyển
                MaNguoiGui = parseUserID,  // Thay thế bằng ID người dùng thực tế
                SdtnguoiNhan = sdtNguoiNhan,  // Giả định số điện thoại người nhận
                DiaChiNguoiNhan = diaChiNguoiNhan,  // Giả định địa chỉ
                TenNguoiNhan = tenNguoiNhan,  // Giả định tên người nhận
                HinhThucNhanHang = "Giao Hàng",

                //MaVoucher = 1,// Bạn có thể thay thế bằng mã voucher nếu có
            };

            // Lưu đơn hàng vào cơ sở dữ liệu
            _context.DonHangs.Add(donHang);
            _context.SaveChanges();
            CreatePayment(donHang);
            CreateInvoice(donHang, parseUserID);
        
            foreach (var item in cartItems)
            {
                ChiTietDonHang chiTietDonHang = new ChiTietDonHang
                {
                    MaDonHang = donHang.MaDonHang,
                    MaKichCo = (int)item.MaKichCoNavigation.MaKichCo,
                    Soluong = item.SoLuong,
                    DonGia = (int)item.GiaBan,
                    TongTien = (int)(item.SoLuong * item.GiaBan)
                };

                _context.ChiTietDonHangs.Add(chiTietDonHang);
            }
            _context.SaveChanges();
            _context.ChiTietGioHangs.RemoveRange(cartItems);
            _context.SaveChanges();
            // Sau khi thanh toán thành công, chuyển hướng đến trang PaymentSuccess
            var maDonHang = donHang.MaDonHang;

            return RedirectToAction("OrderSuccess", "Cart", new { maDonHang });
        }
        public IActionResult OrderSuccess(string maDonHang)
        {
            var order = _context.DonHangs.FirstOrDefault(o => o.MaDonHang == maDonHang);

            if (order == null)
            {
                return NotFound(); // Xử lý trường hợp không tìm thấy đơn hàng
            }

            ViewBag.OrderId = maDonHang;
            ViewBag.OrderDetails = order;
            return View();
        }
        private void CreateInvoice(DonHang donHang, int userId)
        {
            // Tạo hóa đơn mới
            HoaDon hoaDon = new HoaDon
            {
                MaHoaDon = Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper(), // Tạo mã hóa đơn duy nhất
                MaDonHang = donHang.MaDonHang,
                MaNguoiDung = userId,
                NgayXuatHoaDon = DateTime.Now,
                TongTien = donHang.TongSoTien, // Tổng tiền hóa đơn bằng tổng số tiền đơn hàng
                MaThanhToan = 1 // Bạn có thể thay thế bằng mã thanh toán thực tế nếu cần
            };

            // Lưu hóa đơn vào cơ sở dữ liệu
            _context.HoaDons.Add(hoaDon);
            _context.SaveChanges();
        }
        private void CreatePayment(DonHang donHang)
        {
            // Tạo hóa đơn mới
            ThanhToan thanhToan = new ThanhToan
            {
                MaDonHang = donHang.MaDonHang,
                PhuongThucThanhToan = "Trực tuyến",
                NgayThanhToan = DateTime.Now,
                TongTien = donHang.TongSoTien, // Tổng tiền hóa đơn bằng tổng số tiền đơn hàng
                TrangThaiThanhToan = true
            };

            // Lưu hóa đơn vào cơ sở dữ liệu
            _context.ThanhToans.Add(thanhToan);
            _context.SaveChanges();
        }

    }
}