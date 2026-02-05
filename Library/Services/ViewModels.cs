namespace Library.Services;

/// <summary>
///     Models for transferring data between services and UI
/// </summary>
public static class ViewModels
{
    public record PlayerStats(
        string PlayerName,
        int Rupees,
        int HeartContainers,
        int CurrentMagic,
        int MagicUpgrade,
        int HeartPieces,
        int ArrowUpgrades,
        int ArrowsHeld,
        int BombUpgrades,
        int BombsHeld);

    public record ItemState(
        int Address,
        int Value,
        bool IsEnabled);

    public record BottleState(
        int Bottle1Contents,
        int Bottle2Contents,
        int Bottle3Contents,
        int Bottle4Contents,
        int InventoryBottleDisplay);

    public record CollectibleState(
        byte Pendants,
        byte Crystals);

    public record SaveFileInfo(
        int SlotNumber,
        bool IsValid,
        string PlayerName,
        SaveRegion Region);

    public record GameState(
        SaveFileInfo CurrentFile,
        PlayerStats PlayerStats,
        BottleState Bottles,
        CollectibleState Collectibles,
        Dictionary<string, ItemState> Items);
}
