# NosePlug
Sometimes you have to test code that smells. When using static methods it tightly couples two pieces of code together. When over used this can lead to code that is difficult to test since all coupled code will be invoked. Ideally the code should be refactored to eliminate this problem, however in the real world ideals are not always possible. This library serves to be a half way point to allow testing to occur during the interim before appropriate refactoring can occur.

## Getting started
This library's API is heavily influenced by Moq. 
See the documentation for the full [getting started guide](docs/GettingStarted.md).

## Limitations
This library relies on [Harmony](https://harmony.pardeike.net/) to monkey patch methods. This library maintains all of the same [limitations of Harmony](https://harmony.pardeike.net/articles/patching-edgecases.html).
The most common failure comes from inclining. This is most apparent when compiling in Release configuration (optimizations enabled).

