
/* Unmerged change from project 'NosePlug (net48)'
Before:
using HarmonyLib;
using NosePlug.Plugs;
using System.Linq.Expressions;
After:
using System.Linq.Expressions;
using System.Reflection;
using HarmonyLib;
*/

/* Unmerged change from project 'NosePlug (net472)'
Before:
using HarmonyLib;
using NosePlug.Plugs;
using System.Linq.Expressions;
After:
using System.Linq.Expressions;
using System.Reflection;
using HarmonyLib;
*/

/* Unmerged change from project 'NosePlug (netcoreapp3.1)'
Before:
using HarmonyLib;
using NosePlug.Plugs;
using System.Linq.Expressions;
After:
using System.Linq.Expressions;
using System.Reflection;
using HarmonyLib;
*/
using System.Linq.Expressions;
using NosePlug.Plugs;

namespace NosePlug;

partial class Nasal
{
    /// <summary>
    /// Create a method plug for an instance void returning method
    /// </summary>
    /// <param name="method">The <see cref="MethodInfo"/> to create a plug for</param>
    /// <returns>A new method plug</returns>
    /// <exception cref="ArgumentNullException">When the passed in <see cref="MethodInfo"/> is <c>null</c></exception>
    public static IInstanceMethodPlug InstanceMethod(MethodInfo method)
    {
        if (method is null)
        {
            throw new ArgumentNullException(nameof(method));
        }

        return new InstanceMethodPlug(method);
    }

    /// <summary>
    /// Create a method plug for a void returning instance method
    /// </summary>
    /// <param name="methodExpression">An expression referencing a method. The parameter values passed in the expression, are ignored.</param>
    /// <returns>A new method plug.</returns>
    /// <exception cref="ArgumentNullException">When the passed in <see cref="Expression&lt;Action&gt;"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">When the passed in <see cref="Expression&lt;Action&gt;"/> is not a <see cref="MethodCallExpression"/>.</exception>
    public static IInstanceMethodPlug InstanceMethod<TContainingType>(Expression<Action<TContainingType>> methodExpression)
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
    /// Create a method plug for an instance method given its name and parameters.
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
            throw new NasalException($"'{methodName}' on '{typeof(TContainingType).FullName}' is static. Use {nameof(Nasal)}.{nameof(Method)} to plug it.");
        }

        if (genericTypeParameters is not null)
        {
            method = method.MakeGenericMethod(genericTypeParameters);
        }

        return InstanceMethod(method);
    }

    /// <summary>
    /// Create a method plug for an instance method with a return value.
    /// </summary>
    /// <typeparam name="TReturn">The return type of the method</typeparam>
    /// <param name="methodInfo">The <see cref="MethodInfo"/> to create a plug for</param>
    /// <returns>A new method plug</returns>
    /// <exception cref="ArgumentNullException">When the passed in <see cref="MethodInfo"/> is <c>null</c></exception>
    public static IInstanceMethodPlug<TReturn> InstanceMethod<TReturn>(MethodInfo methodInfo)
    {
        if (methodInfo is null)
        {
            throw new ArgumentNullException(nameof(methodInfo));
        }

        return new InstanceMethodPlug<TReturn>(methodInfo);
    }

    /// <summary>
    /// Create a method plug for a method with a return value
    /// </summary>
    /// <typeparam name="TContainingType">The type that contains the method to plug</typeparam>
    /// <typeparam name="TReturn">The return type of the method</typeparam>
    /// <param name="methodExpression">An expression referencing a method with a return value. The paramters passed in the expression, are ignored.</param>
    /// <returns>A new method plug.</returns>
    /// <exception cref="ArgumentNullException">When the passed in expression is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">When the passed in expression is not a <see cref="MethodCallExpression"/>.</exception>
    public static IInstanceMethodPlug<TReturn> InstanceMethod<TContainingType, TReturn>(Expression<Func<TContainingType, TReturn>> methodExpression)
    {
        if (methodExpression is null)
        {
            throw new ArgumentNullException(nameof(methodExpression));
        }

        if (methodExpression.Body is MethodCallExpression methodCallExpression)
        {
            MethodInfo original = methodCallExpression.Method;
            return InstanceMethod<TReturn>(original);
        }
        throw new ArgumentException("Expresion is not a method call expression", nameof(methodExpression));
    }

    /// <summary>
    /// Create a method plug for an instance method with a return value given its name and parameters.
    /// </summary>
    /// <param name="methodName">The name of the method.</param>
    /// <param name="parameterTypes">A collection of types matching the parameters for the method.</param>
    /// <returns>A new method plug.</returns>
    /// <exception cref="ArgumentException">When the method name is null or whitespace.</exception>
    public static IInstanceMethodPlug<TReturn> InstanceMethod<TContainingType, TReturn>(string methodName, params Type[] parameterTypes)
        => InstanceMethod<TContainingType, TReturn>(methodName, null, parameterTypes);

    /// <summary>
    /// Create a method plug for an instance method with a return value given its name and parameters.
    /// </summary>
    /// <param name="methodName">The name of the method.</param>
    /// <param name="genericTypeParameters">The generic type parameters for the method.</param>
    /// <param name="parameterTypes">A collection of types matching the parameters for the method.</param>
    /// <returns>A new method plug.</returns>
    /// <exception cref="ArgumentException">When the method name is null or whitespace.</exception>
    public static IInstanceMethodPlug<TReturn> InstanceMethod<TContainingType, TReturn>(string methodName, Type[]? genericTypeParameters, Type[] parameterTypes)
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

        return InstanceMethod<TReturn>(method);
    }

}
