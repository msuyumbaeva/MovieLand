using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MovieLand.BLL.Dtos.User
{
    public class UserDto
    {
        public UserDto() {
            Roles = new List<string>();
        }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        public ICollection<string> Roles { get; set; }
    }
}
