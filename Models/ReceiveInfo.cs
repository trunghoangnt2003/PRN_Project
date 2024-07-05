using System;
using System.Collections.Generic;

namespace PRN_Project.Models;

public partial class ReceiveInfo
{
    public int Id { get; set; }

    public int IdProduct { get; set; }

    public int IdReceive { get; set; }

    public int Count { get; set; }

    public double InputPrice { get; set; }

    public virtual Product IdProductNavigation { get; set; } = null!;

    public virtual Receive IdReceiveNavigation { get; set; } = null!;
}
