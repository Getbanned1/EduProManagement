using System;
using System.Collections.Generic;

namespace EduProManagement.Models;

public partial class User
{
    public int Id { get; set; }

    public int RoleId { get; set; }

    public string FullName { get; set; } = null!;

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int CompanyId { get; set; }

    public string? Email { get; set; }

    public virtual Company Company { get; set; } = null!;

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();

    public virtual Role Role { get; set; } = null!;
}
