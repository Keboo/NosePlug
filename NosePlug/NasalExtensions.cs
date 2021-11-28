using HarmonyLib;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NosePlug;

/// <summary>
/// Contains convenience extension methods for the <see cref="Nasal"/> class.
/// </summary>
public static class NasalExtensions
{
    /// <summary>
    /// Creates a plug for a property
    /// </summary>
    /// <typeparam name="T">The type that contains the property</typeparam>
    /// <typeparam name="TProperty">The type of the property</typeparam>
    /// <param name="nasal">The <see cref="Nasal"/> instance to use to create the property plug</param>
    /// <param name="name">The name of the property</param>
    /// <returns>A new property plug</returns>
    /// <exception cref="ArgumentNullException">Thrown when the passed <see cref="Nasal"/> instance is <c>null</c></exception>
    /// <exception cref="ArgumentException">Thrown when the passed string for the property name is <c>null</c> or whitespace</exception>
    /// <exception cref="MissingMemberException">Thrown when the propery could not be found</exception>
    public static IPropertyPlug<TProperty> Property<T, TProperty>(this Nasal nasal, string name)
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

    /// <summary>
    /// Creates a plug for a property
    /// </summary>
    /// <typeparam name="TProperty">The type of the property</typeparam>
    /// <param name="nasal">The <see cref="Nasal"/> instance to use to create the property plug</param>
    /// <param name="propertyExpression">An expression representing the property to create a plug for</param>
    /// <returns>A new property plug</returns>
    /// <exception cref="ArgumentNullException">Thrown when the passed <see cref="Nasal"/> instance is <c>null</c></exception>
    /// <exception cref="ArgumentException">Thrown when the passed expression is not a property expression</exception>
    public static IPropertyPlug<TProperty> Property<TProperty>(this Nasal nasal,
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

        if (propertyExpression.Body is not MemberExpression memberExpression ||
            memberExpression.Member is not PropertyInfo propertyInfo)
        {
            throw new ArgumentException("Expresion is not a member expression to a property");
        }

        return nasal.Property<TProperty>(propertyInfo);
    }

    public static IMethodPlug Method(this Nasal nasal, Expression<Action> methodExpression)
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

    public static IMethodPlug<TReturn> Method<TReturn>(this Nasal nasal, Expression<Func<TReturn>> methodExpression)
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
            return nasal.Method<TReturn>(original);
        }
        throw new ArgumentException("Expresion is not a method call expression", nameof(methodExpression));
    }

    public static IMethodPlug Method<TContainingType>(this Nasal nasal, string methodName, params Type[] parameterTypes)
        => Method<TContainingType>(nasal, methodName, null, parameterTypes);

    public static IMethodPlug<TReturn> Method<TContainingType, TReturn>(this Nasal nasal, string methodName, params Type[] parameterTypes)
        => Method<TContainingType, TReturn>(nasal, methodName, null, parameterTypes);

    public static IMethodPlug Method<TContainingType>(this Nasal nasal, string methodName, Type[]? genericTypeParameters, Type[] parameterTypes)
    {
        if (nasal is null)
        {
            throw new ArgumentNullException(nameof(nasal));
        }

        if (string.IsNullOrWhiteSpace(methodName))
        {
            throw new ArgumentException($"'{nameof(methodName)}' cannot be null or whitespace.", nameof(methodName));
        }

        var methods = AccessTools.GetDeclaredMethods(typeof(TContainingType))
            .Where(x => string.Equals(x.Name, methodName))
            .ToList();

        MethodInfo method = methods.Count switch
        {
            0 => throw new MissingMethodException(typeof(TContainingType).FullName, methodName),
            1 => methods[0],
            _ => methods.FirstOrDefault(x => x.GetParameters().Select(p => p.ParameterType).SequenceEqual(parameterTypes))
                    ?? throw new MissingMethodException($"Could not find method '{methodName}' on '{typeof(TContainingType).FullName}' with parameter type(s) {string.Join(", ", parameterTypes.Select(x => x.FullName))}")
        };

        if (genericTypeParameters is not null)
        {
            method = method.MakeGenericMethod(genericTypeParameters);
        }

        return nasal.Method(method);
    }

    public static IMethodPlug<TReturn> Method<TContainingType, TReturn>(this Nasal nasal, string methodName, Type[]? genericTypeParameters, Type[] parameterTypes)
    {
        if (nasal is null)
        {
            throw new ArgumentNullException(nameof(nasal));
        }

        if (string.IsNullOrWhiteSpace(methodName))
        {
            throw new ArgumentException($"'{nameof(methodName)}' cannot be null or whitespace.", nameof(methodName));
        }

        var methods = AccessTools.GetDeclaredMethods(typeof(TContainingType))
            .Where(x => string.Equals(x.Name, methodName))
            .ToList();

        MethodInfo method = methods.Count switch
        {
            0 => throw new MissingMethodException(typeof(TContainingType).FullName, methodName),
            1 => methods[0],
            _ => methods.FirstOrDefault(x => x.GetParameters().Select(p => p.ParameterType).SequenceEqual(parameterTypes))
                    ?? throw new MissingMethodException($"Could not find method '{methodName}' on '{typeof(TContainingType).FullName}' with parameter type(s) {string.Join(", ", parameterTypes.Select(x => x.FullName))}")
        };

        if (genericTypeParameters is not null)
        {
            method = method.MakeGenericMethod(genericTypeParameters);
        }

        return nasal.Method<TReturn>(method);
    }
}
