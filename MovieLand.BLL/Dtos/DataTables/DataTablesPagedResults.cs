using System;
using System.Collections.Generic;
using System.Text;

namespace MovieLand.BLL.Dtos.DataTables
{
    public class DataTablesPagedResults<T>
    {
        public DataTablesPagedResults() { }

        public IEnumerable<T> Items { get; set; }
        public int TotalSize { get; set; }
    }
}
