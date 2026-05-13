using FlowBoard.ListService.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlowBoard.ListService.Data;

public class ListDbContext : DbContext
{
    public ListDbContext(DbContextOptions<ListDbContext> options)
        : base(options) { }

    public DbSet<TaskList> TaskLists => Set<TaskList>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TaskList>(e =>
        {
            // Index on BoardId for fast board-based queries
            e.HasIndex(l => l.BoardId);

            // Index on BoardId + Position for ordering
            e.HasIndex(l => new { l.BoardId, l.Position });

            e.Property(l => l.Position).HasDefaultValue(0);
            e.Property(l => l.IsArchived).HasDefaultValue(false);
            e.Property(l => l.CreatedAt).HasDefaultValueSql("now()");
            e.Property(l => l.UpdatedAt).HasDefaultValueSql("now()");
        });
    }
}