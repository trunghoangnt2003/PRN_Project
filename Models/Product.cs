using System;
using System.Collections.Generic;

namespace PRN_Project.Models;

public partial class Product
{
    public int Id { get; set; }

    public string DisplayName { get; set; } = null!;

    public int IdUnit { get; set; }

    public int IdSuplier { get; set; }

    public string? Qrcode { get; set; }

    public int Count { get; set; }

    public double OutPrice { get; set; }

    public virtual ICollection<DeliverInfo> DeliverInfos { get; set; } = new List<DeliverInfo>();

    public virtual Suplier IdSuplierNavigation { get; set; } = null!;

    public virtual Unit IdUnitNavigation { get; set; } = null!;

    public virtual ICollection<ReceiveInfo> ReceiveInfos { get; set; } = new List<ReceiveInfo>();
}
