using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            StringBuilder voidInterfaceBuilder = new();
            StringBuilder voidMethodPlugBuilder = new();
            StringBuilder methodPlugBuilder = new();
            StringBuilder interfaceBuilder = new();

            handlerBuilder.AppendLine(@"
#nullable enable
using System;
using System.Reflection;

namespace NosePlug.Plugs
{");
            voidInterfaceBuilder.AppendLine(@$"
using System;

namespace NosePlug
{{
    partial interface IMethodPlug
    {{");

            voidMethodPlugBuilder.AppendLine($@"
#nullable enable
using System;

namespace NosePlug.Plugs
{{
    partial class MethodPlug
    {{");

            interfaceBuilder.AppendLine(@$"
using System;

namespace NosePlug
{{
    partial interface IMethodPlug<TReturn>
    {{");

            methodPlugBuilder.AppendLine($@"
#nullable enable
using System;

namespace NosePlug.Plugs
{{
    partial class MethodPlug<TReturn>
    {{");

            for (int numParameters = 0; numParameters <= NumberOfParameters; numParameters++)
            {
                string genericTypes;
                string genericTypesWithReturn;
                if (numParameters == 0)
                {
                    genericTypes = "";
                    genericTypesWithReturn = "<TReturn>";
                }
                else
                {
                    genericTypes = "<" + string.Join(", ", Enumerable.Range(1, numParameters).Select(x => $"T{x}")) + ">";
                    genericTypesWithReturn = "<" + string.Join(", ", Enumerable.Range(1, numParameters).Select(x => $"T{x}")) + ", TReturn>";
                }

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


                voidInterfaceBuilder.AppendLine("        /// <summary>");
                voidInterfaceBuilder.AppendLine("        /// Add a callback that will be invoked instead of the original static method.");
                voidInterfaceBuilder.AppendLine("        /// </summary>");
                for(int i = 0; i < numParameters; i++)
                {
                    voidInterfaceBuilder.AppendLine($"        /// <typeparam name=\"T{i + 1}\"></typeparam>");

                }
                voidInterfaceBuilder.AppendLine("        /// <param name=\"replacement\">The replacement delegate to invoke.</param>");
                voidInterfaceBuilder.AppendLine("        /// <returns>The original method plug.</returns>");
                voidInterfaceBuilder.AppendLine(@$"        IMethodPlug Callback{genericTypes}(Action{genericTypes} replacement);");

                voidMethodPlugBuilder.AppendLine($@"
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
                interfaceBuilder.AppendLine("        /// <summary>");
                interfaceBuilder.AppendLine("        /// Add a callback that will be invoked instead of the original static method and returns a value.");
                interfaceBuilder.AppendLine("        /// </summary>");
                for (int i = 0; i < numParameters; i++)
                {
                    interfaceBuilder.AppendLine($"        /// <typeparam name=\"T{i + 1}\"></typeparam>");

                }
                interfaceBuilder.AppendLine("        /// <param name=\"replacement\">The replacement delegate to invoke.</param>");
                interfaceBuilder.AppendLine("        /// <returns>The original method plug.</returns>");

                interfaceBuilder.AppendLine(@$"        IMethodPlug<TReturn> Returns{genericTypes}(Func{genericTypesWithReturn} replacement);");
                
                methodPlugBuilder.AppendLine($@"
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public IMethodPlug<TReturn> Returns{genericTypes}(Func{genericTypesWithReturn} replacement)
        {{
            MethodHandler = new MethodHandler{genericTypesWithReturn}(Key, replacement);
            return this;
        }}");
            }

            handlerBuilder.AppendLine("}");

            voidInterfaceBuilder.AppendLine(@$"
    }}
}}");
            voidMethodPlugBuilder.AppendLine(@$"
    }}
}}");
            interfaceBuilder.AppendLine(@$"
    }}
}}");
            methodPlugBuilder.AppendLine(@$"
    }}
}}");

            context.AddSource("MethodHandler.g.cs", handlerBuilder.ToString());
            context.AddSource("IMethodPlug.g.cs", voidInterfaceBuilder.ToString());
            context.AddSource("MethodPlug.g.cs", voidMethodPlugBuilder.ToString());
            context.AddSource("IMethodPlug{T}.g.cs", interfaceBuilder.ToString());
            context.AddSource("MethodPlug{T}.g.cs", methodPlugBuilder.ToString());


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
