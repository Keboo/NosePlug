using System;
using System.Threading.Tasks;

namespace NosePlug.Tests.TestClasses
{
    internal class HasPublicMethod
    {
        public static void NoParameters() { }
        public static void NoParametersOverload(int value) { }

        public static void GuidParameter(Guid value) { }

        public static async Task AsyncMethod() { }

        public static T GenericMethod<T>() { return default; }
    }
}
