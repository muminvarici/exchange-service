using ExchangeService.Core.Entities;
using ExchangeService.Core.Extensions;
using ExchangeService.Core.Infrastructure.Holders.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ExchangeService.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    private IHolder _holder;
    private bool _initialized;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.AddModel<ExchangeLog>(entity =>
        {
            entity.Property(w => w.SourceCurrencyCode).HasMaxLength(5).IsRequired();
            entity.Property(w => w.TargetCurrencyCode).HasMaxLength(5).IsRequired();
            entity.Property(w => w.SourceAmount).HasPrecision(2).IsRequired();
            entity.Property(w => w.TargetAmount).HasPrecision(2).IsRequired();
            entity.Property(w => w.Direction).HasPrecision(2).IsRequired();
            entity.Property(w => w.RateDate).IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }


    private IHolder GetHolder()
    {
        if (_initialized) return _holder;
        try
        {
            if (!_initialized)
                _holder = this.GetService<IHolder>();
        }
        catch (Exception)
        {
            // ignored
        }
        finally
        {
            _initialized = true;
        }

        return _holder;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ChangeTracker.SetCommonProperties(GetHolder()?.UserId);
        return await base.SaveChangesAsync(cancellationToken);
    }
}