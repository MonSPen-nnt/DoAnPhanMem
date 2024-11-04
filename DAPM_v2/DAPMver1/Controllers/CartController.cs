using DAPMver1.Data;
using DAPMver1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
                                   .FirstOrDefault(c => c.MaGioHang == cart.MaGioHang && c.MaKichCo == kichCo.MaKichCo);
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


        public IActionResult Checkout1()
        {
            // Lấy UserID từ Session
            var userId = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy giỏ hàng của người dùng
            var cart = _context.GioHangs
                        .Include(g => g.ChiTietGioHangs)
                        .ThenInclude(c => c.MaKichCoNavigation) // Bao gồm chi tiết sản phẩm
                        .FirstOrDefault(g => g.MaNguoiDung == int.Parse(userId) && g.TrangThai == "Chưa thanh toán");

            if (cart == null || !cart.ChiTietGioHangs.Any())
            {
                TempData["Message"] = "Giỏ hàng của bạn đang trống.";
                return RedirectToAction("Index", "Cart");
            }

            // Lưu thông tin người dùng vào ViewData
            ViewData["TenNguoiDung"] = HttpContext.Session.GetString("TenNguoiDung");
            ViewData["DiaChi"] = HttpContext.Session.GetString("DiaChi");
            ViewData["SDT"] = HttpContext.Session.GetString("SDT");

            // Tạo danh sách các phương thức thanh toán
            ViewBag.PaymentMethods = new List<string> { "Thanh toán khi nhận hàng", "Chuyển khoản ngân hàng", "Ví điện tử" };

            return View(cart.ChiTietGioHangs); // Truyền chi tiết giỏ hàng vào View
        }

        public ActionResult Checkout()
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



    }
}