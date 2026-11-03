using GameVault.Domain.Common;

namespace GameVault.Domain.Clients;

public sealed class Client : AuditableEntity<ClientId>
{
    private Client(ClientId id, string name, Email email, bool active)
        : base(id)
    {
        Name = name;
        Email = email;
        Active = active;
    }

    public string Name { get; private set; }
    public Email Email { get; private set; }
    public bool Active { get; private set; }

    public static Result<Client> Create(string name, string email)
    {
        var nameError = ValidateName(name);
        if (nameError is not null)
            return Result.Failure<Client>(nameError);

        var emailResult = Email.Create(email);
        if (emailResult.IsFailure)
            return Result.Failure<Client>(emailResult.Error);

        return Result.Success(new Client(ClientId.New(), name.Trim(), emailResult.Value, true));
    }

    public static Client Reconstruct(
        ClientId id,
        string name,
        string email,
        bool active,
        DateTime createdAt,
        string createdBy,
        DateTime? updatedAt,
        string? updatedBy)
    {
        var client = new Client(id, name, Email.Rehydrate(email), active);
        client.RestoreAudit(createdAt, createdBy, updatedAt, updatedBy);
        return client;
    }

    public Result UpdateName(string name)
    {
        var nameError = ValidateName(name);
        if (nameError is not null)
            return Result.Failure(nameError);

        Name = name.Trim();
        return Result.Success();
    }

    public Result UpdateEmail(string email)
    {
        var emailResult = Email.Create(email);
        if (emailResult.IsFailure)
            return Result.Failure(emailResult.Error);

        Email = emailResult.Value;
        return Result.Success();
    }

    public Result Deactivate()
    {
        if (!Active)
            return Result.Failure(ClientErrors.AlreadyInactive);

        Active = false;
        return Result.Success();
    }

    public Result Activate()
    {
        if (Active)
            return Result.Failure(ClientErrors.AlreadyActive);

        Active = true;
        return Result.Success();
    }

    private static Error? ValidateName(string name) =>
        string.IsNullOrWhiteSpace(name) || name.Trim().Length is < 2 or > 100
            ? ClientErrors.InvalidName
            : null;
}