# Getting Started

All of the methods start from the `NosePlug.Nasal` class. This class contains the methods needed to create method plugs that intercept method calls. After creating the needed plugs invoke the `ApplyAsync()` method passing all plugs to be applied. This method will return a scope that is expected to be disposed when the plugs should no longer be applied. Internally this method ensures that only a single plugs is active and intercepting a given static call at a time. Changes made after invoking this method will not take affect.

## Methods

To intercept a method simply use the `Method` method on a `Nasal` instance. The `IMethodPlug` instance that can be used to specify a delegate to be invoked instead of the static method using the `Callback` method. This can be used to validate that a method was invoked. If the method is invoked multiple times, only the last delegate is invoked.

```C#
public class HasPublicMethod
{
    public static void NoParameters() { ... }
}

[Fact]
public async Task ExampleTest()
{
    //Arrange
    int invocationCount = 0;

    var plug = Nasal.Method(() => HasPublicMethod.NoParameters())
                    .Callback(() => invocationCount++);

    using IDisposable _ = await Nasal.ApplyAsync(plug);

    //Act
    HasPublicMethod.NoParameters();

    //Assert
    Assert.Equal(1, invocationCount);
}
```

To create a plug for a method that contains parameters, simply specify any value for each of the parameters. The values for these parameters are ignored, and only used to determine with method overload to replace. The delegate passed to `Callback` can be used to validate the parameters that were passed to the method.

If the method has a return value, a default value based on the return type will be automatically be returned.

```C#
public class HasPublicMethod
{
    public static void Overloaded(int value) { ... }
    public static void Overloaded(string @string, int value) { ... }
}

[Fact]
public async Task ExampleTest()
{
    //Arrange
    int invocationCount = 0;
    string? passedString = null;
    int passedValue = 0;

    var plug = Nasal.Method(() => HasPublicMethod.Overloaded("", 0))
        .Callback((string first, int second) => {
            passedString = first;
            passedValue = second;
            invocationCount++;
        });

    using IDisposable _ = await Nasal.ApplyAsync(plug);

    //Act
    HasPublicMethod.Overloaded("Foo", 42);

    //Assert
    Assert.Equal(1, invocationCount);
    Assert.Equal("Foo", passedString);
    Assert.Equal(42, passedValue);
}
```

For methods that return values, you can use the `Returns` method instead of the `Callback` method. This method allows for returning an alternate value. For async methods that return a `Task`, you **must** return a `Task` instance. Not returning a `Task` instance will result in a `null` value being returned, causing any called awaiting the `Task` to throw a `NullReferenceException`. 

```C#
public class HasPublicMethod
{
    public static async Task<int> AsyncMethodWithReturn() { ... }
}

[Fact]
public async Task ExampleTest()
{
    //Arrange
    var plug = Nasal.Method(() => HasPublicMethod.AsyncMethodWithReturn())
                    .Returns(() => Task.FromResult(42));

    using IDisposable _ = await Nasal.ApplyAsync(plug);

    //Act
    int value = await HasPublicMethod.AsyncMethodWithReturn();

    //Assert
    Assert.Equal(42, value);
}
```

## Properties

To intercept a property simply use the `Property` method on the `Nasal` class. The `IPropertyPlug<T>` instance that can be used to specify delegates to be invoked instead of the getter and/or setter methods for the property. To intercept the getter use the `Returns` method.  

```C#
public class HasPublicProperty
{
    public static Guid Foo { get; set; }
}

[Fact]
public async Task ExampleTest()
{
    //Arrange
    Guid testGuid = Guid.NewGuid();
    var plug = Nasal.Property(() => HasPublicProperty.Foo)
                    .Returns(() => testGuid);

    using IDisposable _ = await Nasal.ApplyAsync(plug);

    //Act
    Guid value = HasPublicProperty.Foo;

    //Assert
    Assert.Equal(testGuid, value);
}
```

To intercept the setter use the `Callback` method to specify a delegate to be invoked *instead* of the original setter. If this method is called multiple times, only the last delegate will be invoked.

```C#
public class HasPublicProperty
{
    public static Guid Foo { get; set; }
}

[Fact]
public async Task ExampleTest()
{
    //Arrange
    Guid testGuid = Guid.NewGuid();
    Guid passedGuid = Guid.Empty;

    var plug = Nasal.Property(() => HasPublicProperty.Foo)
                    .Callback(x => passedGuid = x);

    using IDisposable _ = await Nasal.ApplyAsync(plug);

    //Act
    HasPublicProperty.Foo = testGuid;

    //Assert
    Assert.Equal(testGuid, passedGuid);
    Assert.NotEqual(testGuid, HasPublicProperty.Foo);
}
```

## Invoke Original

There may be cases where invoking the original method, in addition to the replacement delegate, is desired. For both properties and methods, this can be done using the `CallOriginal` method. Optionally you can specify a boolean indicating if the original method should be invoked. If this method is invoked multiple times, only the last value specified will be used. If the plugged method specifies a return value, the original return value will be used.

```C#
public class HasPublicMethod
{
    public static int OverloadedValue { get; set; }

    public static void Overloaded(int value)
    {
        OverloadedValue = value;
    }
}

[Fact]
public async Task ExampleTest()
{
    //Arrange
    int invocationCount = 0;
    var plug = Nasal.Method(() => HasPublicMethod.Overloaded(0))
                    .Callback(() => invocationCount++)
                    .CallOriginal();

    using IDisposable _ = await Nasal.ApplyAsync(plug);
    HasPublicMethod.OverloadedValue = 0;

    //Act
    HasPublicMethod.Overloaded(42);

    //Assert
    Assert.Equal(1, invocationCount);
    Assert.Equal(42, HasPublicMethod.OverloadedValue);
}
```

