using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley.Menus;
using StardewValley;
using System.Collections.Generic;

namespace WeeklyBreakReminder
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private ModConfig Config;
        private bool doStartup = false;
        private bool showStartupNotice = true;
        private int startDay;
        private int interval = 7;

        /*********
        ** Public methods
        *********/
        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            
            showStartupNotice = this.Config.ShowStartupNotice;
            interval = this.Config.DaysBetweenBreaks;
            // Cap the interval at one month; nobody should set it this high, but just in case...
            if (interval > 28)
            {
                interval = 28;
            }

            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        }

        /*********
        ** Private methods
        *********/
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            startDay = SDate.Now().DaysSinceStart;
            doStartup = true;
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            var today = SDate.Now();
            int dayDelta = today.DaysSinceStart - startDay;
            if (doStartup)
            {
                if (showStartupNotice)
                {
                    if (interval % 7 == 0)
                    {
                        Game1.addHUDMessage(new HUDMessage($"You will be reminded to take a break on the next {today.DayOfWeek}.", 2));
                    }
                    else
                    {
                        Game1.addHUDMessage(new HUDMessage($"You will be reminded to take a break every {interval} days.", 2));
                    }
                }
                this.Monitor.Log($"Current day on game load is {today.DayOfWeek} {today.Day} {today.Season}. \nBreak reminders will occur every {interval} days.", LogLevel.Debug);
                doStartup = false;
            }
            else if (dayDelta % interval == 0)
            {
                Game1.activeClickableMenu = new DialogueBox(GenerateMessage(true));
            }
        }

        private String GenerateMessage(bool isSessionMessage)
        {
            String announcementMessage;
            String breakMessage = "This might be a good time to take a break.";
            if (Game1.dayOfMonth % 7 == 1 && interval % 7 == 0)
            {
                // Special message for Mondays, which are the start of the week in Stardew Valley
                announcementMessage = "It's the start of a new week!";
            }
            else if (isSessionMessage)
            {
                if (interval % 7 == 0)
                {
                    if (interval > 7)
                    {
                        announcementMessage = $"Another {GetNumName(interval / 7)} weeks have passed!";
                    }
                    else
                    {
                        announcementMessage = "Another week has passed!";
                    }
                }
                else if (interval <= 10)
                {
                    if (interval > 1)
                    {
                        announcementMessage = $"Another {GetNumName(interval)} days have passed!";
                    }
                    else
                    {
                        announcementMessage = $"Another day has passed!";
                    }
                }
                else
                {
                    announcementMessage = $"Another {interval} days have passed!";
                }
            }
            else
            {
                announcementMessage = $"It's {GetDayOfWeek(Game1.dayOfMonth % 7)} again.";
            }

            return $"^{announcementMessage} ^^{breakMessage}^";
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