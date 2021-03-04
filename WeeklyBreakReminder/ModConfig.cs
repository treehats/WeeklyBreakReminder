
namespace WeeklyBreakReminder
{
    class ModConfig
    {
        // ShowStartupNotice: Specify whether a notification should be shown on game load for what day reminders will
        //                    occur on. Messages are hidden by default, but some users may find them helpful.
        public bool ShowStartupNotice { get; set; } = false;
    }
}
