using NosePlug;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

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

    public static IMethodPlug StaticMethod(this AutoMocker mocker, Expression<Action> methodExpression)
        => WithPlug(mocker, Nasal.Method(methodExpression));

    public static IMethodPlug<Task> StaticMethod(this AutoMocker mocker, Expression<Func<Task>> methodExpression) 
        => WithPlug(mocker, Nasal.Method(methodExpression));

    public static IMethodPlug<TReturn> StaticMethod<TReturn>(this AutoMocker mocker, MethodInfo methodInfo)
        => WithPlug(mocker, Nasal.Method<TReturn>(methodInfo));

    public static IMethodPlug StaticMethod(this AutoMocker mocker, MethodInfo methodInfo)
        => WithPlug(mocker, Nasal.Method(methodInfo));

    public static IMethodPlug StaticMethod<TContainingType>(this AutoMocker mocker, string methodName, params Type[] parameterTypes)
        => StaticMethod<TContainingType>(mocker, methodName, null, parameterTypes);

    public static IMethodPlug StaticMethod<TContainingType>(this AutoMocker mocker, string methodName, Type[]? genericTypeParameters, Type[] parameterTypes)
        => WithPlug(mocker, Nasal.Method<TContainingType>(methodName, genericTypeParameters, parameterTypes));


    public static IPropertyPlug<TProperty> StaticProperty<TProperty>(this AutoMocker mocker,
        Expression<Func<TProperty>> propertyExpression)
        => WithPlug(mocker, Nasal.Property(propertyExpression));

    public static IPropertyPlug<TProperty> StaticProperty<T, TProperty>(this AutoMocker mocker, string name)
        => WithPlug(mocker, Nasal.Property<T, TProperty>(name));

    public static IPropertyPlug<TProperty> StaticProperty<TProperty>(this AutoMocker mocker, PropertyInfo property)
        => WithPlug(mocker, Nasal.Property<TProperty>(property));


    public static async Task<IDisposable> ApplyStaticMocksAsync(this AutoMocker mocker)
        => await mocker.Get<NasalPlugList>().ApplyAsync();
}

