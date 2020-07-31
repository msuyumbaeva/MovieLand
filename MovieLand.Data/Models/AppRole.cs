using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MovieLand.Data.Models
{
    public class AppRole : IdentityRole
    {
        public AppRole() {
        }

        public AppRole(string roleName) : base(roleName) {
        }
    }
}
