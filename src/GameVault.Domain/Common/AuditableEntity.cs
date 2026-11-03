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

    protected void RestoreAudit(DateTime createdAt, string createdBy, DateTime? updatedAt, string? updatedBy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(createdBy);
        CreatedAt = createdAt;
        CreatedBy = createdBy;
        UpdatedAt = updatedAt;
        UpdatedBy = updatedBy;
    }
}