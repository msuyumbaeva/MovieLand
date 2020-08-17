using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MovieLand.BLL.Dtos.Comment
{
    public class CommentDto
    {
        [Required]
        public Guid MovieId { get; set; }

        [Required]
        public string User { get; set; }

        [Required]
        [MaxLength(500)]
        public string Text { get; set; }

        public DateTime CreatedAt { get; set; }

        public string LocalCreatedAt {
            get {
                return String.Concat(CreatedAt.ToLongDateString(),CreatedAt.ToShortTimeString());
            }
        }
    }
}
