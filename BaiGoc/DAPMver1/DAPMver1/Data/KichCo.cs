﻿using System;
using System.Collections.Generic;

namespace DAPMver1.Data;

public partial class KichCo
{
    public int MaKichCo { get; set; }

    public int? MaSanPham { get; set; }

    public int SoKichCo { get; set; }

    public int SoLuong { get; set; }

    public virtual SanPham? MaSanPhamNavigation { get; set; }
}
