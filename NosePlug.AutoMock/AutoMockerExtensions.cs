using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NosePlug.AutoMock
{
    public static class AutoMockerExtensions
    {
        public static INasalPlug StaticProperty<TProperty>(this Moq.AutoMock.AutoMocker mocker,
            Expression<Func<TProperty>> propertyExpression)
        {
            Nasal nasal = mocker.Get<Nasal>();
            return nasal.Property(propertyExpression);
        }

        public static async Task<IDisposable> ApplyStaticMocksAsync(this Moq.AutoMock.AutoMocker mocker)
        {
            Nasal nasal = mocker.Get<Nasal>();
            return await nasal.ApplyAsync();
        }
    }
}
