using FlowBoard.BoardService.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlowBoard.BoardService.Data;

public class BoardDbContext : DbContext
{
    public BoardDbContext(DbContextOptions<BoardDbContext> options) : base(options) { }

    public DbSet<Board>       Boards       => Set<Board>();
    public DbSet<BoardMember> BoardMembers => Set<BoardMember>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Board>(e =>
        {
            e.HasIndex(b => b.WorkspaceId);
            e.HasIndex(b => b.OwnerId);
            e.Property(b => b.Visibility).HasDefaultValue("PRIVATE");
            e.Property(b => b.IsArchived).HasDefaultValue(false);
            e.Property(b => b.IsActive).HasDefaultValue(true);
            e.Property(b => b.CreatedAt).HasDefaultValueSql("now()");
            e.Property(b => b.UpdatedAt).HasDefaultValueSql("now()");
        });

        modelBuilder.Entity<BoardMember>(e =>
        {
            e.HasIndex(m => new { m.BoardId, m.UserId }).IsUnique();
            e.Property(m => m.Role).HasDefaultValue("MEMBER");
            e.Property(m => m.JoinedAt).HasDefaultValueSql("now()");

            e.HasOne(m => m.Board)
             .WithMany(b => b.Members)
             .HasForeignKey(m => m.BoardId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}