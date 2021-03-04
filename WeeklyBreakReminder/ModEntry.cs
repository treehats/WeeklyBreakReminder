using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley.Menus;
using StardewValley;

namespace WeeklyBreakReminder
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private ModConfig Config;
        private int messageDay;
        private bool doStartup = false;
        private readonly bool relativeMode = true;
        private bool showStartupNotice = true;

        /*********
        ** Public methods
        *********/
        public override void Entry(IModHelper helper)
        {
            this.Config = base.Helper.ReadConfig<ModConfig>();
            this.showStartupNotice = this.Config.ShowStartupNotice;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        }

        /*********
        ** Private methods
        *********/
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (relativeMode)
            {
                messageDay = Game1.dayOfMonth % 7;
            }
            doStartup = true;
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            bool flag = this.doStartup;
            if (flag)
            {
                bool flag2 = this.showStartupNotice;
                if (flag2)
                {
                    Game1.addHUDMessage(new HUDMessage("You will be reminded to take a break on the next " + this.GetDayOfWeek(this.messageDay) + ".", 2));
                }
                this.Monitor.Log($"Current day on game load is {Game1.shortDayDisplayNameFromDayOfSeason(Game1.dayOfMonth)} {Game1.dayOfMonth} {Game1.currentSeason}. \nBreak reminders will occur on {this.GetDayOfWeek(this.messageDay)}s.", LogLevel.Debug);
                this.doStartup = false;
            }
            else
            {
                bool flag3 = Game1.dayOfMonth % 7 == this.messageDay && this.relativeMode;
                if (flag3)
                {
                    Game1.activeClickableMenu = new DialogueBox(this.GenerateMessage(true));
                }
            }
        }

        private String GenerateMessage(bool isSessionMessage)
        {
            string breakMessage = "This might be a good time to take a break.";
            bool flag = Game1.dayOfMonth % 7 == 1;
            string announcementMessage;
            if (flag)
            {
                // Special message for Mondays, which are the start of the week in Stardew Valley
                announcementMessage = "It's the start of a new week!";
            }
            else if (isSessionMessage)
            {
                announcementMessage = "Another week has passed!";
            }
            else
            {
                announcementMessage = "It's " + this.GetDayOfWeek(Game1.dayOfMonth % 7) + " again.";
            }
            return string.Concat(new string[]
            {
                "^",
                announcementMessage,
                " ^^",
                breakMessage,
                "^"
            });
        }

        private String GetDayOfWeek(int day)
        {
            // Helper method for cleaner getting of the day name
            String[] dayName = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            return dayName[day - 1];
        }

        private String GetNumName(int num)
        {
            String[] numeralName = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten" };
            return numeralName[num];
        }
    }
}