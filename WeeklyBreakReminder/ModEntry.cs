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
                    Game1.addHUDMessage(new HUDMessage(Messages.GenerateStartupMessage(interval, today.DayOfWeek.ToString()), 2));
                }
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
                return; // Return if the Generic Mod Config Menu API can't be found, so we don't create an error
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