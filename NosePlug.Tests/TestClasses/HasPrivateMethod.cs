using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace NosePlug.Tests.TestClasses
{
    internal class HasPrivateMethod
    {
        private static void NoParameters() { }
        public static void InvokeNoParameters() => NoParameters();

        public static void InvokeOverloaded(int value) => Overloaded(value);
        private static void Overloaded(int value) { }
        public static void InvokeOverloaded(string @string, int value) => Overloaded(@string, value);
        private static void Overloaded(string @string, int value) { }

        private static int ReturnValue() => 42;
        public static int InvokeReturnValue() => ReturnValue();

        private static void GuidParameter(Guid value) { }

        public static async Task InvokeAsyncMethod() => await AsyncMethod();
        public static async Task<int> InvokeAsyncMethodWithReturn() => await AsyncMethodWithReturn();
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private static async Task AsyncMethod() { }
        private static async Task<int> AsyncMethodWithReturn() => 42;
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

        [return: MaybeNull]
        public static T InvokeGenericMethod<T>() => GenericMethod<T>();

        [return: MaybeNull]
        private static T GenericMethod<T>() { return default; }
    }
}
