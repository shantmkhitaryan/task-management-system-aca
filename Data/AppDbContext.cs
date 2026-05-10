using Microsoft.EntityFrameworkCore;
using task_management_system_aca.Entities;

namespace task_management_system_aca.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Board> Boards { get; set; }
    public DbSet<Section> Sections { get; set; }
    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<BoardInvite> BoardInvites { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        
        modelBuilder.Entity<Board>()
            .HasIndex(b => b.Sku)
            .IsUnique();

        modelBuilder.Entity<Board>()
            .HasOne(b => b.Owner)
            .WithMany(u => u.Boards)
            .HasForeignKey(b => b.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        
        modelBuilder.Entity<Section>()
            .HasOne(s => s.Board)
            .WithMany(b => b.Sections)
            .HasForeignKey(s => s.BoardId)
            .OnDelete(DeleteBehavior.Cascade);

        
        modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.Board)
            .WithMany(b => b.Tasks)
            .HasForeignKey(t => t.BoardId);

        modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.Section)
            .WithMany(s => s.Tasks)
            .HasForeignKey(t => t.SectionId);

        modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.Assignee)
            .WithMany(u => u.AssignedTasks)
            .HasForeignKey(t => t.AssigneeId)
            .IsRequired(false);

        modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.Creator)
            .WithMany()
            .HasForeignKey(t => t.CreatedBy)
            .IsRequired(true);

        
        modelBuilder.Entity<BoardInvite>()
            .HasOne(i => i.Board)
            .WithMany()
            .HasForeignKey(i => i.BoardId);

        modelBuilder.Entity<BoardInvite>()
            .HasOne(i => i.InvitedUser)
            .WithMany()
            .HasForeignKey(i => i.InvitedUserId);

        modelBuilder.Entity<BoardInvite>()
            .HasOne(i => i.Inviter)
            .WithMany()
            .HasForeignKey(i => i.InvitedBy);

        base.OnModelCreating(modelBuilder);
    }
}