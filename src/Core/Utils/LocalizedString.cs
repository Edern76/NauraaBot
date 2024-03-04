using Microsoft.EntityFrameworkCore;

namespace NauraaBot.Core.Utils;

[Owned]
public class LocalizedString
{
    public string en { get; set; }
    public string fr { get; set; }
    public string de { get; set; }
    public string es { get; set; }
    public string it { get; set; }
}