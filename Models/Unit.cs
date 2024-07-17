using System;
using System.Collections.Generic;
namespace PRN_Project.Models;

public partial class Unit
{
    public int Id { get; set; }

    public string DisplayName { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
