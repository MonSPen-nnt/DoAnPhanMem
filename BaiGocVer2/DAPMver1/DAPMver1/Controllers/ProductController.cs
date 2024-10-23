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
            var goiYSP = db.SanPhams
                     .Where(sp => sp.MaDanhMuc == data.MaDanhMuc && sp.MaSanPham != data.MaSanPham)
                     .OrderByDescending(sp => sp.NgayTao)
                     .Take(5) // Lấy 5 sản phẩm mới nhất
                     .ToList();
            var goiYTheoGiaCaoNhat = db.SanPhams
                        .Where(sp => sp.MaDanhMuc == data.MaDanhMuc && sp.MaSanPham != data.MaSanPham)
                        .OrderByDescending(sp => sp.GiaTienMoi)
                        .Take(5)
                        .ToList();
            var viewModel = new ChiTietSanPhamViewModels
            {
                SanPham = data,
                GoiYSanPhams = goiYSP,
                GoiYSanPhamsTheoGiaCaoNhat = goiYTheoGiaCaoNhat
            };
            return View("ChiTietSP", viewModel); // Gọi rõ ràng View "ChiTietSanPham"
        }
       

    }
}
