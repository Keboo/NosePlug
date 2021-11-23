using HarmonyLib;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace NosePlug
{
    public static class NasalExtensions
    {
        public static INasalPropertyPlug<TProperty> Property<T, TProperty>(this Nasal nasal, string name)
        {
            if (nasal is null)
            {
                throw new ArgumentNullException(nameof(nasal));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
            }

            var property = typeof(T).GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.GetProperty)
                ?? throw new MissingMemberException($"Could not find property '{name}' on type '{typeof(T).FullName}'");
            return nasal.Property<TProperty>(property);
        }

        public static INasalPropertyPlug<TProperty> Property<TProperty>(this Nasal nasal, 
            Expression<Func<TProperty>> propertyExpression)
        {
            if (nasal is null)
            {
                throw new ArgumentNullException(nameof(nasal));
            }

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

        public static INasalPropertyPlug<object> Property(this Nasal nasal, PropertyInfo property)
        {
            if (nasal is null)
            {
                throw new ArgumentNullException(nameof(nasal));
            }

            return nasal.Property<object>(property);
        }

        public static INasalMethodPlug Method(this Nasal nasal, Expression<Action> methodExpression)
        {
            if (nasal is null)
            {
                throw new ArgumentNullException(nameof(nasal));
            }

            if (methodExpression is null)
            {
                throw new ArgumentNullException(nameof(methodExpression));
            }

            if (methodExpression.Body is MethodCallExpression methodCallExpression)
            {
                MethodInfo original = methodCallExpression.Method;
                return nasal.Method(original);
            }
            throw new ArgumentException("Expresion is not a method call expression");
        }

        public static INasalMethodPlug Method<TReturn>(this Nasal nasal, Expression<Func<TReturn>> methodExpression)
        {
            if (nasal is null)
            {
                throw new ArgumentNullException(nameof(nasal));
            }

            if (methodExpression is null)
            {
                throw new ArgumentNullException(nameof(methodExpression));
            }

            if (methodExpression.Body is MethodCallExpression methodCallExpression)
            {
                MethodInfo original = methodCallExpression.Method;
                return nasal.Method(original);
            }
            throw new ArgumentException("Expresion is not a method call expression", nameof(methodExpression));
        }

        public static INasalMethodPlug Method<T>(this Nasal nasal, string methodName, params Type[] parameterTypes)
            => Method<T>(nasal, methodName, null, parameterTypes);

        public static INasalMethodPlug Method<T>(this Nasal nasal, string methodName, Type[]? genericTypeParameters, Type[] parameterTypes)
        {
            if (nasal is null)
            {
                throw new ArgumentNullException(nameof(nasal));
            }

            if (string.IsNullOrWhiteSpace(methodName))
            {
                throw new ArgumentException($"'{nameof(methodName)}' cannot be null or whitespace.", nameof(methodName));
            }

            var methods = AccessTools.GetDeclaredMethods(typeof(T))
                .Where(x => string.Equals(x.Name, methodName))
                .ToList();

            MethodInfo method = methods.Count switch
            {
                0 => throw new MissingMethodException(typeof(T).FullName, methodName),
                1 => methods[0],
                _ => methods.FirstOrDefault(x => x.GetParameters().Select(p => p.ParameterType).SequenceEqual(parameterTypes))
                        ?? throw new MissingMethodException($"Could not find method '{methodName}' on '{typeof(T).FullName}' with parameter type(s) {string.Join(", ", parameterTypes.Select(x => x.FullName))}")
            };

            if (genericTypeParameters is not null)
            {
                method = method.MakeGenericMethod(genericTypeParameters);
            }

            return nasal.Method(method);
        }
    }
}
