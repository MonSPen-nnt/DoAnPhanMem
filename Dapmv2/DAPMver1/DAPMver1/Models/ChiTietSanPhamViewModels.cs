using DAPMver1.Data;

namespace DAPMver1.Models
{
    public class ChiTietSanPhamViewModels
    {
        public SanPham SanPham { get; set; }
        public IEnumerable<SanPham> GoiYSanPhams { get; set; }
    }
}