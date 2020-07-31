using System;
using System.Collections.Generic;
using System.Text;

namespace MovieLand.BLL.Dtos
{
    public class Page
    {
        public Page(int number, int size) {
            Number = number;
            Size = size;
        }

        public int Number { get; set; }
        public int Size { get; set; }
    }
}
