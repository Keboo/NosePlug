using System.Linq.Expressions;
using System.Reflection;
using NosePlug;

namespace Moq.AutoMock;

/// <summary>
/// A set of extension methods for adding Nasal plugs to an AutoMocker instance.
/// </summary>
public static partial class AutoMockerExtensions
{
    /// <summary>
    /// Add a plug to the AutoMocker instance. 
    /// </summary>
    /// <typeparam name="TPlug">The type of plug to add.</typeparam>
    /// <param name="mocker">The AutoMocker instance.</param>
    /// <param name="plug">The plug instance to add.</param>
    /// <returns>The original passed in plug.</returns>
    /// <exception cref="ArgumentNullException">If either the AutoMocker or plug instances are <c>null</c></exception>
    public static TPlug WithPlug<TPlug>(this AutoMocker mocker, TPlug plug)
        where TPlug : IPlug
    {
        if (mocker is null) throw new ArgumentNullException(nameof(mocker));
        if (plug is null) throw new ArgumentNullException(nameof(plug));

        mocker.Get<NasalPlugList>().Add(plug);
        return plug;
    }

    /// <summary>
    /// Add static method plug to the AutoMocker instance. 
    /// </summary>
    /// <param name="mocker">The AutoMocker instance.</param>
    /// <param name="methodExpression">An expression referencing a method. The paramters passed in the expression, are ignored.</param>
    /// <returns>The method plug.</returns>
    public static IMethodPlug StaticMethod(this AutoMocker mocker, Expression<Action> methodExpression)
        => WithPlug(mocker, Nasal.Method(methodExpression));

    /// <summary>
    /// Add static method plug to the AutoMocker instance. 
    /// </summary>
    /// <param name="mocker">The AutoMocker instance.</param>
    /// <param name="methodExpression">An expression referencing a method with a return value. The paramters passed in the expression, are ignored.</param>
    /// <returns>The method plug.</returns>
    public static IMethodPlug<TReturn> StaticMethod<TReturn>(this AutoMocker mocker, Expression<Func<TReturn>> methodExpression)
        => WithPlug(mocker, Nasal.Method(methodExpression));

    /// <summary>
    /// Add static method plug to the AutoMocker instance. 
    /// </summary>
    /// <param name="mocker">The AutoMocker instance.</param>
    /// <param name="methodInfo">The <see cref="MethodInfo"/> to create a plug for</param>
    /// <returns>The method plug.</returns>
    public static IMethodPlug<TReturn> StaticMethod<TReturn>(this AutoMocker mocker, MethodInfo methodInfo)
        => WithPlug(mocker, Nasal.Method<TReturn>(methodInfo));

    /// <summary>
    /// Add static method plug to the AutoMocker instance. 
    /// </summary>
    /// <param name="mocker">The AutoMocker instance.</param>
    /// <param name="methodInfo">The <see cref="MethodInfo"/> to create a plug for</param>
    /// <returns>The method plug.</returns>
    public static IMethodPlug StaticMethod(this AutoMocker mocker, MethodInfo methodInfo)
        => WithPlug(mocker, Nasal.Method(methodInfo));

    /// <summary>
    /// Add static method plug to the AutoMocker instance. 
    /// </summary>
    /// <param name="mocker">The AutoMocker instance.</param>
    /// <param name="methodName">The name of the method.</param>
    /// <param name="parameterTypes">A collection of types matching the parameters for the method.</param>
    /// <returns>The method plug.</returns>
    public static IMethodPlug StaticMethod<TContainingType>(this AutoMocker mocker, string methodName, params Type[] parameterTypes)
        => StaticMethod<TContainingType>(mocker, methodName, null, parameterTypes);

    /// <summary>
    /// Add static method plug to the AutoMocker instance. 
    /// </summary>
    /// <param name="mocker">The AutoMocker instance.</param>
    /// <param name="methodName">The name of the method.</param>
    /// <param name="genericTypeParameters">The generic type parameters for the method.</param>
    /// <param name="parameterTypes">A collection of types matching the parameters for the method.</param>
    /// <returns>The method plug.</returns>
    public static IMethodPlug StaticMethod<TContainingType>(this AutoMocker mocker, string methodName, Type[]? genericTypeParameters, Type[] parameterTypes)
        => WithPlug(mocker, Nasal.Method<TContainingType>(methodName, genericTypeParameters, parameterTypes));


    /// <summary>
    /// Add static property plug to the AutoMocker instance.
    /// </summary>
    /// <param name="mocker">The AutoMocker instance.</param>
    /// <param name="propertyExpression">An expression representing the property to create a plug for</param>
    /// <returns>The property plug.</returns>
    public static IPropertyPlug<TProperty> StaticProperty<TProperty>(this AutoMocker mocker,
        Expression<Func<TProperty>> propertyExpression)
        => WithPlug(mocker, Nasal.Property(propertyExpression));

    /// <summary>
    /// Add static property plug to the AutoMocker instance.
    /// </summary>
    /// <typeparam name="T">The type that contains the property</typeparam>
    /// <typeparam name="TProperty">The type of the property</typeparam>
    /// <param name="mocker">The AutoMocker instance.</param>
    /// <param name="name">The name of the property</param>
    /// <returns>The property plug.</returns>
    public static IPropertyPlug<TProperty> StaticProperty<T, TProperty>(this AutoMocker mocker, string name)
        => WithPlug(mocker, Nasal.Property<T, TProperty>(name));

    /// <summary>
    /// Add static property plug to the AutoMocker instance.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property</typeparam>
    /// <param name="mocker">The AutoMocker instance.</param>
    /// <param name="property">The <see cref="PropertyInfo"/> to create a plug for.</param>
    /// <returns>The property plug.</returns>
    public static IPropertyPlug<TProperty> StaticProperty<TProperty>(this AutoMocker mocker, PropertyInfo property)
        => WithPlug(mocker, Nasal.Property<TProperty>(property));


    /// <summary>
    /// Applies all plugs stored in the AutoMocker instance and returns a disposable scope.
    /// This scope should be disposed when the plugs are no longer needed (typically at the end of a test).
    /// If multiple calls to ApplyAsync occur, subsequent ones will block until a 
    /// lock on all plugged methods and properties can be obtained.
    /// </summary>
    /// <returns>A disposable scope.</returns>
    public static async Task<IDisposable> ApplyStaticMocksAsync(this AutoMocker mocker)
        => await mocker.Get<NasalPlugList>().ApplyAsync();
}

