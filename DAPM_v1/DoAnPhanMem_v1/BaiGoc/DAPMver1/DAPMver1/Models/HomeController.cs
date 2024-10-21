using DAPMver1.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DAPMver1.Models
{
    [Route("home")]
    public class HomeController : Controller
    {
        [Route("index")]
        public IActionResult Index()
        {
            return View();
        }
   

  
    }
   
}
