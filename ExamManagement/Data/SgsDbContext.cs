using ExamManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExamManagement.Data
{
    public class SgsDbContext : DbContext
    {
        public SgsDbContext(DbContextOptions<SgsDbContext> options) : base(options)
        {
        }

        // DbSet properties for each entity
        public DbSet<Student> Student { get; set; }
        public DbSet<Teacher> Teacher { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<AssignmentQuestion> AssignmentQuestions { get; set; }
        public DbSet<FeedBack> FeedBacks { get; set; }
        public DbSet<StudentAssignment> StudentAssignments { get; set; }
        public DbSet<TeacherAssignment> TeacherAssignments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure primary keys
            modelBuilder.Entity<Student>().HasKey(std => std.StudentId);
            modelBuilder.Entity<Teacher>().HasKey(tch => tch.TeacherId);
            modelBuilder.Entity<Assignment>().HasKey(a => a.Id);
            modelBuilder.Entity<Question>().HasKey(q => q.Id);
            modelBuilder.Entity<AssignmentQuestion>().HasKey(aq => new { aq.AssignmentId, aq.QuestionId });
            modelBuilder.Entity<FeedBack>().HasKey(fb => fb.Id);
            modelBuilder.Entity<StudentAssignment>().HasKey(sa => new { sa.StudentId, sa.AssignmentId });
            modelBuilder.Entity<TeacherAssignment>().HasKey(ta => new { ta.TeacherId, ta.AssignmentId });

            // Configure relationships

            // Many-to-Many Relationships (using fluent API for clarity)
            modelBuilder.Entity<StudentAssignment>()
                .HasOne(sa => sa.Student)
                .WithMany(s => s.StudentAssignments)
                .HasForeignKey(sa => sa.StudentId);

            modelBuilder.Entity<TeacherAssignment>()
                .HasOne(ta => ta.Teacher)
                .WithMany(t => t.TeacherAssignments)
                .HasForeignKey(ta => ta.TeacherId);

            // One-to-Many Relationships
            modelBuilder.Entity<Assignment>()
                .HasMany(a => a.StudentAssignments)
                .WithOne(sa => sa.Assignments)
                .HasForeignKey(sa => sa.AssignmentId);

            modelBuilder.Entity<Assignment>()
                .HasMany(a => a.TeacherAssignments)
                .WithOne(ta => ta.Assignment)
                .HasForeignKey(ta => ta.AssignmentId);

            // 2. Cascade Delete Behavior (Customize as needed)
            modelBuilder.Entity<Assignment>()
                .HasMany(a => a.AssignmentQuestions)
                .WithOne(aq => aq.Assignment)
                .HasForeignKey(aq => aq.AssignmentId);
            //.OnDelete(DeleteBehavior.Cascade); // Example: Delete questions when assignment is deleted

            // Many-to-1 relationship between Question and AssignmentQuestion
            modelBuilder.Entity<AssignmentQuestion>()
                .HasOne(aq => aq.Question)
                .WithMany(q => q.AssignmentQuestion)
                .HasForeignKey(aq => aq.QuestionId);

            // Many-to-1 relationship between Assignment and AssignmentQuestion
            modelBuilder.Entity<AssignmentQuestion>()
                .HasOne(aq => aq.Assignment)
                .WithMany(a => a.AssignmentQuestions)
                .HasForeignKey(aq => aq.AssignmentId);

            modelBuilder.Entity<FeedBack>()
                .HasOne(q => q.QuestionResponse)
                .WithMany(fb => fb.QuestionFeedback);
        }
    }
}
