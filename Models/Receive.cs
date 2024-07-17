using System;
using System.Collections.Generic;

namespace PRN_Project.Models;

public partial class Receive
{
    public int Id { get; set; }

    public DateOnly DateInput { get; set; }

    public int IdUser { get; set; }

    public bool Status { get; set; }

    public virtual User IdUserNavigation { get; set; } = null!;

    public virtual ICollection<ReceiveInfo> ReceiveInfos { get; set; } = new List<ReceiveInfo>();
}
