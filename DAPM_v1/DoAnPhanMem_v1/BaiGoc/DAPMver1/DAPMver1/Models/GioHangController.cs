﻿using Microsoft.AspNetCore.Mvc;

namespace DAPMver1.Models
{
    public class GioHangController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ThanhToan()
        {
            return View();
        }
        public IActionResult XacNhanThanhToan()
        {
            return View();
        }
    }
}
