using System;
using System.Threading.Tasks;

namespace NosePlug.Tests.TestClasses
{
    internal class HasPrivateMethod
    {
        private static void NoParameters() { }
        private static void NoParametersOverload(int value) { }

        private static void GuidParameter(Guid value) { }

        private static async Task AsyncMethod() { }

        private static T GenericMethod<T>() { return default; }
    }
}
