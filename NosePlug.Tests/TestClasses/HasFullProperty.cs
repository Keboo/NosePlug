namespace NosePlug.Tests.TestClasses
{

    internal static class HasFullProperty
    {
        public static int _field;

        public static int Property
        {
            get => _field;
            set => _field = value;
        }
    }
}
