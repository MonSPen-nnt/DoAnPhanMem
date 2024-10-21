using BanTrangSuc.Data;
using Microsoft.AspNetCore.Mvc;

namespace BanTrangSuc.Controllers
{
    public class AdminHomeController : Controller
    {
        private readonly DapmTrangContext db;

        public AdminHomeController(DapmTrangContext db)
        {
            this.db = db;
        }

        public IActionResult Index()
        {
            var donhang = db.DonHangs;
            int demSanPham = db.SanPhams.Count();
            int demUser = db.NguoiDungs.Count();
            int demDonHang = donhang.Count();
            int donHangThanhCong = donhang.Where(d => d.TrangThai == "Thành công").Count();
            int donHangDaHuy = donhang.Where(d => d.TrangThai == "Đã hủy").Count();


            double doanhThu = 0;
            if (donhang != null)
            {
                if (donhang.Where(d => d.TrangThai == "Thành công").Count() > 0)
                {
                    doanhThu = donhang.Where(d => d.TrangThai == "Thành công").Sum(s => s.TongSoTien);
                }
            }


            ViewBag.demSanPham = demSanPham;
            ViewBag.demUser = demUser;
            ViewBag.demDonHang = demDonHang;
            ViewBag.donHangThanhCong = donHangThanhCong;
            ViewBag.donHangDaHuy = donHangDaHuy;
            ViewBag.doanhThu = doanhThu;
            return View();
        }
    }
}
