using DAPMver1.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DAPMver1.Controllers
{
    public class ProductController : Controller
    {
        private readonly DapmTrangv1Context db;

        public ProductController(DapmTrangv1Context context)
        {
            this.db = context;
        }

        public IActionResult Index()
        {
            var products = db.SanPhams.ToList(); // Lấy tất cả sản phẩm từ bảng SanPham
            return View(products); // Truyền danh sách sản phẩm sang view
        }

        public IActionResult ChiTietSP(int id)
        {
            var data = db.SanPhams.Include(s => s.KichCos).FirstOrDefault
                (s => s.MaSanPham == id);
            data = db.SanPhams
                .Include(p => p.MaDanhMucNavigation)// Bao gồm thông tin danh mục nếu cần
                 .Include(p => p.MaVatLieuNavigation)
                    .Include(p => p.MaNhaCungCapNavigation)
                .SingleOrDefault(p => p.MaSanPham == id);

            if (data == null)
            {
                TempData["Message"] = $"Không thấy sản phẩm có mã {id}";
                return Redirect("/404");
            }

            return View("ChiTietSP", data); // Gọi rõ ràng View "ChiTietSanPham"
        }

    }
}
