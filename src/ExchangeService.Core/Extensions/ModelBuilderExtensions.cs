using ExchangeService.Core.Constants;
using ExchangeService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExchangeService.Core.Extensions;

public static class ModelBuilderExtensions
{
    public static void AddModel<T>(this ModelBuilder modelBuilder, Action<EntityTypeBuilder<T>> buildAction = null)
        where T : EntityBase
    {
        modelBuilder.Entity<T>(entity =>
        {
            entity.HasKey(w => w.Id);
            entity.Property(w => w.CreatedAt).IsRequired();
            entity.Property(w => w.ModifiedAt);
            entity.Property(w => w.RecordStatus).IsRequired();
            entity.Property(w => w.ModifiedBy).HasMaxLength(50);
            entity.Property(w => w.CreatedBy).IsRequired().HasMaxLength(50);
        });
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(T).IsAssignableFrom(entityType.ClrType))
            {
                entityType.AddSoftDeleteQueryFilter();
            }
        }

        if (buildAction != null) modelBuilder.Entity<T>(buildAction);
    }

    public static void SetCommonProperties(this ChangeTracker changeTracker, int? modifiedBy)
    {
        changeTracker.DetectChanges();
        IEnumerable<EntityEntry> entities =
            changeTracker
                .Entries()
                .Where(t => t.Entity is EntityBase &&
                            t.State is EntityState.Deleted or EntityState.Modified or EntityState.Added)
                .ToList();

        if (!entities.Any()) return;

        foreach (var entry in entities)
        {
            var entity = (EntityBase)entry.Entity;
            switch (entry.State)
            {
                case EntityState.Deleted:
                    entity.RecordStatus = RecordStatusConstants.Deleted;
                    entity.ModifiedAt = DateTime.Now;
                    entity.ModifiedBy = modifiedBy;
                    entry.State = EntityState.Modified;
                    break;
                case EntityState.Modified:
                    entity.ModifiedAt = DateTime.Now;
                    entity.ModifiedBy = modifiedBy;
                    entry.State = EntityState.Modified;
                    break;
                case EntityState.Added:
                    entity.CreatedAt = DateTime.Now;
                    entity.CreatedBy = modifiedBy ?? int.MinValue;
                    entity.RecordStatus = RecordStatusConstants.Active;
                    break;
            }
        }
    }
}