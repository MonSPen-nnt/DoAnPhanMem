using System;
using System.Collections.Generic;

namespace DAPMver1.Data;

public partial class ChiTietGioHang
{
    public int MaGioHang { get; set; }

    public int MaKichCo { get; set; }

    public int SoLuong { get; set; }

    public double GiaBan { get; set; }

    public virtual GioHang MaGioHangNavigation { get; set; }

<<<<<<< HEAD
    public virtual KichCo MaKichCoNavigation { get; set; }
=======
    public virtual KichCo MaKichCoNavigation { get; set; } = null!;
>>>>>>> bcdbd4465becac8ac1932712dcb28487a1627c7b
}
