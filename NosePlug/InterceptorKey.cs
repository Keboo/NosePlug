using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace NosePlug;

internal class InterceptorKey : IEquatable<InterceptorKey?>
{
    private static Dictionary<InterceptorKey, InterceptorKey> Keys { get; } = new();

    private SemaphoreSlim Semaphore { get; } = new(1, 1);

    private string Name { get; }
    private Type? DeclaringType { get; }
    private IReadOnlyList<Type> ParameterTypes { get; }
    // Not possible in C# but IL allows methods to differ by return type
    private Type? ReturnType { get; }

    private InterceptorKey(
        string name,
        Type? declaringType,
        IReadOnlyList<Type> paramtersTypes,
        Type? returnType)
    {
        Name = name;
        DeclaringType = declaringType;
        ParameterTypes = paramtersTypes;
        ReturnType = returnType;
    }
    public static InterceptorKey FromProperty(PropertyInfo property)
    {
        InterceptorKey rv = new(
            property.Name,
            property.DeclaringType,
            new[] { property.PropertyType },
            property.PropertyType
        );

        return GetKey(rv);
    }

    public static InterceptorKey FromMethod(MethodBase method)
    {
        InterceptorKey rv = new(
            method.Name,
            method.DeclaringType,
            method.GetParameters().Select(x => x.ParameterType).ToArray(),
            (method as MethodInfo)?.ReturnType
        );

        return GetKey(rv);
    }

    private static InterceptorKey GetKey(InterceptorKey key)
    {
        lock (Keys)
        {
            if (Keys.TryGetValue(key, out var existing))
            {
                return existing;
            }
            Keys[key] = key;
            return key;
        }
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as InterceptorKey);
    }

    public bool Equals(InterceptorKey? other)
    {
        return other != null &&
               Name == other.Name &&
               EqualityComparer<Type?>.Default.Equals(DeclaringType, other.DeclaringType) &&
               EqualityComparer<Type?>.Default.Equals(ReturnType, other.ReturnType) &&
               ParameterTypes.SequenceEqual(other.ParameterTypes);
    }

    public override int GetHashCode()
    {
        int hashCode = ParameterTypes.Aggregate(42, (cur, x) => HashCode.Combine(cur, x));
        return HashCode.Combine(hashCode, Name, DeclaringType, ReturnType);
    }

    internal async Task LockAsync()
        => await Semaphore.WaitAsync();

    internal void Unlock()
        => Semaphore.Release();
}
