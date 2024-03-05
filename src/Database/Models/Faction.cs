using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NauraaBot.Database.Models;

public class Faction
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string ID { get; set; }
    public string Name { get; set; }
}