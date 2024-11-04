using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq.Expressions;

namespace Anemoi.BuildingBlock.Domain.StronglyTypedHelper;

public sealed class StronglyTypedIdConverter(Type stronglyTypedIdType) : TypeConverter
{
    private static readonly ConcurrentDictionary<Type, TypeConverter> ActualConverters = new();

    private readonly TypeConverter _innerConverter = ActualConverters.GetOrAdd(stronglyTypedIdType, CreateActualConverter);

    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) =>
        _innerConverter.CanConvertFrom(context, sourceType);

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) =>
        _innerConverter.CanConvertTo(context, destinationType);

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) =>
        _innerConverter.ConvertFrom(context, culture, value);

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
        Type destinationType) =>
        _innerConverter.ConvertTo(context, culture, value, destinationType);


    private static TypeConverter CreateActualConverter(Type stronglyTypedIdType)
    {
        if (!StronglyTypedIdHelper.IsStronglyTypedId(stronglyTypedIdType, out var idType))
            throw new InvalidOperationException($"The type '{stronglyTypedIdType}' is not a strongly typed id");

        var actualConverterType = typeof(StronglyTypedIdConverter<>).MakeGenericType(idType);
        return (TypeConverter)Activator.CreateInstance(actualConverterType, stronglyTypedIdType)!;
    }
}

public sealed class StronglyTypedIdConverter<TValue>(Type type) : TypeConverter
    where TValue : notnull
{
    private static TypeConverter GetIdValueConverter()
    {
        var converter = TypeDescriptor.GetConverter(typeof(TValue));
        if (!converter.CanConvertFrom(typeof(string)))
            throw new InvalidOperationException(
                $"Type '{typeof(TValue)}' doesn't have a converter that can convert from string");
        return converter;
    }

    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) =>
        sourceType == typeof(string) || sourceType == typeof(TValue) || base.CanConvertFrom(context, sourceType);

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) =>
        destinationType == typeof(string) || destinationType == typeof(TValue) ||
        base.CanConvertTo(context, destinationType);

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string s) value = GetIdValueConverter().ConvertFrom(s);
        if (value is not TValue idValue) return base.ConvertFrom(context, culture, value);
        var factory = StronglyTypedIdHelper.GetFactory<TValue>(type);
        return factory(idValue);
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
        Type destinationType)
    {
        if (value is null) throw new ArgumentNullException(nameof(value));
        var stronglyTypedId = (StronglyTypedId<TValue>)value;
        var idValue = stronglyTypedId.Value;
        if (destinationType == typeof(string)) return idValue.ToString()!;
        return destinationType == typeof(TValue) ? idValue : base.ConvertTo(context, culture, value, destinationType);
    }
}

public static class StronglyTypedIdHelper
{
    private static readonly ConcurrentDictionary<Type, Delegate> StronglyTypedIdFactories = new();

    public static Func<TValue, object> GetFactory<TValue>(Type stronglyTypedIdType)
        where TValue : notnull =>
        (Func<TValue, object>)StronglyTypedIdFactories.GetOrAdd(stronglyTypedIdType, CreateFactory<TValue>);

    private static Func<TValue, object> CreateFactory<TValue>(Type stronglyTypedIdType)
        where TValue : notnull
    {
        if (!IsStronglyTypedId(stronglyTypedIdType))
            throw new ArgumentException($"Type '{stronglyTypedIdType}' is not a strongly-typed id type",
                nameof(stronglyTypedIdType));

        var ctor = stronglyTypedIdType.GetConstructor([typeof(TValue)]);
        if (ctor is null)
            throw new ArgumentException(
                $"Type '{stronglyTypedIdType}' doesn't have a constructor with one parameter of type '{typeof(TValue)}'",
                nameof(stronglyTypedIdType));

        var param = Expression.Parameter(typeof(TValue), "value");
        var body = Expression.New(ctor, param);
        var lambda = Expression.Lambda<Func<TValue, object>>(body, param);
        return lambda.Compile();
    }

    private static bool IsStronglyTypedId(Type type) => IsStronglyTypedId(type, out _);

    public static bool IsStronglyTypedId(Type type, [NotNullWhen(true)] out Type idType)
    {
        if (type is null) throw new ArgumentNullException(nameof(type));

        if (type.BaseType is { IsGenericType: true } baseType &&
            baseType.GetGenericTypeDefinition() == typeof(StronglyTypedId<>))
        {
            idType = baseType.GetGenericArguments()[0];
            return true;
        }

        idType = null;
        return false;
    }
}