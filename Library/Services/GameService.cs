namespace Library.Services;

/// <summary>
/// Facade service that coordinates all game-related services
/// Provides a simplified interface for the UI layer
/// </summary>
public class GameService
{
    private readonly SramFileService fileService = new();
    private readonly GameStateService gameStateService = new();

    public bool HasLoadedFile => gameStateService.HasLoadedFile;
    public string CurrentFilePath { get; private set; } = string.Empty;

    public int CurrentSlot => gameStateService.CurrentSlot;

    public event EventHandler<SaveSlotChangedEventArgs>? SaveSlotChanged
    {
        add => gameStateService.SaveSlotChanged += value;
        remove => gameStateService.SaveSlotChanged -= value;
    }

    public event EventHandler<SramLoadedEventArgs>? SramLoaded
    {
        add => gameStateService.SramLoaded += value;
        remove => gameStateService.SramLoaded -= value;
    }

    #region File Operations

    /// <summary>
    /// Opens an SRAM file
    /// </summary>
    public (bool success, string message, SaveRegion? region) OpenFile(string filePath,
        TextCharacterData textCharacterData)
    {
        var loadResult = fileService.LoadSramFile(filePath);

        if (!loadResult.Success || loadResult.Data is null)
        {
            return (false, loadResult.ErrorMessage ?? "Unknown error", null);
        }

        CurrentFilePath = filePath;
        var region = gameStateService.LoadSram(loadResult.Data, textCharacterData);

        return (true, $"Opened {filePath}", region);
    }

    /// <summary>
    /// Loads SRAM data from memory
    /// </summary>
    public (bool success, string message, SaveRegion? region) LoadSramData(byte[] data,
        TextCharacterData textCharacterData, string fileName = "")
    {
        var loadResult = fileService.LoadSramData(data);

        if (!loadResult.Success || loadResult.Data is null)
        {
            return (false, loadResult.ErrorMessage ?? "Unknown error", null);
        }

        CurrentFilePath = fileName;
        var region = gameStateService.LoadSram(loadResult.Data, textCharacterData);

        return (true, string.IsNullOrWhiteSpace(fileName)
            ? "Loaded SRAM data"
            : $"Opened {fileName}", region);
    }

    /// <summary>
    /// Saves the current SRAM file
    /// </summary>
    public (bool success, string message) SaveFile()
    {
        if (string.IsNullOrEmpty(CurrentFilePath))
        {
            return (false, "Load a file first!");
        }

        var outputData = gameStateService.MergeSaveData();
        var saveResult = fileService.SaveSramFile(CurrentFilePath, outputData);

        return saveResult.Success
            ? (true, $"Saved file at {CurrentFilePath}")
            : (false, saveResult.ErrorMessage ?? "Unknown error");
    }

    /// <summary>
    /// Returns the current SRAM data for saving elsewhere
    /// </summary>
    public byte[] GetSaveData() => gameStateService.MergeSaveData();

    #endregion

    #region Save Slot Operations

    /// <summary>
    /// Changes the active save slot
    /// </summary>
    public void SetCurrentSlot(int slot) => gameStateService.SetCurrentSlot(slot);

    /// <summary>
    /// Gets information about the current save slot
    /// </summary>
    public SaveSlot GetCurrentSaveSlot() => gameStateService.GetCurrentSaveSlot();

    /// <summary>
    /// Gets a specific save slot by number
    /// </summary>
    public SaveSlot GetSaveSlot(int slot) => gameStateService.GetSaveSlot(slot);

    /// <summary>
    /// Gets whether the current save slot is valid
    /// </summary>
    public bool IsCurrentSlotValid()
    {
        try
        {
            var slot = GetCurrentSaveSlot();
            return slot.SaveIsValid();
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Creates a new save file
    /// </summary>
    public SaveSlot CreateFile(int slot, SaveRegion region, TextCharacterData textCharacterData) =>
        gameStateService.CreateFile(slot, region, textCharacterData);

    /// <summary>
    /// Copies a save file
    /// </summary>
    public string CopyFile(int slot, TextCharacterData textCharacterData) =>
        gameStateService.CopyFile(slot, textCharacterData);

    /// <summary>
    /// Writes changes to a save file
    /// </summary>
    public SaveSlot WriteFile(int slot, TextCharacterData textCharacterData) =>
        gameStateService.WriteFile(slot, textCharacterData);

    /// <summary>
    /// Erases a save file
    /// </summary>
    public void EraseFile(int slot) => gameStateService.EraseFile(slot);

    #endregion

    #region Player Data Operations

    /// <summary>
    /// Gets the current player
    /// </summary>
    public Link GetCurrentPlayer() => gameStateService.GetCurrentPlayer();

    /// <summary>
    /// Gets comprehensive player statistics
    /// </summary>
    public ViewModels.PlayerStats GetPlayerStats()
    {
        var saveSlot = GetCurrentSaveSlot();
        var player = saveSlot.GetPlayer();

        return new ViewModels.PlayerStats(
            saveSlot.GetPlayerName(),
            player.GetRupeesValue(),
            player.GetHeartContainers(),
            player.GetCurrMagic(),
            player.GetCurrMagicUpgrade(),
            player.GetHeartPieces(),
            player.GetCurrArrowUpgrades(),
            player.GetHeldArrows(),
            player.GetCurrBombUpgrades(),
            player.GetHeldBombs()
        );
    }

    /// <summary>
    /// Updates player name
    /// </summary>
    public void SetPlayerName(string name) => GetCurrentSaveSlot().SetPlayerName(name);

    /// <summary>
    /// Updates player name (raw format)
    /// </summary>
    public void SetPlayerNameRaw(ushort[] nameData) => GetCurrentSaveSlot().SetPlayerNameRaw(nameData);

    /// <summary>
    /// Gets player name
    /// </summary>
    public string GetPlayerName() => GetCurrentSaveSlot().GetPlayerName();

    /// <summary>
    /// Sets rupee count
    /// </summary>
    public void SetRupees(int value)
    {
        var player = GetCurrentPlayer();
        var clampedValue = Math.Clamp(value, 0, 999);
        player.SetRupeesValue((ushort)clampedValue);
    }

    /// <summary>
    /// Sets heart containers
    /// </summary>
    public void SetHeartContainers(int value) => GetCurrentPlayer().SetHeartContainers(value);

    /// <summary>
    /// Sets current magic
    /// </summary>
    public void SetMagic(int value) => GetCurrentPlayer().SetMagic(value);

    /// <summary>
    /// Cycles magic upgrade level
    /// </summary>
    public int CycleMagicUpgrade()
    {
        var player = GetCurrentPlayer();
        var currentUpgrade = player.GetCurrMagicUpgrade();
        var newUpgrade = ItemManagementService.CycleMagicUpgrade(currentUpgrade);
        player.SetMagicUpgrade(newUpgrade);
        return newUpgrade;
    }

    /// <summary>
    /// Resets death counter
    /// </summary>
    public void ResetDeaths(bool showOnFileSelect) => GetCurrentSaveSlot().ResetFileDeaths(showOnFileSelect);

    #endregion

    #region Item Operations

    /// <summary>
    /// Sets an item/equipment value
    /// </summary>
    public void SetItem(int address, byte value) => GetCurrentPlayer().SetHasItemEquipment(address, value);

    /// <summary>
    /// Gets an item/equipment value
    /// </summary>
    public int GetItem(int address) => GetCurrentPlayer().GetItemEquipment(address);

    /// <summary>
    /// Toggles an item on/off
    /// </summary>
    public void ToggleItem(int address, int enabledValue) =>
        ItemManagementService.ToggleItem(GetCurrentPlayer(), address, enabledValue);

    /// <summary>
    /// Toggles Pegasus Boots
    /// </summary>
    public void TogglePegasusBoots() => ItemManagementService.TogglePegasusBoots(GetCurrentPlayer());

    /// <summary>
    /// Toggles Zora's Flippers
    /// </summary>
    public void ToggleZorasFlippers() => ItemManagementService.ToggleZorasFlippers(GetCurrentPlayer());

    /// <summary>
    /// Sets arrow count and validates against max
    /// </summary>
    public void SetArrows(int count, int upgradeLevel)
    {
        var maxArrows = ItemManagementService.GetMaxArrows(upgradeLevel);
        var clampedCount = Math.Clamp(count, 0, maxArrows);
        GetCurrentPlayer().SetHasItemEquipment(ArrowCountAddress, (byte)clampedCount);
    }

    /// <summary>
    /// Sets bomb count and validates against max
    /// </summary>
    public void SetBombs(int count, int upgradeLevel)
    {
        var maxBombs = ItemManagementService.GetMaxBombs(upgradeLevel);
        var clampedCount = Math.Clamp(count, 0, maxBombs);
        GetCurrentPlayer().SetHasItemEquipment(BombCountAddress, (byte)clampedCount);
    }

    /// <summary>
    /// Sets arrow upgrade level
    /// </summary>
    public void SetArrowUpgrades(int level) => GetCurrentPlayer().SetCurrArrowUpgrades(level);

    /// <summary>
    /// Sets bomb upgrade level
    /// </summary>
    public void SetBombUpgrades(int level) => GetCurrentPlayer().SetCurrBombUpgrades(level);

    /// <summary>
    /// Gets bottle states
    /// </summary>
    public ViewModels.BottleState GetBottleState()
    {
        var player = GetCurrentPlayer();
        return new ViewModels.BottleState(
            player.GetItemEquipment(Bottle1ContentsAddress),
            player.GetItemEquipment(Bottle2ContentsAddress),
            player.GetItemEquipment(Bottle3ContentsAddress),
            player.GetItemEquipment(Bottle4ContentsAddress),
            ItemManagementService.GetInventoryBottleContents(player)
        );
    }

    /// <summary>
    /// Sets bottle contents and updates selected bottle
    /// </summary>
    public void SetBottleContents(int bottleNumber, byte contents)
    {
        var player = GetCurrentPlayer();
        var address = bottleNumber switch
        {
            1 => Bottle1ContentsAddress,
            2 => Bottle2ContentsAddress,
            3 => Bottle3ContentsAddress,
            4 => Bottle4ContentsAddress,
            _ => throw new ArgumentOutOfRangeException(nameof(bottleNumber))
        };

        player.SetHasItemEquipment(address, contents);
        ItemManagementService.UpdateSelectedBottle(player);
    }

    /// <summary>
    /// Increments heart pieces
    /// </summary>
    public (int heartContainers, int heartPieces) IncrementHeartPiece()
    {
        var player = GetCurrentPlayer();
        var (newContainers, newPieces) = ItemManagementService.IncrementHeartPiece(
            player.GetHeartContainers(),
            player.GetHeartPieces()
        );

        if (newContainers != player.GetHeartContainers())
        {
            player.SetHeartContainers(newContainers);
        }

        player.IncrementHeartPieces();

        return (newContainers, newPieces);
    }

    /// <summary>
    /// Decrements heart pieces
    /// </summary>
    public (int heartContainers, int heartPieces) DecrementHeartPiece()
    {
        var player = GetCurrentPlayer();
        var (newContainers, newPieces) = ItemManagementService.DecrementHeartPiece(
            player.GetHeartContainers(),
            player.GetHeartPieces()
        );

        if (newContainers != player.GetHeartContainers())
        {
            player.SetHeartContainers(newContainers);
        }

        player.DecrementHeartPieces();

        return (newContainers, newPieces);
    }

    #endregion

    #region Collectibles

    /// <summary>
    /// Gets collectible state (pendants and crystals)
    /// </summary>
    public ViewModels.CollectibleState GetCollectibleState()
    {
        var saveSlot = GetCurrentSaveSlot();
        return new ViewModels.CollectibleState(
            saveSlot.GetPendants(),
            saveSlot.GetCrystals()
        );
    }

    /// <summary>
    /// Toggles a pendant
    /// </summary>
    public void TogglePendant(int pendantBit) => GetCurrentSaveSlot().TogglePendant(pendantBit);

    /// <summary>
    /// Toggles a crystal
    /// </summary>
    public void ToggleCrystal(int crystalBit) => GetCurrentSaveSlot().ToggleCrystal(crystalBit);

    #endregion

    #region Utility

    /// <summary>
    /// Gets ability flags
    /// </summary>
    public byte GetAbilityFlags() => GetCurrentPlayer().GetAbilityFlags();

    /// <summary>
    /// Gets save region
    /// </summary>
    public SaveRegion GetSaveRegion() => GetCurrentSaveSlot().GetRegion();

    #endregion
}
