using EduProManagement.Models;
using System;
using System.Collections.Generic;

namespace EduProManagement;

public partial class Request
{
    public int Id { get; set; }

    public int CourseId { get; set; }

    public int UserId { get; set; }

    public DateOnly Date { get; set; }

    public int StatusId { get; set; }

    public int TotalCost { get; set; }

    public string? Comment { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual RequestStatus Status { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
