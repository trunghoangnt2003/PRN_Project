using System;
using System.Collections.Generic;

namespace PRN_Project.Models;

public partial class Suplier
{
    public int Id { get; set; }

    public string DisplayName { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string MoreInfo { get; set; } = null!;

    public DateOnly ContractDate { get; set; }


    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
