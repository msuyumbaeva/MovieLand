using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MovieLand.ViewModels.Comment
{
    public class CommentCreateViewModel
    {
        [Required]
        public Guid MovieId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Text { get; set; }

    }
}
