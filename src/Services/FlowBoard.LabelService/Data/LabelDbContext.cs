using FlowBoard.LabelService.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlowBoard.LabelService.Data;

public class LabelDbContext : DbContext
{
    public LabelDbContext(DbContextOptions<LabelDbContext> options)
        : base(options) { }

    public DbSet<Label>     Labels     => Set<Label>();
    public DbSet<CardLabel> CardLabels => Set<CardLabel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Label>(e =>
        {
            // Fast lookup by board
            e.HasIndex(l => l.BoardId);
            e.Property(l => l.Color).HasDefaultValue("#6366F1");
            e.Property(l => l.CreatedAt).HasDefaultValueSql("now()");
        });

        modelBuilder.Entity<CardLabel>(e =>
        {
            // One label can only be assigned once per card
            e.HasIndex(cl => new { cl.CardId, cl.LabelId }).IsUnique();
            e.Property(cl => cl.AssignedAt).HasDefaultValueSql("now()");

            e.HasOne(cl => cl.Label)
             .WithMany(l => l.CardLabels)
             .HasForeignKey(cl => cl.LabelId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}