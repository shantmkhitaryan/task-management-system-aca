using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using task_management_system_aca.Entities;

namespace task_management_system_aca.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Board> Boards { get; set; }
    public DbSet<BoardMember> BoardMembers { get; set; }
    public DbSet<Section> Sections { get; set; }
    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<BoardInvite> BoardInvites { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(new ValueConverter<DateTime, DateTime>(
                        v => v.ToUniversalTime(),
                        v => DateTime.SpecifyKind(v, DateTimeKind.Utc)));
                }
            }
        }

        
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Username).IsUnique();
            entity.Property(u => u.Username).IsRequired().HasMaxLength(50);
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        
        modelBuilder.Entity<Board>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.HasIndex(b => b.Sku).IsUnique();
            entity.Property(b => b.Title).IsRequired().HasMaxLength(100);
            entity.Property(b => b.Sku).IsRequired().HasMaxLength(3);
            entity.Property(b => b.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            entity.HasOne(b => b.Owner)
                  .WithMany(u => u.Boards)
                  .HasForeignKey(b => b.OwnerId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        
        modelBuilder.Entity<BoardMember>(entity =>
        {
            entity.HasKey(bm => new { bm.BoardId, bm.UserId });
            
            entity.HasOne(bm => bm.Board)
                  .WithMany()
                  .HasForeignKey(bm => bm.BoardId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(bm => bm.User)
                  .WithMany()
                  .HasForeignKey(bm => bm.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            entity.Property(bm => bm.JoinedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

       
        modelBuilder.Entity<Section>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Name).IsRequired().HasMaxLength(100);
            entity.Property(s => s.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            entity.HasOne(s => s.Board)
                  .WithMany(b => b.Sections)
                  .HasForeignKey(s => s.BoardId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        
        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Title).IsRequired().HasMaxLength(200);
            entity.Property(t => t.Priority).HasMaxLength(20).HasDefaultValue("Medium");
            entity.Property(t => t.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            entity.HasOne(t => t.Board)
                  .WithMany(b => b.Tasks)
                  .HasForeignKey(t => t.BoardId);
            
            entity.HasOne(t => t.Section)
                  .WithMany(s => s.Tasks)
                  .HasForeignKey(t => t.SectionId);
            
            entity.HasOne(t => t.Assignee)
                  .WithMany(u => u.AssignedTasks)
                  .HasForeignKey(t => t.AssigneeId)
                  .IsRequired(false);
            
            entity.HasOne(t => t.Creator)
                  .WithMany()
                  .HasForeignKey(t => t.CreatedBy)
                  .IsRequired(true);
        });

        
        modelBuilder.Entity<BoardInvite>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.Property(i => i.Status).HasMaxLength(20).HasDefaultValue("Pending");
            entity.Property(i => i.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            entity.HasOne(i => i.Board)
                  .WithMany()
                  .HasForeignKey(i => i.BoardId);
            
            entity.HasOne(i => i.InvitedUser)
                  .WithMany()
                  .HasForeignKey(i => i.InvitedUserId);
            
            entity.HasOne(i => i.Inviter)
                  .WithMany()
                  .HasForeignKey(i => i.InvitedBy);
        });
    }
}