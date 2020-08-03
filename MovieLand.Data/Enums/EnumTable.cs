using MovieLand.Data.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MovieLand.Data.Enums
{
    public class EnumTable<TEnum>
        where TEnum : struct {
        public TEnum Id { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; }

        protected EnumTable() { }

        public EnumTable(TEnum enumType) {
            ExceptionHelpers.ThrowIfNotEnum<TEnum>();

            Id = enumType;
            Name = enumType.ToString();
        }

        public static implicit operator EnumTable<TEnum>(TEnum enumType) => new EnumTable<TEnum>(enumType);
        public static implicit operator TEnum(EnumTable<TEnum> status) => status.Id;
    }
}
