namespace GameVault.Domain.Common;

public abstract class AuditableEntity<TId> : Entity<TId>
    where TId : notnull
{
    protected AuditableEntity(TId id)
        : base(id)
    {
    }

    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; } = string.Empty;
    public DateTime? UpdatedAt { get; private set; }
    public string? UpdatedBy { get; private set; }

    public void MarkCreated(string by, DateTime at)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(by);
        CreatedBy = by;
        CreatedAt = at;
    }

    public void MarkUpdated(string by, DateTime at)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(by);
        UpdatedBy = by;
        UpdatedAt = at;
    }
}