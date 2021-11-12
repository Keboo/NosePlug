using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace NosePlug.Tests.TestClasses
{
    internal class HasPublicMethod
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void NoParameters() { }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Overloaded(int value) { }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Overloaded(string @string, int value) { }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static int ReturnValue() => 42;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static async Task AsyncMethod() { }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static async Task<int> AsyncMethodWithReturn() => 42;
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static async Task AsyncMethod(int value) { }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static async Task<double> AsyncMethod(string @string, int value) => 42.0;
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

        [MethodImpl(MethodImplOptions.NoInlining)]
        [return:MaybeNull]
        public static T GenericMethod<T>() { return default; }
    }
}
