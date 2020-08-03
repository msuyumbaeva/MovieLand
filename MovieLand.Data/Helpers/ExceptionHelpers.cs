using System;
using System.Collections.Generic;
using System.Text;

namespace MovieLand.Data.Helpers
{
    static class ExceptionHelpers
    {
        public static void ThrowIfNotEnum<TEnum>()
            where TEnum : struct {
            if (!typeof(TEnum).IsEnum) {
                throw new Exception($"Invalid generic method argument of type {typeof(TEnum)}");
            }
        }
    }
}
