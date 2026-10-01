// GameVault.Domain/Games/Money.cs
using GameVault.Domain.Common;

namespace GameVault.Domain.Games;

public sealed record Money
{
    public decimal Amount { get; }

    private Money(decimal amount) => Amount = amount;

    public static Result<Money> Create(decimal amount)
    {
        if (amount < 0)
            return Result.Failure<Money>(GameErrors.NegativePrice);

        if (amount > 500)
            return Result.Failure<Money>(GameErrors.PriceExceedsMax);

        return Result.Success(new Money(amount));
    }
}