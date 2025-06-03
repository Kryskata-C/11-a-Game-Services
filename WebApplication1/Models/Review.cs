using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        public int PlayerId { get; set; }
        [ForeignKey("PlayerId")]
        public virtual Player Player { get; set; }

        [Required(ErrorMessage = "Reviewer name is required.")]
        [StringLength(100)]
        public string ReviewerName { get; set; }

        [Required(ErrorMessage = "A comment is required.")]
        [StringLength(1000, ErrorMessage = "Comment cannot be longer than 1000 characters.")]
        public string CommentText { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int StarRating { get; set; }

        public DateTime ReviewDate { get; set; }

        public Review()
        {
            ReviewDate = DateTime.UtcNow;
        }
    }
}