using DAPMver1.Data;
using Microsoft.AspNetCore.Mvc;

namespace DAPMver1.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly DapmTrangv1Context _context;
        public ShoppingCartController(DapmTrangv1Context context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Lấy MaNguoiDung từ Session
            var userId = HttpContext.Session.GetString("UserID");

            // Kiểm tra xem người dùng đã đăng nhập hay chưa
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Message"] = "Bạn cần đăng nhập để xem giỏ hàng.";
                return RedirectToAction("Login", "Account");
            }

            // Chuyển đổi userId sang int
            int parsedUserId;
            if (!int.TryParse(userId, out parsedUserId))
            {
                TempData["Message"] = "Có lỗi xảy ra, vui lòng thử lại.";
                return RedirectToAction("Index", "Home");
            }

            // Lấy giỏ hàng và ánh xạ vào danh sách chi tiết giỏ hàng
            var cartItems = _context.GioHangs
                                .Where(g => g.MaNguoiDung == parsedUserId && g.TrangThai == "Chưa thanh toán")
                                .SelectMany(g => g.ChiTietGioHangs)
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
        public IActionResult AddToCart(int id, int size, int SoLuong)
        {
            // Lấy UserID từ Session
            var userId = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userId))
            {
                // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
                return RedirectToAction("Login", "Account");
            }

            // Tìm sản phẩm dựa trên id
            var product = _context.SanPhams.Find(id);
            var kichCo = _context.KichCos.FirstOrDefault(n => n.MaSanPham == id && n.SoKichCo == size);
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
                                    .FirstOrDefault(c => c.MaGioHang == cart.MaGioHang && c.MaKichCo == size);
            if (cartItem != null)
            {
                // Tăng số lượng nếu sản phẩm đã có trong giỏ hàng
                cartItem.SoLuong += SoLuong;
            }
            else
            {
                // Thêm sản phẩm mới vào chi tiết giỏ hàng
                cartItem = new ChiTietGioHang
                {
                    MaGioHang = cart.MaGioHang,
                    MaKichCo = size, // sử dụng kích cỡ được truyền vào
                    SoLuong = SoLuong,
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
        public IActionResult RemoveFromCart(int id, int size)
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

            // Tìm sản phẩm trong chi tiết giỏ hàng và xóa
            var cartItem = _context.ChiTietGioHangs.FirstOrDefault(c => c.MaGioHang == cart.MaGioHang && c.MaKichCo == size);
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
        public IActionResult UpdateQuantity(int id, int size, int quantity)
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
            var cartItem = _context.ChiTietGioHangs.FirstOrDefault(c => c.MaGioHang == cart.MaGioHang && c.MaKichCo == size);
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

    }
}