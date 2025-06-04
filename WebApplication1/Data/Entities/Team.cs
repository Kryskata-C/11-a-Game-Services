// File: WebApplication1/Data/Entities/Team.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Models;

namespace WebApplication1.Data.Entities
{
    public class Team
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PricePerHour { get; set; }

        [StringLength(255)]
        public string? ImageUrl { get; set; }

        [Range(0, 5)]
        public double Rating { get; set; }

        public DateTime DateCreated { get; set; }

        public virtual ICollection<Player> Players { get; set; } = new List<Player>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}