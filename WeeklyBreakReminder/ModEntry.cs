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
        
        //TODO
        // Avoid message conflicts:
        // 1. When sending message, check if one already exists; if so, set a "waiting" bool
        // 2. Use OnSecondUpdateTicked to track waiting bool
        // 3. When waiting bool is true, check if anything is on screen with Context.IsPlayerFree
        // 4. Once player is free, send message and clear waiting bool



        private ModConfig Config;
        private MessageGenerator Messages;
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
            this.Messages = new MessageGenerator();

            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        }

        /*********
        ** Private methods
        *********/
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // Try to set up the Generic Mod Config Menu integration, if it exists
            SetupConfigMenu();
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            showStartupNotice = this.Config.ShowStartupNotice;
            interval = this.Config.DaysBetweenBreaks;
            startDay = SDate.Now().DaysSinceStart;
            doStartup = true;
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            var today = SDate.Now();
            int dayDelta = today.DaysSinceStart - startDay;
            bool isCompactMsg = this.Config.CompactMessages;
            if (doStartup)
            {
                if (showStartupNotice)
                {
                    if (interval == 1)
                    {
                        Game1.addHUDMessage(new HUDMessage($"You will be reminded to take a break every day.", 2));
                    }
                    else if (interval % 7 == 0)
                    {
                        Game1.addHUDMessage(new HUDMessage($"You will be reminded to take a break on the next {today.DayOfWeek}.", 2));
                    }
                    else
                    {
                        Game1.addHUDMessage(new HUDMessage($"You will be reminded to take a break every {interval} days.", 2));
                    }
                }
                this.Monitor.Log($"Break reminders will occur every {interval} days.", LogLevel.Debug);
                doStartup = false;
            }
            else if (dayDelta % interval == 0)
            {
                Game1.activeClickableMenu = new DialogueBox(Messages.GenerateMessage(interval, today.Day, isCompactMsg));
            }
        }

        private void SetupConfigMenu()
        {
            var api = Helper.ModRegistry.GetApi<IModConfigMenuAPI>("spacechase0.GenericModConfigMenu");

            if (api == null)
            {
                return;
            }
            api.RegisterModConfig(this.ModManifest, () => this.Config = new ModConfig(), () => this.Helper.WriteConfig(this.Config));
            api.RegisterSimpleOption(this.ModManifest, "Display Startup Message", "Display a message stating the frequency of break reminders when you start playing.",
                () => this.Config.ShowStartupNotice, (bool val) => this.Config.ShowStartupNotice = val);
            api.RegisterClampedOption(this.ModManifest, "Days Between Breaks", "How many days should pass before a break reminder occurs.",
                () => this.Config.DaysBetweenBreaks, (int val) => this.Config.DaysBetweenBreaks = val, 1, 14);
            api.RegisterSimpleOption(this.ModManifest, "Use Compact Messages", "Select to make the messages take up less screen space.",
                () => this.Config.CompactMessages, (bool val) => this.Config.CompactMessages = val);
        }
    }
}