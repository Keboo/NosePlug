using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace NosePlug.Tests.TestClasses
{
    internal class HasPrivateMethod
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void NoParameters() { }
        public static void InvokeNoParameters() => NoParameters();

        public static void InvokeOverloaded(int value) => Overloaded(value);
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void Overloaded(int value) { }

        public static void InvokeOverloaded(string @string, int value) => Overloaded(@string, value);
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void Overloaded(string @string, int value) { }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static int ReturnValue() => 42;
        public static int InvokeReturnValue() => ReturnValue();

        public static async Task InvokeAsyncMethod() => await AsyncMethod();
        public static async Task InvokeAsyncMethod(int value) => await AsyncMethod(value);
        public static async Task<double> InvokeAsyncMethod(string @string, int value) => await AsyncMethod(@string, value);
        public static async Task<int> InvokeAsyncMethodWithReturn() => await AsyncMethodWithReturn();
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static async Task AsyncMethod() { }
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static async Task<int> AsyncMethodWithReturn() => 42;
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static async Task AsyncMethod(int value) { }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static async Task<double> AsyncMethod(string @string, int value) => 42.0;
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

        [MethodImpl(MethodImplOptions.NoInlining)]
        [return: MaybeNull]
        public static T InvokeGenericMethod<T>() => GenericMethod<T>();

        [MethodImpl(MethodImplOptions.NoInlining)]
        [return: MaybeNull]
        private static T GenericMethod<T>() { return default; }
    }
}
