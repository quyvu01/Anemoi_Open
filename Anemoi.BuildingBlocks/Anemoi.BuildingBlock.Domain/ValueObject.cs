using System.Collections.Generic;
using System.Linq;

namespace Anemoi.BuildingBlock.Domain;

public abstract class ValueObject
{
    protected abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object obj)
    {
        if (obj == null || obj.GetType() != GetType()) return false;
        var other = (ValueObject)obj;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode() => !GetEqualityComponents().Select(GetHashCode).Any()
        ? 0
        : GetEqualityComponents().Select(GetHashCode).Aggregate((x, y) => x ^ y);

    private static int GetHashCode(object obj) => obj?.GetHashCode() ?? 0;
}