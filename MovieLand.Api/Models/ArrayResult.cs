using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieLand.Api.Models
{
    public class ArrayResult<T>
    {
        public ArrayResult(IEnumerable<T> items) {
            Items = items;
        }

        public int Count {
            get {
                return Items.Count();
            }
        }
        public IEnumerable<T> Items { get; set; }        
    }
}
