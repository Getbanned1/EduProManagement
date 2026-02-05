using System;
using System.Collections.Generic;

namespace EduProManagement.Models;

public partial class Course
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int CourseTypeId { get; set; }

    public int Duration { get; set; }

    public DateOnly StartDate { get; set; }

    public int Price { get; set; }

    public int TeacherTypeId { get; set; }

    public int AvaliableSpace { get; set; }

    public string FileName { get; set; } = null!;

    public string FilePath => string.IsNullOrEmpty(FileName) ? "Images/deafult.png" : $"Images/{FileName}";
    public string LowSeats => AvaliableSpace > TeacherType.Capacity * 0.1 ? "White" : "#FFB6C1";

    public string StartsSoon => StartDate > DateOnly.FromDateTime(DateTime.Today) &&
    StartDate.DayNumber - DateOnly.FromDateTime(DateTime.Today).DayNumber < 7
        ? "Bold"
        : "Normal";

    public virtual CourseType CourseType { get; set; } = null!;

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();

    public virtual TeacherType TeacherType { get; set; } = null!;
}
