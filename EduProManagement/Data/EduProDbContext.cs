using System;
using System.Collections.Generic;
using EduProManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace EduProManagement.Data;

public partial class EduProDbContext : DbContext
{
    public EduProDbContext()
    {
    }

    public EduProDbContext(DbContextOptions<EduProDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseType> CourseTypes { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<RequestStatus> RequestStatuses { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<TeacherType> TeacherTypes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-MN81TOI;Database=EduProDb;TrustServerCertificate=True;Trusted_Connection=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Company>(entity =>
        {
            entity.ToTable("Company");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.ToTable("Course");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.FileName).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.CourseType).WithMany(p => p.Courses)
                .HasForeignKey(d => d.CourseTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Course_CourseType");

            entity.HasOne(d => d.TeacherType).WithMany(p => p.Courses)
                .HasForeignKey(d => d.TeacherTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Course_TeacherType");
        });

        modelBuilder.Entity<CourseType>(entity =>
        {
            entity.ToTable("CourseType");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Description).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.ToTable("Request");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Comment).HasMaxLength(50);

            entity.HasOne(d => d.Course).WithMany(p => p.Requests)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Request_Course");

            entity.HasOne(d => d.Status).WithMany(p => p.Requests)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Request_RequestStatus");

            entity.HasOne(d => d.User).WithMany(p => p.Requests)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Request_User");
        });

        modelBuilder.Entity<RequestStatus>(entity =>
        {
            entity.ToTable("RequestStatus");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<TeacherType>(entity =>
        {
            entity.ToTable("TeacherType");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Description).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.FullName).HasMaxLength(50);
            entity.Property(e => e.Login).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(50);

            entity.HasOne(d => d.Company).WithMany(p => p.Users)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Company");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
