using NosePlug;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Moq.AutoMock
{
    public static partial class AutoMockerExtensions
    {
        public static INasalMethodPlug StaticMethod(this AutoMocker mocker, Expression<Action> methodExpression)
            => mocker.Get<Nasal>().Method(methodExpression);

        public static INasalMethodPlug StaticMethod(this AutoMocker mocker, Expression<Func<Task>> methodExpression)
            => mocker.Get<Nasal>().Method(methodExpression);

        public static INasalMethodPlug StaticMethod<T>(this AutoMocker mocker, string methodName, params Type[] parameterTypes)
            => StaticMethod<T>(mocker, methodName, null, parameterTypes);

        public static INasalMethodPlug StaticMethod<T>(this AutoMocker mocker, string methodName, Type[]? genericTypeParameters, Type[] parameterTypes)
            => mocker.Get<Nasal>().Method<T>(methodName, genericTypeParameters, parameterTypes);

        public static INasalMethodPlug StaticMethod<TProperty>(this AutoMocker mocker,
            MethodInfo methodInfo)
            => mocker.Get<Nasal>().Method(methodInfo);


        public static INasalPropertyPlug<TProperty> StaticProperty<TProperty>(this AutoMocker mocker,
            Expression<Func<TProperty>> propertyExpression)
            => mocker.Get<Nasal>().Property(propertyExpression);

        public static INasalPropertyPlug<TProperty> StaticProperty<T, TProperty>(this AutoMocker mocker, string name)
            => mocker.Get<Nasal>().Property<T, TProperty>(name);

        public static INasalPropertyPlug<TProperty> StaticProperty<TProperty>(this AutoMocker mocker, PropertyInfo property)
            => mocker.Get<Nasal>().Property<TProperty>(property);


        public static async Task<IDisposable> ApplyStaticMocksAsync(this AutoMocker mocker)
            => await mocker.Get<Nasal>().ApplyAsync();
    }
}
