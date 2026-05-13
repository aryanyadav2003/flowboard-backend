using FlowBoard.CardService.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlowBoard.CardService.Data;

public class CardDbContext : DbContext
{
    public CardDbContext(DbContextOptions<CardDbContext> options)
        : base(options) { }

    public DbSet<Card>         Cards         => Set<Card>();
    public DbSet<CardAssignee> CardAssignees => Set<CardAssignee>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Card>(e =>
        {
            e.HasIndex(c => c.ListId);
            e.HasIndex(c => c.BoardId);
            e.HasIndex(c => new { c.ListId, c.Position });

            e.Property(c => c.Status).HasDefaultValue("TO_DO");
            e.Property(c => c.Priority).HasDefaultValue("MEDIUM");
            e.Property(c => c.Position).HasDefaultValue(0);
            e.Property(c => c.IsArchived).HasDefaultValue(false);
            e.Property(c => c.CreatedAt).HasDefaultValueSql("now()");
            e.Property(c => c.UpdatedAt).HasDefaultValueSql("now()");
        });

        modelBuilder.Entity<CardAssignee>(e =>
        {
            // One user can only be assigned once per card
            e.HasIndex(a => new { a.CardId, a.UserId }).IsUnique();
            e.Property(a => a.AssignedAt).HasDefaultValueSql("now()");

            e.HasOne(a => a.Card)
             .WithMany(c => c.Assignees)
             .HasForeignKey(a => a.CardId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}