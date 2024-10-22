using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAPMver1.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DAPMver1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SanPhamsController : Controller
    {
        private readonly DapmTrangv1Context _context;

        public SanPhamsController(DapmTrangv1Context context)
        {
            _context = context;
        }

        // GET: SanPhams
        public async Task<IActionResult> Index(string searchString, int? maDanhMuc)
        {
            // Khởi tạo IQueryable<SanPham> để lưu kết quả tìm kiếm.
            IQueryable<SanPham> sanPham = _context.SanPhams;

            // Thực hiện Include để lấy dữ liệu liên quan.
            sanPham = sanPham.Include(s => s.MaDanhMucNavigation)
                         .Include(s => s.MaNhaCungCapNavigation)
                         .Include(s => s.MaVatLieuNavigation);

            // Lọc theo danh mục nếu được chọn.
            if (maDanhMuc.HasValue && maDanhMuc.Value > 0)
            {
                sanPham = sanPham.Where(sp => sp.MaDanhMuc == maDanhMuc.Value);

                var category = await _context.DanhMucs.FindAsync(maDanhMuc.Value);
                ViewBag.Title = category?.TenDanhMuc ?? "Danh sách sản phẩm";
            }
            else
            {
                ViewBag.Title = "Tất cả sản phẩm";
            }

            // Gán danh sách danh mục và danh mục được chọn vào ViewBag.
            var danhMucs = await _context.DanhMucs.ToListAsync();
            ViewBag.DanhMucs = danhMucs.Select(d => new SelectListItem
            {
                Value = d.MaDanhMuc.ToString(),
                Text = d.TenDanhMuc
            }).ToList();
            ViewBag.SelectedDanhMuc = maDanhMuc;  // Truyền danh mục đã chọn

            return View(await sanPham.ToListAsync());
        }

        // GET: SanPhams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams
                .Include(s => s.MaDanhMucNavigation)
                .Include(s => s.MaNhaCungCapNavigation)
                .Include(s => s.MaVatLieuNavigation)
                .FirstOrDefaultAsync(m => m.MaSanPham == id);
            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }

        // GET: SanPhams/Create
        public IActionResult Create()
        {
            ViewData["MaDanhMuc"] = new SelectList(_context.DanhMucs, "MaDanhMuc", "MaDanhMuc");
            ViewData["MaNhaCungCap"] = new SelectList(_context.NhaCungCaps, "MaNhaCungCap", "MaNhaCungCap");
            ViewData["MaVatLieu"] = new SelectList(_context.VatLieus, "MaVatLieu", "MaVatLieu");
            return View();
        }

        // POST: SanPhams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaSanPham,TenSanPham,GiaTienMoi,GiaTienCu,MoTa,AnhSp,MaVatLieu,MaDanhMuc,NgayTao,MaNhaCungCap")] SanPham sanPham)
        {

            _context.Add(sanPham);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        // GET: SanPhams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham == null)
            {
                return NotFound();
            }
            ViewData["MaDanhMuc"] = new SelectList(_context.DanhMucs, "MaDanhMuc", "MaDanhMuc", sanPham.MaDanhMuc);
            ViewData["MaNhaCungCap"] = new SelectList(_context.NhaCungCaps, "MaNhaCungCap", "MaNhaCungCap", sanPham.MaNhaCungCap);
            ViewData["MaVatLieu"] = new SelectList(_context.VatLieus, "MaVatLieu", "MaVatLieu", sanPham.MaVatLieu);
            return View(sanPham);
        }

        // POST: SanPhams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaSanPham,TenSanPham,GiaTienMoi,GiaTienCu,MoTa,AnhSp,MaVatLieu,MaDanhMuc,NgayTao,MaNhaCungCap")] SanPham sanPham)
        {
            if (id != sanPham.MaSanPham)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sanPham);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SanPhamExists(sanPham.MaSanPham))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaDanhMuc"] = new SelectList(_context.DanhMucs, "MaDanhMuc", "MaDanhMuc", sanPham.MaDanhMuc);
            ViewData["MaNhaCungCap"] = new SelectList(_context.NhaCungCaps, "MaNhaCungCap", "MaNhaCungCap", sanPham.MaNhaCungCap);
            ViewData["MaVatLieu"] = new SelectList(_context.VatLieus, "MaVatLieu", "MaVatLieu", sanPham.MaVatLieu);
            return View(sanPham);
        }

        // GET: SanPhams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams
                .Include(s => s.MaDanhMucNavigation)
                .Include(s => s.MaNhaCungCapNavigation)
                .Include(s => s.MaVatLieuNavigation)
                .FirstOrDefaultAsync(m => m.MaSanPham == id);
            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }

        // POST: SanPhams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham != null)
            {
                _context.SanPhams.Remove(sanPham);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SanPhamExists(int id)
        {
            return _context.SanPhams.Any(e => e.MaSanPham == id);
        }
    }
}
