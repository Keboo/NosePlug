using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace NosePlug
{
    internal class InterceptorKey : IEquatable<InterceptorKey?>
    {
        private static Dictionary<InterceptorKey, InterceptorKey> Keys { get; } = new();


        private SemaphoreSlim Semaphore { get; } = new(1, 1);

        public string Name { get; }
        public Type? DelcaringType { get; }
        public IReadOnlyList<Type> ParameterTypes { get; }
        // Not possible in C# but IL allows methods to differ by return type
        public Type? ReturnType { get; }

        private InterceptorKey(
            string name,
            Type? declaringType,
            IReadOnlyList<Type> paramtersTypes,
            Type? returnType)
        {
            Name = name;
            DelcaringType = declaringType;
            ParameterTypes = paramtersTypes;
            ReturnType = returnType;
        }

        public static InterceptorKey FromMethod(MethodBase method)
        {
            InterceptorKey rv = new(
                method.Name,
                method.DeclaringType,
                method.GetParameters().Select(x => x.ParameterType).ToArray(),
                (method as MethodInfo)?.ReturnType
            );

            lock (Keys)
            {
                if (Keys.TryGetValue(rv, out var existing))
                {
                    return existing;
                }
                Keys[rv] = rv;
                return rv;
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
                   EqualityComparer<Type?>.Default.Equals(DelcaringType, other.DelcaringType) &&
                   EqualityComparer<Type?>.Default.Equals(ReturnType, other.ReturnType) &&
                   ParameterTypes.SequenceEqual(other.ParameterTypes);
        }

        public override int GetHashCode()
        {
            int hashCode = ParameterTypes.Aggregate(42, (cur, x) => HashCode.Combine(cur, x));
            return HashCode.Combine(hashCode, Name, DelcaringType, ReturnType);
        }

        internal async Task LockAsync()
            => await Semaphore.WaitAsync();

        internal void Unlock()
            => Semaphore.Release();
    }
}
