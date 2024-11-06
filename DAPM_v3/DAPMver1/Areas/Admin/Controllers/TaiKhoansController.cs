using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAPMver1.Data;

namespace DAPMver1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TaiKhoansController : Controller
    {
        private readonly DapmTrangv1Context _context;

        public TaiKhoansController(DapmTrangv1Context context)
        {
            _context = context;
        }

        // GET: Admin/TaiKhoans
        public async Task<IActionResult> Index()
        {
            return View(await _context.TaiKhoans.ToListAsync());
        }

        // GET: Admin/TaiKhoans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taiKhoan = await _context.TaiKhoans
                .FirstOrDefaultAsync(m => m.MaTaiKhoan == id);
            if (taiKhoan == null)
            {
                return NotFound();
            }

            return View(taiKhoan);
        }

        // GET: Admin/TaiKhoans/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/TaiKhoans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaTaiKhoan,Email,MatKhau,VaiTro")] TaiKhoan taiKhoan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(taiKhoan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(taiKhoan);
        }

        // GET: Admin/TaiKhoans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taiKhoan = await _context.TaiKhoans.FindAsync(id);
            if (taiKhoan == null)
            {
                return NotFound();
            }
            return View(taiKhoan);
        }

        // POST: Admin/TaiKhoans/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaTaiKhoan,Email,MatKhau,VaiTro")] TaiKhoan taiKhoan)
        {
            if (id != taiKhoan.MaTaiKhoan)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(taiKhoan);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật tài khoản thành công."; // Feedback message
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaiKhoanExists(taiKhoan.MaTaiKhoan))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật tài khoản. Vui lòng thử lại."); // User feedback on error
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            // Return the view with the current model state to show validation errors if any.
            return View(taiKhoan);
        }

        // GET: Admin/TaiKhoans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taiKhoan = await _context.TaiKhoans
                .FirstOrDefaultAsync(m => m.MaTaiKhoan == id);
            if (taiKhoan == null)
            {
                return NotFound();
            }

            return View(taiKhoan);
        }

        // POST: Admin/TaiKhoans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var taiKhoan = await _context.TaiKhoans.FindAsync(id);
            if (taiKhoan != null)
            {
                _context.TaiKhoans.Remove(taiKhoan);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaiKhoanExists(int id)
        {
            return _context.TaiKhoans.Any(e => e.MaTaiKhoan == id);
        }
        // GET: Admin/TaiKhoans/Disable/5
        public async Task<IActionResult> Disable(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taiKhoan = await _context.TaiKhoans.FindAsync(id);
            if (taiKhoan == null)
            {
                return NotFound();
            }

            return View(taiKhoan); // Hiển thị view để xác nhận việc vô hiệu hóa
        }

        // POST: Admin/TaiKhoans/Disable/5
        [HttpPost, ActionName("Disable")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DisableConfirmed(int id)
        {
            var taiKhoan = await _context.TaiKhoans.FindAsync(id);
            if (taiKhoan == null)
            {
                return NotFound();
            }

            taiKhoan.VaiTro = true; // Cập nhật trạng thái tài khoản thành không kích hoạt
            _context.Update(taiKhoan);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Tài khoản đã được vô hiệu hóa thành công.";
            return RedirectToAction(nameof(Index));
        }
        // GET: Admin/TaiKhoans/Enable/5
        public async Task<IActionResult> Enable(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taiKhoan = await _context.TaiKhoans.FindAsync(id);
            if (taiKhoan == null)
            {
                return NotFound();
            }

            return View(taiKhoan); // Hiển thị view để xác nhận việc kích hoạt
        }

        // POST: Admin/TaiKhoans/Enable/5
        [HttpPost, ActionName("Enable")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableConfirmed(int id)
        {
            var taiKhoan = await _context.TaiKhoans.FindAsync(id);
            if (taiKhoan == null)
            {
                return NotFound();
            }

            taiKhoan.VaiTro = false; // Đổi trạng thái sang kích hoạt
            _context.Update(taiKhoan);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Tài khoản đã được kích hoạt thành công.";
            return RedirectToAction(nameof(Index));
        }








    }
}

