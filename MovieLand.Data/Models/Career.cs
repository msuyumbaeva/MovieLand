using MovieLand.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MovieLand.Data.Models
{
    public class Career : EnumTable<CareerEnum>
    {      
        public Career(CareerEnum @enum) : base(@enum) {
        }

        public Career() : base() { }
    }
}
