using System;

namespace LastRaid
{
  public static class EpicsDataConst
  {
    public static readonly TimeSpan EVENT_HEADROOM = TimeSpan.FromSeconds(5);
    public const int DEFAULT_HEADSUP_MINUTES = 30;

    // rb data
    // IMPORTANT the enum int values are used to index the arrays, ensure they are ordered correctly
    public enum BossNames { QueenAnt, Core, Orfen, Zaken, Baium, Antharas, Valakas, Frintezza, Barakiel, Cabrio, Hallate, Kernon, Golkonda }
    public static readonly int[] DEATH_DURATIONS = { 24, 30, 30, 40, 120, 192, 264, 48, 12, 12, 12, 12, 12 };
    public static readonly int[] WINDOW_DURATIONS = { 6, 6, 6, 8, 8, 8, 0, 2, 24, 24, 24, 24, 24 };
    public static readonly string[] EPIC_THUMBNAILS = {
      "https://lineage.pmfun.com/data/img/accessory_ring_of_queen_ant_i00.png",
      "https://lineage.pmfun.com/data/img/accessory_ring_of_core_i00.png",
      "https://lineage.pmfun.com/data/img/accessory_earring_of_orfen_i00.png",
      "https://lineage.pmfun.com/data/img/accessory_earring_of_zaken_i00.png",
      "https://lineage.pmfun.com/data/img/accessory_ring_of_baium_i00.png",
      "https://lineage.pmfun.com/data/img/accessory_earring_of_antaras_i00.png",
      "https://lineage.pmfun.com/data/img/accessory_necklace_of_valakas_i00.png",
      "https://lineage.pmfun.com/data/img/accessory_necklace_of_frintessa_i00.png",
      "https://lineage.pmfun.com/data/img/accessory_noblesse_tiara_i00_0.png",
      "https://lineage.pmfun.com/data/img/etc_bead_white_i00.png",
      "https://lineage.pmfun.com/data/img/etc_imperial_scepter_i00.png",
      "https://lineage.pmfun.com/data/img/etc_imperial_scepter_i02.png",
      "https://lineage.pmfun.com/data/img/etc_imperial_scepter_i01.png" };

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

    // commands
    public const string COMMAND_NAME_TOD = "tod";
    public const string COMMAND_DESCRIPTION_TOD = "Commands for creating RB respawn window reminders.";
    public const string COMMAND_NAME_RELATIVE = "relative";
    public const string COMMAND_DESCRIPTION_RELATIVE = "How long ago it died as HH:MM. Max 24 hours ago!";
    public const string COMMAND_NAME_EXACT = "exact";
    public const string COMMAND_DESCRIPTION_EXACT = "Exact date and time of death.";

    public const string PARAM_NAME_RELATIVE_TIME = "relative-time";
    public const string PARAM_DESCRIPTION_RELATIVE_TIME = "How long since boss death in HH:MM (e.g. 00:25 -> 25 min ago)";
    public const string PARAM_NAME_HEADS_UP = "Heads-up-minutes";
    public const string PARAM_DESCRIPTION_HEADS_UP = "Get notified ahead of the window start, in minutes. Default is 30.";
    public const string PARAM_NAME_LAST_TOD = "Last-known-time-of-death";
    public const string PARAM_DESCRIPTION_LAST_TOD = "[DD/MM/YYYY HH:MM] In your local time.";
    public const string PARAM_NAME_USER_TIME = "Local-time";
    public const string PARAM_DESCRIPTION_USER_TIME = "[DD/MM/YYYY HH:MM] Your current local time. (PC clock)";
  }
}
