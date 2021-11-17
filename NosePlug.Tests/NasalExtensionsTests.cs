using NosePlug.Tests.TestClasses;
using System;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;

namespace NosePlug.Tests
{
    public class NasalExtensionsTests
    {
        [Fact]
        public void Property_WhenNameIsNull_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => NasalExtensions.Property<HasPublicProperty, Guid>(new Nasal(), null!));
        }

        [Fact]
        public void Property_WhenPropertyNameIsNotFound_ThrowsException()
        {
            var ex = Assert.Throws<MissingMemberException>(() => NasalExtensions.Property<HasPublicProperty, Guid>(new Nasal(), "DoesNotExist"));
            Assert.Contains("Could not find property 'DoesNotExist'", ex.Message);
        }

        [Fact]
        public void Property_WhenNasalIsNull_ThrowsException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => NasalExtensions.Property<HasPublicProperty, Guid>(null!, nameof(HasPublicProperty.Foo)));
            Assert.Equal("nasal", ex.ParamName);
        }


        [Fact]
        public void PropertyExpression_WhenNasalIsNull_ThrowsException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => NasalExtensions.Property(null!, () => HasPublicProperty.Foo));
            Assert.Equal("nasal", ex.ParamName);
        }

        [Fact]
        public void PropertyExpression_WhenExpressionIsNull_ThrowsException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => NasalExtensions.Property<Guid>(new Nasal(), null!));
            Assert.Equal("propertyExpression", ex.ParamName);
        }

        [Fact]
        public void PropertyExpression_WhenExpressionIsNotAPropertyExpression_ThrowsException()
        {
            var ex = Assert.Throws<ArgumentException>(() => NasalExtensions.Property<Guid>(new Nasal(), () => Guid.NewGuid()));
            Assert.Equal("Expresion is not a member expression to a property", ex.Message);
        }

        [Fact]
        public void PropertyNonGeneric_WhenNasalIsNull_ThrowsException()
        {
            PropertyInfo property = typeof(HasPublicProperty).GetProperty(nameof(HasPublicProperty.Foo))!;
            var ex = Assert.Throws<ArgumentNullException>(() => NasalExtensions.Property(null!, property));
            Assert.Equal("nasal", ex.ParamName);
        }


        [Fact]
        public void MethodExpression_WhenNasalIsNull_ThrowsException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => NasalExtensions.Method(null!, () => HasPublicMethod.NoParameters()));
            Assert.Equal("nasal", ex.ParamName);
        }

        [Fact]
        public void MethodExpression_WhenExpressionIsNull_ThrowsException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => NasalExtensions.Method(new Nasal(), null!));
            Assert.Equal("methodExpression", ex.ParamName);
        }

        [Fact]
        public void MethodExpressionGeneric_WhenNasalIsNull_ThrowsException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => NasalExtensions.Method(null!, () => HasPublicMethod.ReturnValue()));
            Assert.Equal("nasal", ex.ParamName);
        }

        [Fact]
        public void MethodExpressionGeneric_WhenExpressionIsNull_ThrowsException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => NasalExtensions.Method(new Nasal(), (Expression<Func<int>>)null!));
            Assert.Equal("methodExpression", ex.ParamName);
        }

        [Fact]
        public void MethodExpressionGeneric_WhenExpressionIsNotAMethodCall_ThrowsException()
        {
            var ex = Assert.Throws<ArgumentException>(() => NasalExtensions.Method(new Nasal(), () => HasPublicProperty.Foo));
            Assert.Equal("methodExpression", ex.ParamName);
            Assert.Contains("Expresion is not a method call expression", ex.Message);
        }

        [Fact]
        public void MethodGeneric_WhenNasalIsNull_ThrowsException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => NasalExtensions.Method<HasPublicMethod>(null!, nameof(HasPublicMethod.NoParameters)));
            Assert.Equal("nasal", ex.ParamName);
        }

        [Fact]
        public void MethodGeneric_WhenMethodNameIsNull_ThrowsException()
        {
            var ex = Assert.Throws<ArgumentException>(() => NasalExtensions.Method<HasPublicMethod>(new Nasal(), (string)null!));
            Assert.Equal("methodName", ex.ParamName);
        }

        [Fact]
        public void MethodGeneric_WhenMethodNameIsNotFound_ThrowsException()
        {
            var ex = Assert.Throws<MissingMethodException>(() => NasalExtensions.Method<HasPublicMethod>(new Nasal(), "DoesNotExist"));
            Assert.Contains("HasPublicMethod", ex.Message);
            Assert.Contains("DoesNotExist", ex.Message);
        }
    }
}
