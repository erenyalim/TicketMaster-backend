using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TicketMaster.Domain.Abstraction;
using TicketMaster.Domain.Entities;
using TicketMaster.Domain.Events;
using TicketMaster.Domain.Tickets;
using TicketMaster.Domain.Users;

namespace TicketMaster.Infrastructure.Context;

internal sealed class AppDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        IHttpContextAccessor httpContextAccessor
        ) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        _httpContextAccessor = new HttpContextAccessor();
    }

    public DbSet<Event> Events { get; set; }
    public DbSet<TicketType> TicketTypes { get; set; }
    public DbSet<Banner> Banners => Set<Banner>();


    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditValues();

        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        ApplyAuditValues();

        return base.SaveChanges();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        modelBuilder.Entity<Event>()
            .HasQueryFilter(entity => !entity.IsDeleted);

        modelBuilder.Entity<TicketType>()
            .HasQueryFilter(entity => !entity.IsDeleted);

        modelBuilder.Entity<AppUser>()
            .HasQueryFilter(entity => !entity.IsDeleted);
    }

    private void ApplyAuditValues()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;

        string userId = _httpContextAccessor.HttpContext?
            .User?
            .FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";

        var entries = ChangeTracker.Entries().ToList();

        foreach (var entry in entries)
        {
            if (entry.Entity is not IAuditable)
            {
                continue;
            }

            if (entry.State == EntityState.Added)
            {
                entry.Property(nameof(Entity.CreatedAt)).CurrentValue = now;
                entry.Property(nameof(Entity.CreateUserId)).CurrentValue = userId;
                entry.Property(nameof(Entity.IsDeleted)).CurrentValue = false;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Property(nameof(Entity.UpdatedAt)).CurrentValue = now;
                entry.Property(nameof(Entity.UpdatedUserId)).CurrentValue = userId;
            }
            else if (entry.State == EntityState.Deleted &&
                     entry.Entity is ISoftDeletable)
            {
                entry.State = EntityState.Modified;
                entry.Property(nameof(Entity.IsDeleted)).CurrentValue = true;
                entry.Property(nameof(Entity.DeletedAt)).CurrentValue = now;
                entry.Property(nameof(Entity.DeletedUserId)).CurrentValue = userId;

                RestoreOwnedEntities();
            }
        }
    }

    private void RestoreOwnedEntities()
    {
        foreach (EntityEntry entry in ChangeTracker.Entries()
            .Where(entry => entry.Metadata.IsOwned() &&
                            entry.State == EntityState.Deleted))
        {
            entry.State = EntityState.Unchanged;
        }
    }
}


