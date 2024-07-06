using System;
using System.Collections.Generic;

namespace PRN_Project.Models;

public partial class Deliver
{
    public int Id { get; set; }

    public DateOnly DateOutput { get; set; }

    public int IdUser { get; set; }


    public virtual ICollection<DeliverInfo> DeliverInfos { get; set; } = new List<DeliverInfo>();


    public virtual User IdUserNavigation { get; set; } = null!;
}
