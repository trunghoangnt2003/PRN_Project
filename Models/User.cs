using System;
using System.Collections.Generic;

namespace PRN_Project.Models;

public partial class User
{
    public int Id { get; set; }

    public string DisplayName { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int IdRole { get; set; }

    public virtual ICollection<Deliver> Delivers { get; set; } = new List<Deliver>();

    public virtual UserRole IdRoleNavigation { get; set; } = null!;

    public virtual ICollection<Receive> Receives { get; set; } = new List<Receive>();
}
