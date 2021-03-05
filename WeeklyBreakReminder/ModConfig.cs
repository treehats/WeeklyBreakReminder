namespace WeeklyBreakReminder
{
    class ModConfig
    {
        // ShowStartupNotice: Specify whether a notification should be shown on game load for what day reminders will
        //                    occur on. Messages are hidden by default, but some users may find them helpful.
        public bool ShowStartupNotice { get; set; } = false;

        // DaysBetweenBreaks: How many days the game should play for before giving the break message.
        //                    Defaults to 7 (one week).
        public int DaysBetweenBreaks { get; set; } = 7;

        // ShowReminders: Toggle reminders on or off. Setting this to false will disable the mod entirely.
        //                Useful for if the mod needs to be turned off without uninstalling, i.e. for streaming
        public bool ShowReminders { get; set; } = true;

        // Compact Messages: Toggle to remove extra lines from messages, so they take up less screen space.
        public bool CompactMessages { get; set; } = false;
    }
}
