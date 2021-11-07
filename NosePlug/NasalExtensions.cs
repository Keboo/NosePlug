using System;
using System.Linq.Expressions;
using System.Reflection;

namespace NosePlug
{
    public static class NasalExtensions
    {
        public static INasalPropertyPlug<TProperty> Property<T, TProperty>(this Nasal nasal, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
            }

            var property = typeof(T).GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.GetProperty)
                ?? throw new ArgumentException($"Could not find property '{name}' on type '{typeof(T).FullName}'");
            return nasal.Property<TProperty>(property);
        }

        public static INasalPropertyPlug<TProperty> Property<TProperty>(this Nasal nasal, 
            Expression<Func<TProperty>> propertyExpression)
        {
            if (propertyExpression is null)
            {
                throw new ArgumentNullException(nameof(propertyExpression));
            }
            if (!(propertyExpression.Body is MemberExpression memberExpression) ||
                !(memberExpression.Member is PropertyInfo propertyInfo))
            {
                throw new ArgumentException("Expresion is not a member expression to a property");
            }

            return nasal.Property<TProperty>(propertyInfo);
        }


        public static INasalPlug Method(this Nasal nasal, Expression<Action> methodExpression)
        {
            var methodCallExpression = methodExpression.Body as MethodCallExpression;
            if (methodCallExpression is null)
            {
                throw new ArgumentException();
            }
            MethodInfo origianl = methodCallExpression.Method;
            return nasal.GetPlug(origianl);
        }
    }
}
