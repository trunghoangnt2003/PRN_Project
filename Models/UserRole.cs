using System;
using System.Collections.Generic;
namespace PRN_Project.Models;
public partial class UserRole
{
    public int Id { get; set; }

    public string DisplayName { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
