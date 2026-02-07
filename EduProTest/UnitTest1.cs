using EduProManagement.Models;
using EduProManagement.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using System;
using EduProManagement.Services;

namespace EduProTest
{
    public class CourseServiceTests
    {
        private List<Course> GetCourses()
        {
            return new List<Course>
            {
                new Course { Name = "C# Basics", StartDate = new DateOnly(2024, 1, 10) },
                new Course { Name = "ASP.NET", StartDate = new DateOnly(2024, 3, 1) },
                new Course { Name = "Java", StartDate = new DateOnly(2023, 12, 5) }
            };
        }

        [Fact]
        public void GetCourses_NoFilters_ReturnsAllCourses()
        {
            var service = new CourseService();

            var result = service.GetCourses(GetCourses(), null, null);

            Assert.Equal(3, result.Count);
        }

        [Fact]
        public void GetCourses_SearchByName_ReturnsFilteredCourses()
        {
            var service = new CourseService();

            var result = service.GetCourses(GetCourses(), "c#", null);

            Assert.Single(result);
            Assert.Equal("C# Basics", result.First().Name);
        }

        [Fact]
        public void GetCourses_SortByDateAscending_ReturnsCorrectOrder()
        {
            var service = new CourseService();

            var result = service.GetCourses(GetCourses(), null, "По возрастанию");

            Assert.True(result[0].StartDate <= result[1].StartDate);
        }

        [Fact]
        public void GetCourses_SortByDateDescending_ReturnsCorrectOrder()
        {
            var service = new CourseService();

            var result = service.GetCourses(GetCourses(), null, "По убыванию");

            Assert.True(result[0].StartDate >= result[1].StartDate);
        }

        [Fact]
        public void GetCourses_SearchAndSort_WorksCorrectly()
        {
            var service = new CourseService();

            var result = service.GetCourses(GetCourses(), "a", "По убыванию");

            Assert.Equal(3, result.Count);
            Assert.True(result[0].StartDate >= result[1].StartDate);
        }

    }

}