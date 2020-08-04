using Newtonsoft.Json;
using System.Collections.Generic;

namespace MovieLand.Models
{
    public class DataTablesResult<T>
    {
        public DataTablesResult() { }

        [JsonProperty(PropertyName = "draw")]
        public int Draw { get; set; }
        [JsonProperty(PropertyName = "recordsTotal")]
        public int RecordsTotal { get; set; }
        [JsonProperty(PropertyName = "recordsFiltered")]
        public int RecordsFiltered { get; set; }
        [JsonProperty(PropertyName = "data")]
        public IEnumerable<T> Data { get; set; }
    }
}
