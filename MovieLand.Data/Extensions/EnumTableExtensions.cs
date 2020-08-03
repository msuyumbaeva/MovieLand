using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MovieLand.Data.Extensions
{
    public static class EnumTableExtensions
    {
        public static void SeedEnumValues<T, TEnum>(this ModelBuilder mb, Func<TEnum, T> converter) where T : class => 
            Enum.GetValues(typeof(TEnum))
                           .Cast<object>()
                           .Select(value => converter((TEnum)value))
                           .ToList()
                           .ForEach(instance => mb.Entity<T>()
                           .HasData(instance));
    }
}
