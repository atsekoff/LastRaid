using System;

namespace LastRaid
{
  public static class Consts
  {
    public static readonly TimeSpan EVENT_HEADROOM = TimeSpan.FromSeconds(5);
    public const int DEFAULT_HEADSUP_MINUTES = 30;

    // IMPORTANT the enum int values are used to index the arrays, ensure they are ordered correctly
    // rb data
    public enum BossNames { QueenAnt, Core, Orfen, Zaken, Baium, Antharas, Valakas, Frintezza, Barakiel, LilithAnakim, Beleth }
    public static readonly int[] DEATH_DURATIONS = { 24, 36, 36, 48, 124, 196, 268, 48, 18, 18, 268 };
    public static readonly int[] WINDOW_DURATIONS = { 4, 4, 4, 4, 4, 4, 4, 4, 9, 9, 4 };
    public static readonly string[] EPIC_THUMBNAILS =
    {
      "https://lineage.pmfun.com/data/img/accessory_ring_of_queen_ant_i00.png",
      "https://lineage.pmfun.com/data/img/accessory_ring_of_core_i00.png",
      "https://lineage.pmfun.com/data/img/accessory_earring_of_orfen_i00.png",
      "https://lineage.pmfun.com/data/img/accessory_earring_of_zaken_i00.png",
      "https://lineage.pmfun.com/data/img/accessory_ring_of_baium_i00.png",
      "https://lineage.pmfun.com/data/img/accessory_earring_of_antaras_i00.png",
      "https://lineage.pmfun.com/data/img/accessory_necklace_of_valakas_i00.png",
      "https://lineage.pmfun.com/data/img/accessory_necklace_of_frintessa_i00.png",
      "https://lineage.pmfun.com/data/img/accessory_noblesse_tiara_i00_0.png",
      "https://lineage.pmfun.com/data/img/etc_wind_rune_i00.png",
      "https://lineage.pmfun.com/data/img/accessary_dynasty_ring_i00.png"
    };

    // castle data
    public enum Castle { Gludio, Dion, Giran, Oren, Aden, Goddard, Rune, Innadril, Shuttgart }
    public const int SIEGE_DURATION = 2; // 2 hours

    // buttons
    public const string BUTTON_LABEL_CANCEL = "Cancel";
    public const string BUTTON_ID_CANCEL = "cancelbutton";

    public const string BUTTON_LABEL_OURS = "Ours";
    public const string BUTTON_ID_OURS = "oursbutton";

    public const string BUTTON_LABEL_ENEMIES = "Enemies";
    public const string BUTTON_ID_ENEMIES = "enemiesbutton";

    public const string BUTTON_LABEL_NO_DROP = "No drop";
    public const string BUTTON_ID_NO_DROP = "nodropbutton";

    public const string BUTTON_LABEL_CONFIRM = "Confirm";
    public const string BUTTON_ID_CONFIRM = "confirmbutton";

    public const string BUTTON_LABEL_OVERRIDE = "Override";
    public const string BUTTON_ID_OVERRIDE = "overridebutton";

    // tod strings
    public const string COMMAND_TOD_NAME = "tod";
    public const string COMMAND_TOD_DESCRIPTION = "Commands for creating RB respawn window reminders.";
    public const string COMMAND_RELATIVE_NAME = "relative";
    public const string COMMAND_RELATIVE_DESCRIPTION = "How long ago it died as HH:MM. Max 24 hours ago!";
    public const string COMMAND_EXACT_NAME = "exact";
    public const string COMMAND_EXACT_DESCRIPTION = "Exact date and time of death.";

    public const string PARAM_RELATIVE_TIME_NAME = "relative-time";
    public const string PARAM_RELATIVE_TIME_DESCRIPTION = "How long since boss death in HH:MM (e.g. 00:25 -> 25 min ago)";
    public const string PARAM_HEADS_UP_NAME = "Heads-up-minutes";
    public const string PARAM_HEADS_UP_DESCRIPTION = "Get notified ahead of the start time, in minutes. Default is 30.";
    public const string PARAM_LAST_TOD_NAME = "Last-known-time-of-death";
    public const string PARAM_LAST_TOD_DESCRIPTION = "[DD/MM/YYYY HH:MM] In your local time.";
    public const string PARAM_USER_TIME_NAME = "Local-time";
    public const string PARAM_USER_TIME_DESCRIPTION = "[DD/MM/YYYY HH:MM] Your current local time. (PC clock)";

    // siege strings
    public const string COMMAND_SIEGE_NAME = "siege";
    public const string COMMAND_SIEGE_DESCRIPTION = "Command for siege time reminders";

    public const string PARAM_SIEGE_START_NAME = "siege-time";
    public const string PARAM_SIEGE_START_DESCRIPTION = "[DD/MM/YYYY HH:MM]";
    public const string PARAM_EXTRA_INFO_NAME = "extra-info";
    public const string PARAM_EXTRA_INFO_DESCRIPTION = "Information regarding the siege.";
  }
}
