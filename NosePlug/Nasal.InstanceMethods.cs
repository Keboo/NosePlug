using HarmonyLib;
using NosePlug.Plugs;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NosePlug;

partial class Nasal
{

    public static IInstanceMethodPlug InstanceMethod(MethodInfo method)
    {
        if (method is null)
        {
            throw new ArgumentNullException(nameof(method));
        }

        return new InstanceMethodPlug(method);
    }

    public static IInstanceMethodPlug InstanceMethod<TInstance>(Expression<Action<TInstance>> methodExpression)
    {
        if (methodExpression is null)
        {
            throw new ArgumentNullException(nameof(methodExpression));
        }

        if (methodExpression.Body is MethodCallExpression methodCallExpression)
        {
            MethodInfo original = methodCallExpression.Method;
            return InstanceMethod(original);
        }
        throw new ArgumentException("Expresion is not a method call expression");
    }

    /// <summary>
    /// Create a method plug for a static method given its name and parameters.
    /// </summary>
    /// <param name="methodName">The name of the method.</param>
    /// <param name="parameterTypes">A collection of types matching the parameters for the method.</param>
    /// <returns>A new method plug.</returns>
    /// <exception cref="ArgumentException">When the method name is null or whitespace.</exception>
    public static IInstanceMethodPlug InstanceMethod<TContainingType>(string methodName, params Type[] parameterTypes)
        => InstanceMethod<TContainingType>(methodName, null, parameterTypes);

    /// <summary>
    /// Create a method plug for a method with a return value given its name and parameters.
    /// </summary>
    /// <param name="methodName">The name of the method.</param>
    /// <param name="parameterTypes">A collection of types matching the parameters for the method.</param>
    /// <returns>A new method plug.</returns>
    /// <exception cref="ArgumentException">When the method name is null or whitespace.</exception>
    //public static IInstanceMethodPlug<TReturn> Method<TContainingType, TReturn>(string methodName, params Type[] parameterTypes)
    //    => InstanceMethod<TContainingType, TReturn>(methodName, null, parameterTypes);

    /// <summary>
    /// Create a method plug for a method given its name and parameters.
    /// </summary>
    /// <param name="methodName">The name of the method.</param>
    /// <param name="genericTypeParameters">The generic type parameters for the method.</param>
    /// <param name="parameterTypes">A collection of types matching the parameters for the method.</param>
    /// <returns>A new method plug.</returns>
    /// <exception cref="ArgumentException">When the method name is null or whitespace.</exception>
    public static IInstanceMethodPlug InstanceMethod<TContainingType>(string methodName, Type[]? genericTypeParameters, Type[] parameterTypes)
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

        if (method.IsStatic)
        {
            throw new NasalException($"'{methodName}' on '{typeof(TContainingType).FullName}' is static. Use {nameof(Nasal)}.{nameof(Method)} to plug it.")
        }    

        if (genericTypeParameters is not null)
        {
            method = method.MakeGenericMethod(genericTypeParameters);
        }

        return InstanceMethod(method);
    }

    /// <summary>
    /// Create a method plug for a method with a return value
    /// </summary>
    /// <typeparam name="TReturn">The return type of the method</typeparam>
    /// <param name="methodInfo">The <see cref="MethodInfo"/> to create a plug for</param>
    /// <returns>A new method plug</returns>
    /// <exception cref="ArgumentNullException">When the passed in <see cref="MethodInfo"/> is <c>null</c></exception>
    public static IMethodPlug<TReturn> InstanceMethod<TReturn>(MethodInfo methodInfo)
    {
        if (methodInfo is null)
        {
            throw new ArgumentNullException(nameof(methodInfo));
        }

        return new MethodPlug<TReturn>(methodInfo);
    }

    /// <summary>
    /// Create a method plug for a method with a return value
    /// </summary>
    /// <typeparam name="TReturn">The return type of the method</typeparam>
    /// <param name="methodExpression">An expression referencing a method with a return value. The paramters passed in the expression, are ignored.</param>
    /// <returns>A new method plug.</returns>
    /// <exception cref="ArgumentNullException">When the passed in expression is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">When the passed in expression is not a <see cref="MethodCallExpression"/>.</exception>
    public static IMethodPlug<TReturn> InstanceMethod<TReturn>(Expression<Func<TReturn>> methodExpression)
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

    public static IMethodPlug<TReturn> InstanceMethod<T, TReturn>(Expression<Func<T, TReturn>> methodExpression)
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
}
