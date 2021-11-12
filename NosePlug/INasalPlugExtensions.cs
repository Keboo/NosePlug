using System;

namespace NosePlug
{
    public static class INasalPlugExtensions
    {
        public static INasalMethodPlug ReturnsValue<TReturn>(this INasalMethodPlug plug, TReturn returnValue)
        {
            if (plug is null)
            {
                throw new ArgumentNullException(nameof(plug));
            }
            return plug.Returns(() => returnValue);
        }

        public static INasalPropertyPlug<TProperty> ReturnsValue<TProperty>(this INasalPropertyPlug<TProperty> plug, TProperty returnValue)
        {
            if (plug is null)
            {
                throw new ArgumentNullException(nameof(plug));
            }

            return plug.Returns(() => returnValue);
        }
    }
}
