using System;

namespace LastRaid
{
  public static class EpicsDataConst
  {
    public static readonly TimeSpan EVENT_HEADROOM = TimeSpan.FromSeconds(5);
    public const int DEFAULT_HEADSUP_MINUTES = 30;

    // rb data
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

    // component
    public const string CANCEL_BUTTON_LABEL = "Cancel";
    public const string CANCEL_BUTTON_ID = "cancelbutton";
    public const string OURS_BUTTON_LABEL = "Ours";
    public const string OURS_BUTTON_ID = "oursbutton";
    public const string ENEMIES_BUTTON_LABEL = "Enemies";
    public const string ENEMIES_BUTTON_ID = "enemiesbutton";
    public const string NODROP_BUTTON_LABEL = "No drop";
    public const string NODROP_BUTTON_ID = "nodropbutton";
    public const string DEAD_BUTTON_LABEL = "Dead";
    public const string DEAD_BUTTON_ID = "deadbutton";
    public const string SPAWNED_BUTTON_LABEL = "Spawned";
    public const string SPAWNED_BUTTON_ID = "spawnedbutton";
  }
}
