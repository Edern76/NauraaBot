using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NauraaBot.Database.Models;

[Table("Faction")]
[Index(nameof(ID), IsUnique = true)]
public class Faction
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string ID { get; set; }
    public string Name { get; set; }
    
    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        Faction f = (Faction) obj;
        return (f.ID == ID && f.Name == Name);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ID, Name);
    }
}