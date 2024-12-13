using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace JOS.ImmutableSerialization;

public class Order
{
    private HashSet<OrderLine> Lines { get; init; } = [];

    private Order() { }
    public required Guid Id { get; init; }
    public required DateTimeOffset Created { get; init; }

    public IReadOnlySet<OrderLine> GetLines()
    {
        return Lines;
    }

    public void AddOrderLine(OrderLine orderLine)
    {
        Lines.Add(orderLine);
    }

    public static Result<Order> Create(Guid id)
    {
        if(id.Equals(Guid.Empty))
        {
            return Result.Failure<Order>(
                new Error(ErrorType.NullOrEmpty, $"{nameof(id)} was null or empty"));
        }

        return Result.Success(new Order { Id = id, Created = TimeProvider.System.GetUtcNow() });
    }
}

public record OrderLine
{
    private OrderLine() { }

    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required decimal Price { get; init; }

    public static Result<OrderLine> Create(Guid id, string name, decimal price)
    {
        if(id.Equals(Guid.Empty))
        {
            return Result.Failure<OrderLine>(
                new Error(ErrorType.NullOrEmpty, $"{nameof(id)} was null or empty"));
        }

        if(string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<OrderLine>(
                new Error(ErrorType.NullOrEmpty, $"{nameof(name)} was null or empty"));
        }

        if(price < 1)
        {
            return Result.Failure<OrderLine>(
                new Error(ErrorType.MinimumValue, $"{nameof(price)} needs to be greater than 0"));
        }

        return Result.Success(new OrderLine { Id = id, Name = name, Price = price });
    }
}
