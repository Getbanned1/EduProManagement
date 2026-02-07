using EduProManagement.Models;

namespace EduProManagement.Services
{
    public class CourseService
    {
        public List<Course> GetCourses(
            IEnumerable<Course> courses,
            string? search,
            string? dateSort)
        {
            var query = courses.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchValue = search.ToLower();
                query = query.Where(c => c.Name.ToLower().Contains(searchValue));
            }

            if (!string.IsNullOrEmpty(dateSort) && dateSort != "Все")
            {
                query = dateSort == "По возрастанию"
                    ? query.OrderBy(c => c.StartDate)
                    : query.OrderByDescending(c => c.StartDate);
            }

            return query.ToList();
        }
    }
}
