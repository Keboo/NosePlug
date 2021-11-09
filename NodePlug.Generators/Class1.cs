using Microsoft.CodeAnalysis;
using System;
using System.Text;

namespace NodePlug.Generators
{
    [Generator]
    public class CallbackGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {

        }

        public void Execute(GeneratorExecutionContext context)
        {
            StringBuilder sb = new();
            context.AddSource(nameof(CallbackGenerator), sb.ToString());
        }
    }
}
