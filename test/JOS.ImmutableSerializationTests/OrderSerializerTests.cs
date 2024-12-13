using JOS.ImmutableSerialization;
using Shouldly;
using System;
using System.Linq;
using System.Text.Json;
using Xunit;

namespace JOS.ImmutableSerializationTests;

public class OrderSerializerTests
{
    [Fact]
    public void CanSerializeAnOrder()
    {
        var order = Order.Create(Guid.NewGuid()).Data;
        var orderLine = OrderLine.Create(Guid.NewGuid(), "iPhone 16 Pro", 14990).Data;
        order.AddOrderLine(orderLine);

        var serialized = OrderSerializer.Serialize(order);

        serialized.ShouldNotBeNull();
        var jsonDocument = JsonDocument.Parse(serialized);
        jsonDocument.RootElement.TryGetProperty("id", out var idProperty).ShouldBeTrue();
        idProperty.GetGuid().ShouldBe(order.Id);
        jsonDocument.RootElement.TryGetProperty("created", out var createdProperty).ShouldBeTrue();
        createdProperty.GetDateTimeOffset().ShouldBe(order.Created);
        jsonDocument.RootElement.TryGetProperty("lines", out var linesProperty).ShouldBeTrue();
        var lines = linesProperty.EnumerateArray().ToList();
        lines.Count.ShouldBe(1);
    }

    [Fact]
    public void CanDeserializeASerializedOrder()
    {
        var order = Order.Create(Guid.NewGuid()).Data;
        var orderLine = OrderLine.Create(Guid.NewGuid(), "iPhone 16 Pro", 14990).Data;
        order.AddOrderLine(orderLine);
        var serialized = OrderSerializer.Serialize(order);

        var result = OrderSerializer.Deserialize(serialized);

        result.Succeeded.ShouldBeTrue(result.Error?.ErrorMessage);
        result.Data.Id.ShouldBe(order.Id);
        result.Data.Created.ShouldBe(order.Created);
        var orderLines = result.Data.GetLines();
        orderLines.Count.ShouldBe(1);
        orderLines.ShouldContain(orderLine);
    }
}
