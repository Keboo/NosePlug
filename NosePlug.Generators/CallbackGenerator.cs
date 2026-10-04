using System.Text;
using Microsoft.CodeAnalysis;

namespace NodePlug.Generators
{
    [Generator]
    public class CallbackGenerator : ISourceGenerator
    {
        private const int NumberOfParameters = 16;

        public void Initialize(GeneratorInitializationContext context)
        { }

        public void Execute(GeneratorExecutionContext context)
        {
            StringBuilder handlerBuilder = new();
            StringBuilder voidStaticInterfaceBuilder = new();
            StringBuilder voidInstanceInterfaceBuilder = new();
            StringBuilder voidStaticMethodPlugBuilder = new();
            StringBuilder voidInstanceMethodPlugBuilder = new();
            StringBuilder staticMethodPlugBuilder = new();
            StringBuilder instanceMethodPlugBuilder = new();
            StringBuilder staticInterfaceBuilder = new();
            StringBuilder instanceInterfaceBuilder = new();

            handlerBuilder.AppendLine(@"
#nullable enable
using System;
using System.Reflection;

namespace NosePlug.Plugs;
");

            voidStaticMethodPlugBuilder.AppendLine($@"
#nullable enable
using System;

namespace NosePlug.Plugs;

partial class MethodPlug
{{");

            voidInstanceMethodPlugBuilder.AppendLine($@"
#nullable enable
using System;

namespace NosePlug.Plugs;

partial class InstanceMethodPlug
{{");

            staticInterfaceBuilder.AppendLine(@$"
using System;

namespace NosePlug;

partial interface IMethodPlug<TReturn>
{{");

            instanceInterfaceBuilder.AppendLine(@$"
using System;

namespace NosePlug;

partial interface IInstanceMethodPlug<TReturn>
{{");

            voidStaticInterfaceBuilder.AppendLine(@$"
using System;

namespace NosePlug;

partial interface IMethodPlug
{{");

            voidInstanceInterfaceBuilder.AppendLine(@$"
using System;

namespace NosePlug;

partial interface IInstanceMethodPlug
{{");

            staticMethodPlugBuilder.AppendLine($@"
#nullable enable
using System;

namespace NosePlug.Plugs;

partial class MethodPlug<TReturn>
{{");

            instanceMethodPlugBuilder.AppendLine($@"
#nullable enable
using System;

namespace NosePlug.Plugs;

partial class InstanceMethodPlug<TReturn>
{{");

            for (int numParameters = 0; numParameters <= NumberOfParameters; numParameters++)
            {
                string genericTypes;
                string genericTypesWithReturn;
                string instanceGenericTypes;
                string instanceGenericTypesWithReturn;
                if (numParameters == 0)
                {
                    genericTypes = "";
                    genericTypesWithReturn = "<TReturn>";
                    instanceGenericTypes = "<TInstance>";
                    instanceGenericTypesWithReturn = "<TInstance, TReturn>";
                }
                else
                {
                    genericTypes = $"<{string.Join(", ", Enumerable.Range(1, numParameters).Select(x => $"T{x}"))}>";
                    genericTypesWithReturn = $"<{string.Join(", ", Enumerable.Range(1, numParameters).Select(x => $"T{x}"))}, TReturn>";
                    instanceGenericTypes = $"<TInstance, {string.Join(", ", Enumerable.Range(1, numParameters).Select(x => $"T{x}"))}>";
                    instanceGenericTypesWithReturn = $"<TInstance, {string.Join(", ", Enumerable.Range(1, numParameters).Select(x => $"T{x}"))}, TReturn>";
                }

                //Static void handler
                handlerBuilder.AppendLine(@$"
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
internal sealed class VoidMethodHandler{genericTypes} : BaseMethodHandler
{{
    protected override MethodInfo PrefixInfo {{ get; }}
        = typeof(VoidMethodHandler{genericTypes}).GetMethod(nameof(PrefixMethod)) ?? throw new MissingMethodException();

    protected override Type ReturnType {{ get; }} = typeof(void);

    private Action{genericTypes} Callback {{ get; }}

    public VoidMethodHandler(InterceptorKey key, Action{genericTypes} callback)
        : base(key)
    {{
        Callback = callback;
    }}

    public static bool PrefixMethod({string.Join(", ", GetPrefixParameters(numParameters))})
    {{
        if (TryGetHandler(__originalMethod, out VoidMethodHandler{genericTypes}? handler))
        {{
            handler.Callback({string.Join(", ", GetCallbackParameters(numParameters))});
            return handler.ShouldCallOriginal;
        }}
        return true;
    }}
}}");
                //Static with return type
                handlerBuilder.AppendLine($@"
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
internal sealed class MethodHandler{genericTypesWithReturn} : BaseMethodHandler
{{
    protected override MethodInfo PrefixInfo {{ get; }}
        = typeof(MethodHandler{genericTypesWithReturn}).GetMethod(nameof(MethodWithReturnPrefix)) ?? throw new MissingMethodException();

    protected override Type ReturnType {{ get; }} = typeof(TReturn);

    private Func{genericTypesWithReturn} Callback {{ get; }}

    public MethodHandler(InterceptorKey key, Func{genericTypesWithReturn} callback)
        : base(key)
    {{
        Callback = callback;
    }}

    public static bool MethodWithReturnPrefix(ref TReturn __result, {string.Join(", ", GetPrefixParameters(numParameters))})
    {{
        if (TryGetHandler(__originalMethod, out MethodHandler{genericTypesWithReturn}? handler))
        {{
            __result = handler.Callback({string.Join(", ", GetCallbackParameters(numParameters))});
            return handler.ShouldCallOriginal;
        }}
        return true;
    }}
}}");

                if (numParameters <= 15)
                {
                    //Instance void method
                    handlerBuilder.AppendLine(@$"
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
internal sealed class VoidInstanceMethodHandler{instanceGenericTypes} : BaseMethodHandler
{{
    protected override MethodInfo PrefixInfo {{ get; }}
        = typeof(VoidInstanceMethodHandler{instanceGenericTypes}).GetMethod(nameof(PrefixMethod)) ?? throw new MissingMethodException();

    protected override Type ReturnType {{ get; }} = typeof(void);

    private Action{instanceGenericTypes} Callback {{ get; }}

    public VoidInstanceMethodHandler(InterceptorKey key, Action{instanceGenericTypes} callback)
        : base(key)
    {{
        Callback = callback;
    }}

    public static bool PrefixMethod(TInstance __instance, {string.Join(", ", GetPrefixParameters(numParameters))})
    {{
        if (TryGetHandler(__originalMethod, out VoidInstanceMethodHandler{instanceGenericTypes}? handler))
        {{
            handler.Callback(__instance {(numParameters > 0 ? "," : "")}{string.Join(", ", GetCallbackParameters(numParameters))});
            return handler.ShouldCallOriginal;
        }}
        return true;
    }}
}}");
                }

                if (numParameters <= 14)
                {
                    //Instance with return type
                    handlerBuilder.AppendLine($@"
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
internal sealed class InstanceMethodHandler{instanceGenericTypesWithReturn} : BaseMethodHandler
{{
    protected override MethodInfo PrefixInfo {{ get; }}
        = typeof(InstanceMethodHandler{instanceGenericTypesWithReturn}).GetMethod(nameof(MethodWithReturnPrefix)) ?? throw new MissingMethodException();

    protected override Type ReturnType {{ get; }} = typeof(TReturn);

    private Func{instanceGenericTypesWithReturn} Callback {{ get; }}

    public InstanceMethodHandler(InterceptorKey key, Func{instanceGenericTypesWithReturn} callback)
        : base(key)
    {{
        Callback = callback;
    }}

    public static bool MethodWithReturnPrefix(TInstance __instance, ref TReturn __result, {string.Join(", ", GetPrefixParameters(numParameters))})
    {{
        if (TryGetHandler(__originalMethod, out InstanceMethodHandler{instanceGenericTypesWithReturn}? handler))
        {{
            __result = handler.Callback(__instance{(numParameters > 0 ? ", " : "")}{string.Join(", ", GetCallbackParameters(numParameters))});
            return handler.ShouldCallOriginal;
        }}
        return true;
    }}
}}");
                }

                voidStaticInterfaceBuilder.AppendLine("    /// <summary>");
                voidStaticInterfaceBuilder.AppendLine("    /// Add a callback that will be invoked instead of the original static method.");
                voidStaticInterfaceBuilder.AppendLine("    /// </summary>");
                for (int i = 0; i < numParameters; i++)
                {
                    voidStaticInterfaceBuilder.AppendLine($"    /// <typeparam name=\"T{i + 1}\"></typeparam>");

                }
                voidStaticInterfaceBuilder.AppendLine("    /// <param name=\"replacement\">The replacement delegate to invoke.</param>");
                voidStaticInterfaceBuilder.AppendLine("    /// <returns>The original method plug.</returns>");
                voidStaticInterfaceBuilder.AppendLine(@$"    IMethodPlug Callback{genericTypes}(Action{genericTypes} replacement);");

                voidStaticMethodPlugBuilder.AppendLine($@"
    /// <inheritdoc />
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public IMethodPlug Callback{genericTypes}(Action{genericTypes} replacement)
    {{
        if (Original.ReturnType == typeof(void))
        {{
            MethodHandler = new VoidMethodHandler{genericTypes}(Key, replacement);
        }}
        else
        {{
            MethodHandler = new MethodHandler<{genericTypes.Trim('<', '>')}{(numParameters > 0 ? ", " : "")}object?>(Key, ({string.Join(", ", GetPrefixParameters(numParameters).Skip(1))}) =>
            {{
                replacement({string.Join(", ", GetCallbackParameters(numParameters))});
                return GetDefaultValue(Original.ReturnType);
            }});
        }}
        return this;
    }}");
                if (numParameters <= 14)
                {
                    voidInstanceInterfaceBuilder.AppendLine("    /// <summary>");
                    voidInstanceInterfaceBuilder.AppendLine("    /// Add a callback that will be invoked instead of the original instance method.");
                    voidInstanceInterfaceBuilder.AppendLine("    /// </summary>");
                    voidInstanceInterfaceBuilder.AppendLine($"    /// <typeparam name=\"TInstance\">The declaring type of the method</typeparam>");
                    for (int i = 0; i < numParameters; i++)
                    {
                        voidInstanceInterfaceBuilder.AppendLine($"    /// <typeparam name=\"T{i + 1}\"></typeparam>");
                    }
                    voidInstanceInterfaceBuilder.AppendLine("    /// <param name=\"replacement\">The replacement delegate to invoke.</param>");
                    voidInstanceInterfaceBuilder.AppendLine("    /// <returns>The original method plug.</returns>");
                    voidInstanceInterfaceBuilder.AppendLine(@$"    IInstanceMethodPlug Callback{instanceGenericTypes}(Action{instanceGenericTypes} replacement);");


                    voidInstanceMethodPlugBuilder.AppendLine($@"
    /// <inheritdoc />
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public IInstanceMethodPlug Callback{instanceGenericTypes}(Action{instanceGenericTypes} replacement)
    {{
        if (Original.ReturnType == typeof(void))
        {{
            MethodHandler = new VoidInstanceMethodHandler{instanceGenericTypes}(Key, replacement);
        }}
        else
        {{
            MethodHandler = new InstanceMethodHandler<{instanceGenericTypes.Trim('<', '>')}, object?>(Key, (TInstance instance{(numParameters > 0 ? ", " : "")}{string.Join(", ", GetPrefixParameters(numParameters).Skip(1))}) =>
            {{
                replacement(instance{(numParameters > 0 ? ", " : "")}{string.Join(", ", GetCallbackParameters(numParameters))});
                return GetDefaultValue(Original.ReturnType);
            }});
        }}
        return this;
    }}");
                }

                staticInterfaceBuilder.AppendLine("    /// <summary>");
                staticInterfaceBuilder.AppendLine("    /// Add a callback that will be invoked instead of the original static method and returns a value.");
                staticInterfaceBuilder.AppendLine("    /// </summary>");
                for (int i = 0; i < numParameters; i++)
                {
                    staticInterfaceBuilder.AppendLine($"    /// <typeparam name=\"T{i + 1}\"></typeparam>");

                }
                staticInterfaceBuilder.AppendLine("    /// <param name=\"replacement\">The replacement delegate to invoke.</param>");
                staticInterfaceBuilder.AppendLine("    /// <returns>The original method plug.</returns>");

                staticInterfaceBuilder.AppendLine(@$"    IMethodPlug<TReturn> Returns{genericTypes}(Func{genericTypesWithReturn} replacement);");

                staticMethodPlugBuilder.AppendLine($@"
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public IMethodPlug<TReturn> Returns{genericTypes}(Func{genericTypesWithReturn} replacement)
    {{
        MethodHandler = new MethodHandler{genericTypesWithReturn}(Key, replacement);
        return this;
    }}");
                if (numParameters <= 14)
                {
                    instanceInterfaceBuilder.AppendLine("    /// <summary>");
                    instanceInterfaceBuilder.AppendLine("    /// Add a callback that will be invoked instead of the original instance method and returns a value.");
                    instanceInterfaceBuilder.AppendLine("    /// </summary>");
                    instanceInterfaceBuilder.AppendLine($"    /// <typeparam name=\"TInstance\">The declaring type of the method</typeparam>");

                    for (int i = 0; i < numParameters; i++)
                    {
                        instanceInterfaceBuilder.AppendLine($"    /// <typeparam name=\"T{i + 1}\"></typeparam>");

                    }
                    instanceInterfaceBuilder.AppendLine("    /// <param name=\"replacement\">The replacement delegate to invoke.</param>");
                    instanceInterfaceBuilder.AppendLine("    /// <returns>The original method plug.</returns>");

                    instanceInterfaceBuilder.AppendLine(@$"    IInstanceMethodPlug<TReturn> Returns{instanceGenericTypes}(Func{instanceGenericTypesWithReturn} replacement);");


                    instanceMethodPlugBuilder.AppendLine($@"
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public IInstanceMethodPlug<TReturn> Returns{instanceGenericTypes}(Func{instanceGenericTypesWithReturn} replacement)
    {{
        MethodHandler = new InstanceMethodHandler{instanceGenericTypesWithReturn}(Key, replacement);
        return this;
    }}");
                }
            }


            voidStaticInterfaceBuilder.AppendLine(@$"
}}");
            voidInstanceInterfaceBuilder.AppendLine($@"
}}");
            voidStaticMethodPlugBuilder.AppendLine(@$"
}}");
            voidInstanceMethodPlugBuilder.AppendLine(@$"
}}");
            staticInterfaceBuilder.AppendLine(@$"
}}");
            instanceInterfaceBuilder.AppendLine(@$"
}}");
            staticMethodPlugBuilder.AppendLine(@$"
}}");
            instanceMethodPlugBuilder.AppendLine(@$"
}}");

            context.AddSource("MethodHandler.g.cs", handlerBuilder.ToString());
            context.AddSource("IMethodPlug.g.cs", voidStaticInterfaceBuilder.ToString());
            context.AddSource("IInstanceMethodPlug.g.cs", voidInstanceInterfaceBuilder.ToString());
            context.AddSource("StaticMethodPlug.g.cs", voidStaticMethodPlugBuilder.ToString());
            context.AddSource("InstanceMethodPlug.g.cs", voidInstanceMethodPlugBuilder.ToString());
            context.AddSource("IMethodPlug{T}.g.cs", staticInterfaceBuilder.ToString());
            context.AddSource("IInstanceMethodPlug{T}.g.cs", instanceInterfaceBuilder.ToString());
            context.AddSource("MethodPlug{T}.g.cs", staticMethodPlugBuilder.ToString());
            context.AddSource("InstanceMethodPlug{T}.g.cs", instanceMethodPlugBuilder.ToString());


            static IEnumerable<string> GetPrefixParameters(int numParamters)
            {
                yield return "MethodBase __originalMethod";
                for (int i = 0; i < numParamters; i++)
                {
                    yield return $"T{i + 1} __{i}";
                }
            }

            static IEnumerable<string> GetCallbackParameters(int numParamters)
            {
                for (int i = 0; i < numParamters; i++)
                {
                    yield return $"__{i}";
                }
            }

        }
    }
}
