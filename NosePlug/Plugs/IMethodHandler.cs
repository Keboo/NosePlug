using HarmonyLib;
using System;

namespace NosePlug
{
    internal interface IMethodHandler : IDisposable
    {
        void Patch(PatchProcessor processor);
    }
}
