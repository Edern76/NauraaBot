using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NauraaBot.Database.Models;

[Table("Rarity")]
[Index(nameof(ID), IsUnique = true)]
public class Rarity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string ID { get; set; }
    public string Name { get; set; }
    public string? Short { get; set; }
}