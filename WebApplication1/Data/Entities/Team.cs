using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


public class Team
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [StringLength(500)]
    public string Description { get; set; }

    public DateTime DateCreated { get; set; }

    public virtual ICollection<Player> Players { get; set; }

    public Team()
    {
        Players = new HashSet<Player>();
        DateCreated = DateTime.UtcNow;
    }
}