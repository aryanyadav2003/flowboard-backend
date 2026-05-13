using FlowBoard.ChecklistService.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlowBoard.ChecklistService.Data;

public class ChecklistDbContext : DbContext
{
    public ChecklistDbContext(DbContextOptions<ChecklistDbContext> options)
        : base(options) { }

    public DbSet<Checklist>     Checklists     => Set<Checklist>();
    public DbSet<ChecklistItem> ChecklistItems => Set<ChecklistItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Checklist>(e =>
        {
            // Fast lookup by card
            e.HasIndex(c => c.CardId);
            e.Property(c => c.CreatedAt).HasDefaultValueSql("now()");
            e.Property(c => c.UpdatedAt).HasDefaultValueSql("now()");
        });

        modelBuilder.Entity<ChecklistItem>(e =>
        {
            // Fast lookup by checklist
            e.HasIndex(i => i.ChecklistId);
            e.HasIndex(i => new { i.ChecklistId, i.Position });

            e.Property(i => i.IsCompleted).HasDefaultValue(false);
            e.Property(i => i.Position).HasDefaultValue(0);
            e.Property(i => i.CreatedAt).HasDefaultValueSql("now()");
            e.Property(i => i.UpdatedAt).HasDefaultValueSql("now()");

            // Cascade delete — when checklist deleted, items deleted too
            e.HasOne(i => i.Checklist)
             .WithMany(c => c.Items)
             .HasForeignKey(i => i.ChecklistId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}