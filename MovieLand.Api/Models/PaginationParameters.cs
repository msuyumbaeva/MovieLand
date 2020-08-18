using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieLand.Api.Models
{
    public class PaginationParameters
    {
        public PaginationParameters() {
            Offset = 0;
            Limit = 10;
        }

        public int Offset { get; set; }
        public int Limit { get; set; }
    }
}
