using System;
using System.Collections.Generic;
using System.Text;

namespace MovieLand.BLL.Dtos.DataTables
{
    public class MovieDataTablesParameters : DataTablesParameters
    {
        public Guid? Genre { get; set; }
        public Guid? Country { get; set; }
        public Guid? Artist { get; set; }
    }
}
