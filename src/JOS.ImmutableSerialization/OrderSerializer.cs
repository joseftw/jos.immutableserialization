using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace JOS.ImmutableSerialization;

public static class OrderSerializer
{
    private static readonly JsonSerializerOptions JsonSerializerOptions;

    static OrderSerializer()
    {
        JsonSerializerOptions = new JsonSerializerOptions
        {
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            TypeInfoResolver = new DefaultJsonTypeInfoResolver
            {
                Modifiers = { OrderJsonTypeInfoModifier, OrderLineJsonTypeInfoModifier }
            }
        };
    }

    public static string Serialize(Order order)
    {
        return JsonSerializer.Serialize(order, JsonSerializerOptions);
    }

    public static Result<Order> Deserialize(string orderJson)
    {
        try
        {
            var result = JsonSerializer.Deserialize<Order>(orderJson, JsonSerializerOptions);
            return result is not null
                ? Result.Success(result)
                : Result.Failure<Order>(
                    new Error(ErrorType.Deserialization, "Order was null after deserialization"));
        }
        catch(Exception e)
        {
            return Result.Failure<Order>(new Error(ErrorType.Deserialization, e.Message));
        }
    }

    private static void OrderJsonTypeInfoModifier(JsonTypeInfo jsonTypeInfo)
    {
        if(jsonTypeInfo.Type != typeof(Order))
        {
            return;
        }

        foreach(var property in jsonTypeInfo.Type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic))
        {
            var jsonPropertyInfo =
                jsonTypeInfo.CreateJsonPropertyInfo(property.PropertyType, property.Name.ToCamelCase());
            jsonPropertyInfo.Get = property.GetValue;
            jsonPropertyInfo.Set = property.SetValue;
            jsonTypeInfo.Properties.Add(jsonPropertyInfo);
        }

        CreateFactoryMethodForObject(jsonTypeInfo);
    }

    private static void OrderLineJsonTypeInfoModifier(JsonTypeInfo jsonTypeInfo)
    {
        if(jsonTypeInfo.Type != typeof(OrderLine))
        {
            return;
        }

        CreateFactoryMethodForObject(jsonTypeInfo);
    }

    private static void CreateFactoryMethodForObject(JsonTypeInfo typeInfo)
    {
        var constructor = typeInfo.Type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, []);
        typeInfo.CreateObject = () => constructor!.Invoke(null);
    }
}
