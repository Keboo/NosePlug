using System;

namespace NosePlug
{
    public static class INasalPlugExtensions
    {
        public static INasalMethodPlug Returns<TReturn>(this INasalMethodPlug plug, TReturn returnValue)
        {
            if (plug is null)
            {
                throw new ArgumentNullException(nameof(plug));
            }

            return plug.Returns(() => returnValue);
        }

        public static INasalPropertyPlug<TProperty> Returns<TProperty>(this INasalPropertyPlug<TProperty> plug, TProperty returnValue)
        {
            if (plug is null)
            {
                throw new ArgumentNullException(nameof(plug));
            }

            return plug.Returns(() => returnValue);
        }
    }
}
