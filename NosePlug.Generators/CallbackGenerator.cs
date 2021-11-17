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
            StringBuilder interfaceBuilder = new();
            StringBuilder methodPlugBuilder = new();

            handlerBuilder.AppendLine(@"
#nullable enable
using System;
using System.Reflection;

namespace NosePlug.Plugs
{");
            interfaceBuilder.AppendLine(@$"
using System;

namespace NosePlug
{{
    public partial interface INasalMethodPlug
    {{");

            methodPlugBuilder.AppendLine($@"
#nullable enable
using System;

namespace NosePlug.Plugs
{{
    internal partial class MethodPlug
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


                interfaceBuilder.AppendLine(@$"        INasalMethodPlug ReplaceWith{genericTypes}(Action{genericTypes} replacement);");

                methodPlugBuilder.AppendLine($@"
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public INasalMethodPlug ReplaceWith{genericTypes}(Action{genericTypes} replacement)
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

                interfaceBuilder.AppendLine(@$"        INasalMethodPlug Returns{genericTypesWithReturn}(Func{genericTypesWithReturn} replacement);");
                
                methodPlugBuilder.AppendLine($@"
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public INasalMethodPlug Returns{genericTypesWithReturn}(Func{genericTypesWithReturn} replacement)
        {{
            MethodHandler = new MethodHandler{genericTypesWithReturn}(Key, replacement);
            return this;
        }}");
            }

            handlerBuilder.AppendLine("}");

            interfaceBuilder.AppendLine(@$"
    }}
}}");
            methodPlugBuilder.AppendLine(@$"
    }}
}}");

            context.AddSource("MethodHandler.g.cs", handlerBuilder.ToString());
            context.AddSource("INasalMethodPlug.g.cs", interfaceBuilder.ToString());
            context.AddSource("MethodPlug.g.cs", methodPlugBuilder.ToString());


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
