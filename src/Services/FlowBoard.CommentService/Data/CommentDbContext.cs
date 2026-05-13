using FlowBoard.CommentService.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlowBoard.CommentService.Data;

public class CommentDbContext : DbContext
{
    public CommentDbContext(DbContextOptions<CommentDbContext> options)
        : base(options) { }

    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Comment>(e =>
        {
            // Fast lookup by card
            e.HasIndex(c => c.CardId);

            // Fast lookup by user
            e.HasIndex(c => c.UserId);

            e.Property(c => c.IsEdited).HasDefaultValue(false);
            e.Property(c => c.CreatedAt).HasDefaultValueSql("now()");
            e.Property(c => c.UpdatedAt).HasDefaultValueSql("now()");
        });
    }
}