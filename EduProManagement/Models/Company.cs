using System;
using System.Collections.Generic;

namespace EduProManagement.Models;

public partial class Company
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
