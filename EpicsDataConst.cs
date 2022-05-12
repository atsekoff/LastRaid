using System;

namespace LastRaid
{
  public static class EpicsDataConst
  {
    public static readonly TimeSpan EVENT_HEADROOM = TimeSpan.FromSeconds(5);
    public enum BossNames { QueenAnt, Core, Orfen, Zaken, Baium, Antharas, Valakas, Frintezza, TestBoss }
    public static readonly int[] DEATH_DURATIONS = { 24, 30, 30, 40, 120, 192, 264, 48, 1 };
    public static readonly int[] WINDOW_DURATIONS = { 6, 6, 6, 8, 8, 8, 0, 2, 1 };
    public static readonly string[] EPIC_THUMBNAILS = {
      "https://lineage.pmfun.com/data/img/accessory_ring_of_queen_ant_i00.png",
      "https://lineage.pmfun.com/data/img/accessory_ring_of_core_i00.png",
      "https://lineage.pmfun.com/data/img/accessory_earring_of_orfen_i00.png",
      "https://lineage.pmfun.com/data/img/accessory_earring_of_zaken_i00.png",
      "https://lineage.pmfun.com/data/img/accessory_ring_of_baium_i00.png",
      "https://lineage.pmfun.com/data/img/accessory_earring_of_antaras_i00.png",
      "https://lineage.pmfun.com/data/img/accessory_necklace_of_valakas_i00.png",
      "https://lineage.pmfun.com/data/img/accessory_necklace_of_frintessa_i00.png"};
  }
}
