using GameVault.Domain.Clients;
using GameVault.Domain.Common;
using GameVault.Domain.Games;

namespace GameVault.Domain.Orders;

public sealed class Order : AuditableEntity<OrderId>
{
    private readonly List<OrderItem> _items;
    private readonly IReadOnlyList<OrderItem> _itemsView;

    private Order(OrderId id, ClientId clientId, OrderStatus status, IEnumerable<OrderItem>? items = null)
        : base(id)
    {
        ClientId = clientId;
        Status = status;
        _items = items?.ToList() ?? [];
        _itemsView = _items.AsReadOnly();
    }

    public ClientId ClientId { get; }
    public OrderStatus Status { get; private set; }
    public IReadOnlyList<OrderItem> Items => _itemsView;
    public decimal Total => _items.Sum(item => item.UnitPrice.Amount);

    public static Result<Order> Create(ClientId clientId) =>
        Result.Success(new Order(OrderId.New(), clientId, OrderStatus.Pending));

    public static Order Reconstruct(
        OrderId id,
        ClientId clientId,
        OrderStatus status,
        IReadOnlyList<OrderItem> items,
        DateTime createdAt,
        string createdBy,
        DateTime? updatedAt,
        string? updatedBy)
    {
        var order = new Order(id, clientId, status, items);
        order.RestoreAudit(createdAt, createdBy, updatedAt, updatedBy);
        return order;
    }

    public Result AddItem(Game game)
    {
        if (game.Status != GameStatus.Published)
            return Result.Failure(OrderErrors.GameNotAvailable(game.Title));

        if (_items.Any(item => item.GameId == game.Id))
            return Result.Failure(OrderErrors.DuplicateGame(game.Title));

        if (Status != OrderStatus.Pending)
            return Result.Failure(OrderErrors.NotModifiable);

        _items.Add(OrderItem.Create(game.Id, game.Price));
        return Result.Success();
    }

    public Result Cancel()
    {
        if (Status != OrderStatus.Pending)
            return Result.Failure(OrderErrors.CannotCancel);

        Status = OrderStatus.Cancelled;
        return Result.Success();
    }

    public Result MarkAsPaid()
    {
        if (Status != OrderStatus.Pending || _items.Count == 0)
            return Result.Failure(OrderErrors.CannotPay);

        Status = OrderStatus.Paid;
        return Result.Success();
    }
}