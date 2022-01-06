using HarmonyLib;
using NosePlug.Plugs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace NosePlug;

/// <summary>
/// The main entry point for creating plugs.
/// </summary>
public static class Nasal
{
    /// <summary>
    /// Creates a plug for a property.
    /// </summary>
    /// <typeparam name="TProperty">The return type of the property</typeparam>
    /// <param name="property">The <see cref="PropertyInfo"/> to create a plug for.</param>
    /// <returns>A new property plug</returns>
    /// <exception cref="ArgumentNullException">When the passed in <see cref="PropertyInfo"/> is <c>null</c></exception>
    public static IPropertyPlug<TProperty> Property<TProperty>(PropertyInfo property)
    {
        if (property is null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        return new PropertyPlug<TProperty>(property);
    }

    /// <summary>
    /// Creates a plug for a property
    /// </summary>
    /// <typeparam name="T">The type that contains the property</typeparam>
    /// <typeparam name="TProperty">The type of the property</typeparam>
    /// <param name="name">The name of the property</param>
    /// <returns>A new property plug</returns>
    /// <exception cref="ArgumentNullException">Thrown when the passed <see cref="Nasal"/> instance is <c>null</c></exception>
    /// <exception cref="ArgumentException">Thrown when the passed string for the property name is <c>null</c> or whitespace</exception>
    /// <exception cref="MissingMemberException">Thrown when the propery could not be found</exception>
    public static IPropertyPlug<TProperty> Property<T, TProperty>(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
        }

        var property = typeof(T).GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.GetProperty)
            ?? throw new MissingMemberException($"Could not find property '{name}' on type '{typeof(T).FullName}'");
        return Property<TProperty>(property);
    }

    /// <summary>
    /// Creates a plug for a property
    /// </summary>
    /// <typeparam name="TProperty">The type of the property</typeparam>
    /// <param name="propertyExpression">An expression representing the property to create a plug for</param>
    /// <returns>A new property plug</returns>
    /// <exception cref="ArgumentNullException">Thrown when the passed <see cref="Nasal"/> instance is <c>null</c></exception>
    /// <exception cref="ArgumentException">Thrown when the passed expression is not a property expression</exception>
    public static IPropertyPlug<TProperty> Property<TProperty>(Expression<Func<TProperty>> propertyExpression)
    {
        if (propertyExpression is null)
        {
            throw new ArgumentNullException(nameof(propertyExpression));
        }

        if (propertyExpression.Body is not MemberExpression memberExpression ||
            memberExpression.Member is not PropertyInfo propertyInfo)
        {
            throw new ArgumentException("Expresion is not a member expression to a property");
        }

        return Property<TProperty>(propertyInfo);
    }

    /// <summary>
    /// Create a method plug for a void returning method
    /// </summary>
    /// <param name="method">The <see cref="MethodInfo"/> to create a plug for</param>
    /// <returns>A new method plug</returns>
    /// <exception cref="ArgumentNullException">When the passed in <see cref="MethodInfo"/> is <c>null</c></exception>
    public static IMethodPlug Method(MethodInfo method)
    {
        if (method is null)
        {
            throw new ArgumentNullException(nameof(method));
        }

        return new MethodPlug(method);
    }

    /// <summary>
    /// Create a method plug for a method with a return value
    /// </summary>
    /// <typeparam name="TReturn">The return type of the method</typeparam>
    /// <param name="methodInfo">The <see cref="MethodInfo"/> to create a plug for</param>
    /// <returns>A new method plug</returns>
    /// <exception cref="ArgumentNullException">When the passed in <see cref="MethodInfo"/> is <c>null</c></exception>
    public static IMethodPlug<TReturn> Method<TReturn>(MethodInfo methodInfo)
    {
        if (methodInfo is null)
        {
            throw new ArgumentNullException(nameof(methodInfo));
        }

        return new MethodPlug<TReturn>(methodInfo);
    }

    /// <summary>
    /// Create a method plug for a void returning method
    /// </summary>
    /// <param name="methodExpression">An expression referencing a method. The paramters passed in the expression, are ignored.</param>
    /// <returns>A new method plug.</returns>
    /// <exception cref="ArgumentNullException">When the passed in <see cref="Expression&lt;Action&gt;"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">When the passed in <see cref="Expression&lt;Action&gt;"/> is not a <see cref="MethodCallExpression"/>.</exception>
    public static IMethodPlug Method(Expression<Action> methodExpression)
    {
        if (methodExpression is null)
        {
            throw new ArgumentNullException(nameof(methodExpression));
        }

        if (methodExpression.Body is MethodCallExpression methodCallExpression)
        {
            MethodInfo original = methodCallExpression.Method;
            return Method(original);
        }
        throw new ArgumentException("Expresion is not a method call expression");
    }

    /// <summary>
    /// Create a method plug for a method with a return value
    /// </summary>
    /// <typeparam name="TReturn">The return type of the method</typeparam>
    /// <param name="methodExpression">An expression referencing a method with a return value. The paramters passed in the expression, are ignored.</param>
    /// <returns>A new method plug.</returns>
    /// <exception cref="ArgumentNullException">When the passed in expression is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">When the passed in expression is not a <see cref="MethodCallExpression"/>.</exception>
    public static IMethodPlug<TReturn> Method<TReturn>(Expression<Func<TReturn>> methodExpression)
    {
        if (methodExpression is null)
        {
            throw new ArgumentNullException(nameof(methodExpression));
        }

        if (methodExpression.Body is MethodCallExpression methodCallExpression)
        {
            MethodInfo original = methodCallExpression.Method;
            return Method<TReturn>(original);
        }
        throw new ArgumentException("Expresion is not a method call expression", nameof(methodExpression));
    }

    /// <summary>
    /// Create a method plug for a method given its name and parameters.
    /// </summary>
    /// <param name="methodName">The name of the method.</param>
    /// <param name="parameterTypes">A collection of types matching the parameters for the method.</param>
    /// <returns>A new method plug.</returns>
    /// <exception cref="ArgumentException">When the method name is null or whitespace.</exception>
    public static IMethodPlug Method<TContainingType>(string methodName, params Type[] parameterTypes)
        => Method<TContainingType>(methodName, null, parameterTypes);

    /// <summary>
    /// Create a method plug for a method with a return value given its name and parameters.
    /// </summary>
    /// <param name="methodName">The name of the method.</param>
    /// <param name="parameterTypes">A collection of types matching the parameters for the method.</param>
    /// <returns>A new method plug.</returns>
    /// <exception cref="ArgumentException">When the method name is null or whitespace.</exception>
    public static IMethodPlug<TReturn> Method<TContainingType, TReturn>(string methodName, params Type[] parameterTypes)
        => Method<TContainingType, TReturn>(methodName, null, parameterTypes);

    /// <summary>
    /// Create a method plug for a method given its name and parameters.
    /// </summary>
    /// <param name="methodName">The name of the method.</param>
    /// <param name="genericTypeParameters">The generic type parameters for the method.</param>
    /// <param name="parameterTypes">A collection of types matching the parameters for the method.</param>
    /// <returns>A new method plug.</returns>
    /// <exception cref="ArgumentException">When the method name is null or whitespace.</exception>
    public static IMethodPlug Method<TContainingType>(string methodName, Type[]? genericTypeParameters, Type[] parameterTypes)
    {
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

        return Method(method);
    }

    /// <summary>
    /// Create a method plug for a method with a return value given its name and parameters.
    /// </summary>
    /// <param name="methodName">The name of the method.</param>
    /// <param name="genericTypeParameters">The generic type parameters for the method.</param>
    /// <param name="parameterTypes">A collection of types matching the parameters for the method.</param>
    /// <returns>A new method plug.</returns>
    /// <exception cref="ArgumentException">When the method name is null or whitespace.</exception>
    public static IMethodPlug<TReturn> Method<TContainingType, TReturn>(string methodName, Type[]? genericTypeParameters, Type[] parameterTypes)
    {
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

        return Method<TReturn>(method);
    }

    /// <summary>
    /// Applies all plugs and returns a disposable scope.
    /// This scope should be disposed when the plugs are no longer needed (typically at the end of a test).
    /// If multiple calls to ApplyAsync occur, subsequent ones will block until a 
    /// lock on all plugged methods and properties can be obtained.
    /// </summary>
    /// <returns>A disposable scope.</returns>
    public static async Task<IDisposable> ApplyAsync(params IPlug[] plugs)
    {
        if (plugs.Length == 0)
        {
            throw new ArgumentException("At least one plug must be specified", nameof(plugs));
        }
        
        List<Plugs.IPlug> internalPlugs = new(plugs.Length);
        foreach(var plug in plugs)
        {
            if (plug is Plugs.IPlug internalPlug)
            {
                internalPlugs.Add(internalPlug);
            }
            else
            {
                throw new ArgumentException($"Plug {plug.GetType().FullName} is not a valid {typeof(IPlug).FullName}", nameof(plugs));
            }
        }

        foreach (var plug in internalPlugs.OrderBy(x => x.Id))
        {
            await plug.AcquireLockAsync();
        }

        var rv = new NoseCleaner(internalPlugs);
        try
        {
            //Ordering here not strictly necessary since we have acquired all locks
            foreach (var plug in internalPlugs.OrderBy(x => x.Id))
            {
                plug.Patch();
            }
        }
        catch (Exception)
        {
            rv.Dispose();
            throw;
        }
        return rv;
    }


    private sealed class NoseCleaner : IDisposable
    {
        private bool _isDisposed;

        public IReadOnlyList<Plugs.IPlug> Plugs { get; }

        public NoseCleaner(IReadOnlyList<Plugs.IPlug> plugs)
        {
            Plugs = plugs;
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    foreach (var plug in Plugs.OrderByDescending(x => x.Id))
                    {
                        plug.Dispose();
                    }
                }

                _isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
