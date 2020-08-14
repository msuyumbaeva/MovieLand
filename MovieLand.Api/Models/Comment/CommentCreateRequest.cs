using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MovieLand.Api.Models.Comment
{
    public class CommentCreateRequest
    {
        [Required]
        [StringLength(500)]
        public string Text { get; set; }
    }
}
