using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace NauraaBot.Database.Models;

[Table("CardSet")]
public class CardSet
{
    public string ID { get; set; }
    public string Name { get; set; }
    
    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        CardSet s = (CardSet) obj;
        return (s.ID == ID && s.Name == Name);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ID, Name);
    }
}