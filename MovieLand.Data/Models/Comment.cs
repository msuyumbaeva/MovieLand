using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MovieLand.Data.Models
{
    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid MovieId { get; set; }

        public string UserId { get; set; }

        [Required]
        [StringLength(500)]
        public string Text { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual Movie Movie { get; set; }
        public virtual AppUser User { get; set; }
    }
}
