using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MaharaFinalVersion.Models;


namespace MaharaFinalVersion.Data


{
    public class Mahara2DbContext : IdentityDbContext<User>
    {
        public Mahara2DbContext(DbContextOptions<Mahara2DbContext> options)
            : base(options)
        {
        }

        public DbSet<Session> Sessions { get; set; } = null!;
        public DbSet<StudentSession> StudentSessions { get; set; } = null!;
        public DbSet<InstructorPromotion> InstructorPromotions { get; set; } = null!;
         public DbSet<SessionInteraction> SessionInteractions { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); 

            // -------------------- Session --------------------
            builder.Entity<Session>(entity =>
            {
                entity.HasKey(e => e.Id)
                      .HasName("PK_Sessions");

                entity.Property(e => e.CreatedAt)
                      .HasDefaultValueSql("(getdate())")
                      .HasColumnType("datetime");

                entity.Property(e => e.CreatorId).HasMaxLength(450);
                entity.Property(e => e.Skill).HasMaxLength(50);
                entity.Property(e => e.Title).HasMaxLength(200);

                entity.HasOne(d => d.Creator)
                      .WithMany(p => p.Sessions)
                      .HasForeignKey(d => d.CreatorId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_Sessions_Creator");
            });

            // -------------------- StudentSession --------------------
            builder.Entity<StudentSession>(entity =>
            {
                entity.HasKey(e => e.Id)
                      .HasName("PK_StudentSessions");

                entity.Property(e => e.StudentId).HasMaxLength(450);

                entity.HasOne(d => d.Session)
                      .WithMany(p => p.StudentSession)
                      .HasForeignKey(d => d.SessionId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_StudentSessions_Session");

                entity.HasOne(d => d.Student)
                      .WithMany(p => p.StudentSessions)
                      .HasForeignKey(d => d.StudentId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_StudentSessions_Student");
            });

            // -------------------- InstructorPromotion --------------------
            builder.Entity<InstructorPromotion>(entity =>
            {
                entity.HasKey(e => e.Id)
                      .HasName("PK_InstructorPromotions");

                entity.Property(e => e.PromotedAt)
                      .HasDefaultValueSql("(getdate())")
                      .HasColumnType("datetime");

                entity.Property(e => e.StudentId).HasMaxLength(450);

                entity.HasOne(d => d.Student)
                      .WithMany(p => p.InstructorPromotions)
                      .HasForeignKey(d => d.StudentId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_InstructorPromotions_Student");
            });
                // -------------------- SessionInteraction --------------------
            builder.Entity<SessionInteraction>(entity =>
            {
                entity.HasKey(e => e.Id)
                      .HasName("PK_SessionInteractions");

                entity.Property(e => e.Comment).IsRequired();
                entity.Property(e => e.CreatedAt)
                      .HasDefaultValueSql("(getdate())")
                      .HasColumnType("datetime");

                entity.Property(e => e.UserId).HasMaxLength(450);

                entity.HasOne(d => d.Session)
                      .WithMany(p => p.Interactions)
                      .HasForeignKey(d => d.SessionId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .HasConstraintName("FK_SessionInteractions_Session");

                entity.HasOne(d => d.User)
          .WithMany(p => p.SessionInteractions)
          .HasForeignKey(d => d.UserId)
          .OnDelete(DeleteBehavior.Cascade)
          .HasConstraintName("FK_SessionInteractions_User");
});
        }
        
    }
}
