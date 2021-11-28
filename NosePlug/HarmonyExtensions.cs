using HarmonyLib;
using System;
using System.Reflection;
using System.Text;

namespace NosePlug;

internal static class HarmonyExtensions
{
    public static string FullDescription(this PropertyInfo member)
    {
        if ((object)member == null)
        {
            return "null";
        }

        Type returnedType = member.PropertyType;
        StringBuilder stringBuilder = new StringBuilder();
        if (member.GetMethod?.IsStatic == true ||
            member.SetMethod?.IsStatic == true)
        {
            stringBuilder.Append("static ");
        }

        if (member.GetMethod?.IsAbstract == true ||
            member.SetMethod?.IsAbstract == true)
        {
            stringBuilder.Append("abstract ");
        }

        if (member.GetMethod?.IsVirtual == true ||
            member.SetMethod?.IsVirtual == true)
        {
            stringBuilder.Append("virtual ");
        }

        stringBuilder.Append(returnedType.FullDescription() + " ");
        if (member.DeclaringType is not null)
        {
            stringBuilder.Append(member.DeclaringType.FullDescription() + "::");
        }

        stringBuilder.Append(member.PropertyType.FullDescription() + " ");

        stringBuilder.Append(member.Name);
        return stringBuilder.ToString();
    }
}
