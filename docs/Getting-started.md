# Getting Started

All of the methods start from the `NosePlug.Nasel` class. This class contains the methods needed to intercept static method calls. After setting up the needed interceptions invoke the `ApplyAsync()` method. This method will return a scope that is expected to be disposed when the static methods should no longer be invoked. Internally this method ensures that only a single scope is active and intercepting given static call at a time. Changes made after invoking this method will not take affect.

## Methods

To intercept a method simply use the `Method` method on a `Nasal` instance. The `INasalMethodPlug` instance that can be used to specify a delegate to be invoked instead of the static method using the `ReplaceWith` method. This can be used to validate that a method was invoked. If the method is invoked multiple times, only the last delegate is invoked.

```C#
[Fact]
public async Task ExampleTest()
{
    //Arrange
    int invocationCount = 0;

    Nasal mocker = new();
    mocker.Method(() => HasPublicMethod.NoParameters())
          .ReplaceWith(() => invocationCount++);

    using IDisposable _ = await mocker.ApplyAsync();

    //Act
    HasPublicMethod.NoParameters();

    //Assert
    Assert.Equal(1, invocationCount);
}
```

To specify a method that contains parameters, simply specify any value for each of the parameters. The values for these parameters are ignored, and only used to determine with method overload to replace. The delegate passed to `ReplaceWith` can be used to validate the parameters that were passed to the method.

If the method has a return value, a default value based on the return type will be automatically be returned.

```C#
[Fact]
public async Task ExampleTest()
{
    //Arrange
    int invocationCount = 0;
    string? passedString = null;
    int passedValue = 0;
    Nasal mocker = new();
    mocker.Method(() => HasPublicMethod.Overloaded("", 0))
        .ReplaceWith((string first, int second) => {
            passedString = first;
            passedValue = second;
            invocationCount++;
        });

    using IDisposable _ = await mocker.ApplyAsync();

    //Act
    HasPublicMethod.Overloaded("Foo", 42);

    //Assert
    Assert.Equal(1, invocationCount);
    Assert.Equal("Foo", passedString);
    Assert.Equal(42, passedValue);
}
```

For methods that return values, you can use the `Returns` method instead of the `ReplaceWith` method. This method allows for returning an alternate value. For async methods that return a Tasks, you **must** use this method and return a `Task` instance. Not returning a `Task` instance will result in a `null` value being returned, causing any called awaiting the `Task` to throw a `NullReferenceException`. 

```C#
[Fact]
public async Task ExampleTest()
{
    //Arrange
    Nasal mocker = new();
    mocker.Method(() => HasPublicMethod.AsyncMethodWithReturn())
          .Returns(() => Task.FromResult(42));

    using IDisposable _ = await mocker.ApplyAsync();

    //Act
    int value = await HasPublicMethod.AsyncMethodWithReturn();

    //Assert
    Assert.Equal(42, value);
}
```

## Properties

To intercept a method simply use the `Property` method on a `Nasal` instance. The `INasalPropertyPlug<T>` instance that can be used to specify delegates to be invoked instead of the getter and/or setter methods for the property. To intercept the getter use the `Returns` method.  

```C#
[Fact]
public async Task ExampleTest()
{
    //Arrange
    Guid testGuid = Guid.NewGuid();
    Nasal mocker = new();
    mocker.Property(() => HasPublicProperty.Foo)
          .Returns(() => testGuid);

    using IDisposable _ = await mocker.ApplyAsync();

    //Act
    Guid value = HasPublicProperty.Foo;

    //Assert
    Assert.Equal(testGuid, value);
}
```

To intercept the setter use the `ReplaceSetter` method to specify a delegate to be invoked instead of the original setter. If this method is called multiple times, only the last delegate will be invoked.

```C#
[Fact]
public async Task ExampleTest()
{
    //Arrange
    Guid testGuid = Guid.NewGuid();
    Guid passedGuid = Guid.Empty;

    Nasal mocker = new();
    mocker.Property(() => HasPublicProperty.Foo)
          .ReplaceSetter(x => passedGuid = x);

    using IDisposable _ = await mocker.ApplyAsync();

    //Act
    HasPublicProperty.Foo = testGuid;

    //Assert
    Assert.Equal(testGuid, passedGuid);
    Assert.NotEqual(testGuid, HasPublicProperty.Foo);
}
```

## Invoke Original

There may be cases where invoking the original method, in addition to the replacement delegate, is desired. For both properties and methods, this can be done using the `CallOriginal` method. Optionally you can specify a boolean indicating if the original method should be invoked. If this method is invoked multiple times, only the last value specified will be used. If the method specifies a return value, the original return value will be used.

```C#
[Fact]
public async Task ExampleTest()
{
    //Arrange
    int invocationCount = 0;
    Nasal mocker = new();
    mocker.Method(() => HasPublicMethod.Overloaded(0))
          .ReplaceWith(() => invocationCount++)
          .CallOriginal();

    using IDisposable _ = await mocker.ApplyAsync();
    HasPublicMethod.OverloadedValue = 0;

    //Act
    HasPublicMethod.Overloaded(42);

    //Assert
    Assert.Equal(1, invocationCount);
    Assert.Equal(42, HasPublicMethod.OverloadedValue);
}
```

