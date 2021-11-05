using HarmonyLib;
using System;
using System.Reflection;

namespace NosePlug
{
    internal class Smell
    {
        private PatchProcessor Processor { get; }
        private string Id { get; }
        public MethodBase Original { get; }

        public Smell(PatchProcessor processor, string id, MethodBase original)
        {
            Processor = processor ?? throw new ArgumentNullException(nameof(processor));
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Original = original ?? throw new ArgumentNullException(nameof(original));
        }

        public Plug<TReturn> Plug<TReturn>(Func<TReturn> @return)
        {
            return new Plug<TReturn>(Processor, Id, Original, @return);
        }
    }
}
