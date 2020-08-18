using MovieLand.Api.HyperMedia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieLand.Api.Models
{
    public class ResultSet<T> : ISupportsHyperMedia
    {
        public ResultSet(IEnumerable<T> items) {
            Items = items;
        }

        public ResultSet(IEnumerable<T> items, ResultSetMethadata methadata) {
            Items = items;
            Methadata = methadata;
        }

        public ResultSetMethadata Methadata { get; set; }

        public int Count {
            get {
                return Items.Count();
            }
        }

        public IEnumerable<T> Items { get; set; }
        public List<HyperMediaLink> Links { get; set; } = new List<HyperMediaLink>();
    }

    public class ResultSetMethadata
    {
        public ResultSetMethadata(int count, int limit, int offset) {
            Count = count;
            Limit = limit;
            Offset = offset;
        }

        public int Count { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
    }
}
