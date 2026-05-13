using FlowBoard.WorkspaceService.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlowBoard.WorkspaceService.Data;

public class WorkspaceDbContext : DbContext
{
    public WorkspaceDbContext(DbContextOptions<WorkspaceDbContext> options)
        : base(options) { }

    public DbSet<Workspace>       Workspaces       => Set<Workspace>();
    public DbSet<WorkspaceMember> WorkspaceMembers => Set<WorkspaceMember>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Workspace>(e =>
        {
            e.HasIndex(w => w.OwnerId);
            e.Property(w => w.Visibility).HasDefaultValue("PRIVATE");
            e.Property(w => w.IsActive).HasDefaultValue(true);
            e.Property(w => w.CreatedAt).HasDefaultValueSql("now()");
            e.Property(w => w.UpdatedAt).HasDefaultValueSql("now()");
        });

        modelBuilder.Entity<WorkspaceMember>(e =>
        {
            e.HasIndex(m => new { m.WorkspaceId, m.UserId }).IsUnique();
            e.Property(m => m.Role).HasDefaultValue("MEMBER");
            e.Property(m => m.JoinedAt).HasDefaultValueSql("now()");

            e.HasOne(m => m.Workspace)
             .WithMany(w => w.Members)
             .HasForeignKey(m => m.WorkspaceId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}