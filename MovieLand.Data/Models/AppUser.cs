using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace MovieLand.Data.Models
{
    public class AppUser : IdentityUser
    {
        public AppUser() {
        }

        public AppUser(string userName) : base(userName) {
        }
    }
}
