using System;
using System.Collections.Generic;

namespace EduProManagement.Models;

public partial class CourseType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}
