using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieLand.Models
{
    public class NamedArray<T>
    {
        public string Name { get; set; }

        public T[] Items { get; set; }
    }
}
