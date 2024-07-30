using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NauraaBot.Database.Models;

[Table("CardType")]
[Index(nameof(ID), IsUnique = true)]
public class CardType
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

        CardType t = (CardType) obj;
        return (t.ID == ID && t.Name == Name);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ID, Name);
    }
}