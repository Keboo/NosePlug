using System;

namespace NosePlug
{
    public static class INasalPlugExtensions
    {
        public static INasalMethodPlug Callback<T1, T2>(this INasalMethodPlug plug, Action<T1, T2> action)
        {
            if (plug is null)
            {
                throw new ArgumentNullException(nameof(plug));
            }

            return plug.Callback(args => action((T1)args[0], (T2)args[1]));
        }

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
