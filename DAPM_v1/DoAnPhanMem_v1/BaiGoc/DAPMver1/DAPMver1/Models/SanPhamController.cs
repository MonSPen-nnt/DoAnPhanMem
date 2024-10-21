using Microsoft.AspNetCore.Mvc;

namespace DAPMver1.Models
{
    public class SanPhamController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ChiTietSP()
        {
            return View();
        }
    }
}
