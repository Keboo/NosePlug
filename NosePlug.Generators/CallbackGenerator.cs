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
using System;

namespace NosePlug.Plugs
{{
    internal partial class MethodPlug
    {{");

            for (int numParameters = 0; numParameters <= NumberOfParameters; numParameters++)
            {
                string genericTypes = "<" + string.Join(", ", Enumerable.Range(1, numParameters).Select(x => $"T{x}")) + ">";
                if (numParameters == 0) genericTypes = "";

                handlerBuilder.AppendLine(@$"
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
                return false;
            }}
            return true;
        }}
    }}");


                if (numParameters > 0)
                {
                    interfaceBuilder.AppendLine(@$"        INasalMethodPlug Callback{genericTypes}(Action{genericTypes} callback);");

                    methodPlugBuilder.AppendLine($@"
        public INasalMethodPlug Callback{genericTypes}(Action{genericTypes} callback)
        {{
            MethodHandler = new VoidMethodHandler{genericTypes}(Key, callback);
            return this;
        }}");
                }
            }

            handlerBuilder.AppendLine("}");

            interfaceBuilder.AppendLine(@$"
    }}
}}");
            methodPlugBuilder.AppendLine(@$"
    }}
}}");

            context.AddSource("VoidMethodHandler.g.cs", handlerBuilder.ToString());
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
