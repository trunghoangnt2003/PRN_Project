using System;
using System.Collections.Generic;

namespace PRN_Project.Models;

public partial class DeliverInfo
{
    public int Id { get; set; }

    public int IdProduct { get; set; }

    public int Count { get; set; }

    public double OutputPrice { get; set; }

    public int IdDeliver { get; set; }

    public virtual Deliver IdDeliverNavigation { get; set; } = null!;

    public virtual Product IdProductNavigation { get; set; } = null!;
}
