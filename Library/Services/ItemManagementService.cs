using Library.Classes;

namespace Library.Services;

/// <summary>
///     Service responsible for item and equipment management logic
/// </summary>
public static class ItemManagementService
{
    // Lookup table for upgrade capacities
    private static readonly Dictionary<int, int> ArrowCapacities = new()
    {
        { 0, 30 },
        { 1, 35 },
        { 2, 40 },
        { 3, 45 },
        { 4, 50 },
        { 5, 55 },
        { 6, 60 },
        { 7, 70 }
    };

    private static readonly Dictionary<int, int> BombCapacities = new()
    {
        { 0, 10 },
        { 1, 15 },
        { 2, 20 },
        { 3, 25 },
        { 4, 30 },
        { 5, 35 },
        { 6, 40 },
        { 7, 50 }
    };

    // Bottle addresses for iteration
    private static readonly int[] BottleAddresses =
    [
        Bottle1ContentsAddress,
        Bottle2ContentsAddress,
        Bottle3ContentsAddress,
        Bottle4ContentsAddress
    ];

    private static readonly Dictionary<string, AbilityConfig> AbilityConfigs = new()
    {
        ["PegasusBoots"] = new(PegasusBootsAddress, 0x4, 0xFB),
        ["ZorasFlippers"] = new(ZorasFlippersAddress, 0x2, 0xFD)
    };

    #region Magic Upgrades

    /// <summary>
    ///     Cycles through magic upgrade levels
    /// </summary>
    public static int CycleMagicUpgrade(int currentUpgrade) => currentUpgrade switch
    {
        0x1 => 0x2, // Half -> Quarter
        0x2 => 0x0, // Quarter -> Normal
        _ => 0x1 // Normal -> Half
    };

    #endregion

    // Ability flag configurations
    private record AbilityConfig(int Address, byte EnableBit, byte DisableMask);

    #region Capacity Calculations

    /// <summary>
    ///     Gets the maximum capacity for a given upgrade type and level
    /// </summary>
    private static int GetMaxCapacity(Dictionary<int, int> capacityMap, int upgradeLevel) =>
        capacityMap.TryGetValue(upgradeLevel, out var capacity)
            ? capacity
            : capacityMap[0];

    /// <summary>
    ///     Gets the maximum arrow count based on upgrade level
    /// </summary>
    public static int GetMaxArrows(int upgradeLevel) => GetMaxCapacity(ArrowCapacities, upgradeLevel);

    /// <summary>
    ///     Gets the maximum bomb count based on upgrade level
    /// </summary>
    public static int GetMaxBombs(int upgradeLevel) => GetMaxCapacity(BombCapacities, upgradeLevel);

    #endregion

    #region Bottle Management

    /// <summary>
    ///     Finds the first non-empty bottle contents
    /// </summary>
    public static int GetInventoryBottleContents(Link player)
    {
        foreach (var address in BottleAddresses)
        {
            var contents = player.GetItemEquipment(address);
            if (contents > 0)
            {
                return contents;
            }
        }

        return 0;
    }

    /// <summary>
    ///     Sets the selected bottle for the player based on which bottles have contents
    /// </summary>
    public static void UpdateSelectedBottle(Link player)
    {
        for (var i = 0; i < BottleAddresses.Length; i++)
        {
            if (player.GetItemEquipment(BottleAddresses[i]) > 0)
            {
                player.SetSelectedBottle(i + 1);
                return;
            }
        }

        player.SetSelectedBottle(0);
    }

    /// <summary>
    ///     Normalizes bottle content value (game uses 1 for empty in some contexts, 9 in others)
    /// </summary>
    public static int NormalizeBottleContent(int contentValue) => contentValue switch
    {
        1 => 9,
        < 1 => 1,
        _ => contentValue
    };

    #endregion

    #region Item Toggles

    /// <summary>
    ///     Toggles an item on/off
    /// </summary>
    public static void ToggleItem(Link player, int address, int enabledValue)
    {
        var newValue = (byte)(player.GetItemEquipment(address) == enabledValue
            ? 0x0
            : enabledValue);
        player.SetHasItemEquipment(address, newValue);
    }

    /// <summary>
    ///     Generic method to toggle items that require ability flag updates
    /// </summary>
    private static void ToggleAbilityItem(Link player, AbilityConfig config)
    {
        var flags = player.GetAbilityFlags();
        var isEnabled = player.GetItemEquipment(config.Address) == 1;

        if (isEnabled)
        {
            flags &= config.DisableMask;
            player.SetHasItemEquipment(config.Address, 0x0);
        }
        else
        {
            flags |= config.EnableBit;
            player.SetHasItemEquipment(config.Address, 0x1);
        }

        player.SetHasItemEquipment(AbilityFlagsAddress, flags);
    }

    /// <summary>
    ///     Toggles Pegasus Boots (requires ability flag update)
    /// </summary>
    public static void TogglePegasusBoots(Link player) =>
        ToggleAbilityItem(player, AbilityConfigs["PegasusBoots"]);

    /// <summary>
    ///     Toggles Zora's Flippers (requires ability flag update)
    /// </summary>
    public static void ToggleZorasFlippers(Link player) =>
        ToggleAbilityItem(player, AbilityConfigs["ZorasFlippers"]);

    #endregion

    #region Heart Piece Management

    private const int MaxHeartContainers = 152;
    private const int MinHeartContainers = 8;
    private const int MaxHeartPieces = 24;
    private const int HeartPiecesPerContainer = 4;
    private const int HeartContainerValue = 8;

    /// <summary>
    ///     Updates heart pieces and heart containers when incrementing
    /// </summary>
    public static (int heartContainers, int heartPieces) IncrementHeartPiece(
        int currentContainers,
        int currentPieces)
    {
        if (currentContainers >= MaxHeartContainers || currentPieces >= MaxHeartPieces)
        {
            return (currentContainers, currentPieces);
        }

        var newPieces = currentPieces + 1;
        var newContainers = currentContainers;

        if (newPieces % HeartPiecesPerContainer == 0)
        {
            newContainers += HeartContainerValue;
        }

        return (newContainers, newPieces);
    }

    /// <summary>
    ///     Updates heart pieces and heart containers when decrementing
    /// </summary>
    public static (int heartContainers, int heartPieces) DecrementHeartPiece(
        int currentContainers,
        int currentPieces)
    {
        if (currentContainers <= MinHeartContainers || currentPieces <= 0)
        {
            return (currentContainers, currentPieces);
        }

        var newPieces = currentPieces - 1;
        var newContainers = currentContainers;

        if (currentPieces % HeartPiecesPerContainer == 0)
        {
            newContainers -= HeartContainerValue;
        }

        return (newContainers, newPieces);
    }

    #endregion
}
