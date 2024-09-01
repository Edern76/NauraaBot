namespace NauraaBot.Quartz.Utils;

public static class JobUpdateLock
{
    public static bool IsLocked { get; set; }

    static JobUpdateLock()
    {
        IsLocked = false;
    }
}