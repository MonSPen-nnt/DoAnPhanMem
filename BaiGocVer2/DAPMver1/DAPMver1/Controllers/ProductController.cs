using DAPMver1.Data;
using DAPMver1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DAPMver1.Controllers
{
    public class ProductController : Controller
    {
        private readonly DapmTrangv1Context db;

        public ProductController(DapmTrangv1Context context)
        {
            this.db = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IActionResult Index(int? categoryId)
        {
            var danhMucList = db.DanhMucs.ToList();
            ViewBag.DanhMucList = danhMucList;
            ViewBag.SelectedDanhMuc = categoryId;

            // Lọc sản phẩm theo danh mục (nếu có)
            var sanPhams = categoryId == null
                ? db.SanPhams.ToList()
                : db.SanPhams.Where(sp => sp.MaDanhMuc == categoryId).ToList();

            return View(sanPhams);
        }
        public IActionResult PhanLoai(int? categoryId)
        {
            var danhMucList = db.DanhMucs.ToList();
            ViewBag.DanhMucList = danhMucList;
            ViewBag.SelectedDanhMuc = categoryId;

            // Lọc sản phẩm theo danh mục (nếu có)
            var sanPhams = categoryId == null
                ? db.SanPhams.ToList()
                : db.SanPhams.Where(sp => sp.MaDanhMuc == categoryId).ToList();

            return View(sanPhams);
        }


        public ActionResult Search(string keyword)
        {

            if (!string.IsNullOrEmpty(keyword))
            {
                // Tìm kiếm sản phẩm theo tên
                var sanPhams = db.SanPhams
                                 .Where(sp => sp.TenSanPham.Contains(keyword))
                                 .OrderByDescending(sp => sp.NgayTao)
                                 .ToList();
                ViewBag.Keyword = keyword;
                return View("Search", sanPhams);
            }


            return RedirectToAction("Index");
        }

        public IActionResult ChiTietSP(int id)
        {
            // Nạp sản phẩm và bao gồm các thông tin liên quan đến danh mục, vật liệu, nhà cung cấp và đánh giá
            var data = db.SanPhams
                .Include(s => s.KichCos)
                .Include(s => s.MaDanhMucNavigation) // Bao gồm thông tin danh mục
                .Include(s => s.MaVatLieuNavigation) // Bao gồm thông tin vật liệu
                .Include(s => s.MaNhaCungCapNavigation) // Bao gồm thông tin nhà cung cấp
                .Include(s => s.DanhGiaSanPhams) // Bao gồm danh sách đánh giá của sản phẩm
                .FirstOrDefault(s => s.MaSanPham == id);

            if (data == null)
            {
                TempData["Message"] = $"Không thấy sản phẩm có mã {id}";
                return Redirect("/404");
            }

            // Lấy danh sách sản phẩm gợi ý theo danh mục
            var goiYSP = db.SanPhams
                .Where(sp => sp.MaDanhMuc == data.MaDanhMuc && sp.MaSanPham != data.MaSanPham)
                .OrderByDescending(sp => sp.NgayTao)
                .Take(5) // Lấy 5 sản phẩm mới nhất
                .ToList();

            // Lấy danh sách sản phẩm gợi ý theo giá cao nhất
            var goiYTheoGiaCaoNhat = db.SanPhams
                .Where(sp => sp.MaDanhMuc == data.MaDanhMuc && sp.MaSanPham != data.MaSanPham)
                .OrderByDescending(sp => sp.GiaTienMoi)
                .Take(5)
                .ToList();

            // Tính điểm trung bình đánh giá
            var diemTrungBinh = data.DanhGiaSanPhams.Any()
                ? data.DanhGiaSanPhams.Average(dg => dg.DiemDanhGia)
                : 0; // Trường hợp không có đánh giá thì trả về 0

            var viewModel = new ChiTietSanPhamViewModels
            {
                SanPham = data,
                GoiYSanPhams = goiYSP,
                GoiYSanPhamsTheoGiaCaoNhat = goiYTheoGiaCaoNhat,
                DiemTrungBinh = diemTrungBinh // Truyền điểm trung bình vào ViewModel
            };

            return View("ChiTietSP", viewModel); // Gọi rõ ràng View "ChiTietSanPham"
        }



    }
}
