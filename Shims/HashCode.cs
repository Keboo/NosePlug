
namespace System;

internal struct HashCode
{
    public static int Combine<T1, T2>(T1 value1, T2 value2)
    {
        int result = value1?.GetHashCode() ?? 0;
        result = (result * 397) ^ (value2?.GetHashCode() ?? 0);
        return result;
    }

    public static int Combine<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4)
    {
        int result = Combine(value1, value2);
        result = (result * 397) ^ Combine(value3, value4);
        return result;
    }
}