using System.Runtime.CompilerServices;

namespace NosePlug.Tests.TestClasses
{
    internal class HasInstanceMethods
    {
        public void VoidMethod()
        {
        }

        public void VoidMethodWithParameter(string myParameter)
        {

        }

        public int GetIntegerValue() => 42;

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void NoParameters() { }
        public void InvokeNoParameters() => NoParameters();
    }
}
