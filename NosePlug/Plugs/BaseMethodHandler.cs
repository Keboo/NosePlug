using System.Diagnostics.CodeAnalysis;

namespace NosePlug.Plugs;

internal abstract class BaseMethodHandler : IMethodHandler
{
    private bool _isDisposed;

    protected abstract Type ReturnType { get; }

    protected abstract MethodInfo PrefixInfo { get; }

    private static Dictionary<InterceptorKey, IMethodHandler> Callbacks { get; } = new();

    private InterceptorKey Key { get; }

    public bool ShouldCallOriginal { get; set; }

    public BaseMethodHandler(InterceptorKey key)
    {
        Key = key;
    }

    public void Patch(PatchProcessor processor)
    {
        lock (Callbacks)
        {
            Callbacks[Key] = this;
        }

        processor.AddPrefix(PrefixInfo);
        _ = processor.Patch();
    }

    public void AssertMatches(MethodInfo original)
    {
        var originalParameters = original.GetParameters();
        var parameters = PrefixInfo.GetParameters().Where(x => x.Name != "__originalMethod" && x.Name != "__result")
            .Select(x => x.ParameterType).ToArray();

        bool useThis = !original.IsStatic && originalParameters.Length + 1 == parameters.Length;
        bool matches = true;
        if (originalParameters.Length == parameters.Length ||
            originalParameters.Length + 1 == parameters.Length || //Instance methods accessing 'this'
            parameters.Length == 0)
        {
            int parameterOffset = 0;
            if (useThis)
            {
                //Check for 'this' parameter match
                matches = parameters[0] == original.DeclaringType ||
                    parameters[0].IsAssignableFrom(original.DeclaringType);
                parameterOffset = 1;
            }

            for (int i = 0; matches && i < parameters.Length - parameterOffset && matches && i < originalParameters.Length; i++)
            {
                ParameterInfo originalParameter = originalParameters[i];
                Type parameterType = parameters[parameterOffset + i];
                matches = originalParameter.ParameterType == parameterType ||
                    parameterType.IsAssignableFrom(originalParameter.ParameterType);
            }
        }
        else
        {
            matches = false;
        }

        if (!matches)
        {
            Type? thisType = useThis ? original.DeclaringType : null;
            throw new NasalException($"{GetPlugTypeDisplay()} has parameters ({GetTypeDisplay(parameters)}) that do not match original method parameters ({GetParametersDisplay(originalParameters, thisType)})");
        }

        if (original.ReturnType != ReturnType &&
            !ReturnType.IsAssignableFrom(original.ReturnType))
        {
            throw new NasalException($"{GetPlugTypeDisplay()} has return type ({ReturnType.FullName}) that do not match original method return type ({original.ReturnType.FullName})");
        }

        string GetPlugTypeDisplay()
            => $"Plug for {original.DeclaringType?.FullName}.{original.Name}";

        static string GetParametersDisplay(ParameterInfo[] parameters, Type? thisType)
            => GetTypeDisplay(parameters.Select(x => x.ParameterType), thisType);

        static string GetTypeDisplay(IEnumerable<Type> types, Type? thisType = null)
        {
            var rv = string.Join(", ", GetParts());
            return rv.Length == 0 ? "<empty>" : rv;

            IEnumerable<string> GetParts()
            {
                if (thisType is not null)
                {
                    yield return $"{thisType.FullName} this";
                }
                foreach (var type in types)
                {
                    yield return type.FullName ?? "";
                }
            }
        }
    }

    protected static bool TryGetHandler<THandler>(MethodBase originalMethod,
        [NotNullWhen(true)] out THandler? handler)
        where THandler : IMethodHandler
    {
        bool gotValue;
        IMethodHandler? methodHandler;
        lock (Callbacks)
        {
            gotValue = Callbacks.TryGetValue(InterceptorKey.FromMethod(originalMethod), out methodHandler);
        }
        if (gotValue && methodHandler is THandler typedHandler)
        {
            handler = typedHandler;
            return true;
        }
        handler = default;
        return false;
    }

    protected void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                lock (Callbacks)
                {
                    Callbacks.Remove(Key);
                }
            }
            _isDisposed = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        System.GC.SuppressFinalize(this);
    }
}
