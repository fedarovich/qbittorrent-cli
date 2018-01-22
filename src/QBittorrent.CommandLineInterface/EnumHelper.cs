using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace QBittorrent.CommandLineInterface
{
    internal class EnumHelper
    {
#if NETCOREAPP2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool TryParse(Type enumType, string value, bool ignoreCase, out object result)
        {
#if NETCOREAPP2_0
            return Enum.TryParse(enumType, value, ignoreCase, out result);
#else
            var gerenericMethod = typeof(Enum)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.Name == "TryParse" && m.IsGenericMethod)
                .Single(m => m.IsGenericMethod && m.GetParameters().Length == 3);
            var method = gerenericMethod.MakeGenericMethod(enumType);
            object[] args = { value, ignoreCase, Enum.ToObject(enumType, 0) };
            var success = (bool) method.Invoke(null, args);
            result = args[2];
            return success;
#endif
        }
    }
}
