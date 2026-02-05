using Library.Classes;

namespace Library.Services;

/// <summary>
///     Service responsible for managing game state and save slots
/// </summary>
public class GameStateService
{
    public SRAM? CurrentSram { get; private set; }

    public int CurrentSlot { get; private set; } = 1;

    public bool HasLoadedFile => CurrentSram is not null;

    public event EventHandler<SaveSlotChangedEventArgs>? SaveSlotChanged;
    public event EventHandler<SramLoadedEventArgs>? SramLoaded;

    /// <summary>
    ///     Loads SRAM data and initializes save slots
    /// </summary>
    public SaveRegion LoadSram(byte[] data, TextCharacterData textCharacterData)
    {
        CurrentSram = new SRAM(data, textCharacterData);

        // Determine the overall region of the .srm
        var saveRegion = DetermineSaveRegion();

        SramLoaded?.Invoke(this, new SramLoadedEventArgs(saveRegion));

        return saveRegion;
    }

    /// <summary>
    ///     Changes the active save slot
    /// </summary>
    public void SetCurrentSlot(int slot)
    {
        if (slot < 1 || slot > 3)
        {
            throw new ArgumentOutOfRangeException(nameof(slot), "Slot must be between 1 and 3");
        }

        CurrentSlot = slot;
        SaveSlotChanged?.Invoke(this, new SaveSlotChangedEventArgs(slot, GetCurrentSaveSlot()));
    }

    /// <summary>
    ///     Gets the currently selected save slot
    /// </summary>
    public SaveSlot GetCurrentSaveSlot()
    {
        if (CurrentSram is null)
        {
            throw new InvalidOperationException("No SRAM file loaded");
        }

        return SRAM.GetSaveSlot(CurrentSlot);
    }

    /// <summary>
    ///     Gets a specific save slot
    /// </summary>
    public SaveSlot GetSaveSlot(int slot)
    {
        if (CurrentSram is null)
        {
            throw new InvalidOperationException("No SRAM file loaded");
        }

        return SRAM.GetSaveSlot(slot);
    }

    /// <summary>
    ///     Gets the player for the current save slot
    /// </summary>
    public Link GetCurrentPlayer() => GetCurrentSaveSlot().GetPlayer();

    /// <summary>
    ///     Merges all save data for writing
    /// </summary>
    public byte[] MergeSaveData()
    {
        if (CurrentSram is null)
        {
            throw new InvalidOperationException("No SRAM file loaded");
        }

        return CurrentSram.MergeSaveData();
    }

    /// <summary>
    ///     Creates a new save file in the specified slot
    /// </summary>
    public SaveSlot CreateFile(int slot, SaveRegion region, TextCharacterData textCharacterData)
    {
        if (CurrentSram is null)
        {
            throw new InvalidOperationException("No SRAM file loaded");
        }

        var saveSlot = SRAM.CreateFile(slot, region, textCharacterData);
        SaveSlotChanged?.Invoke(this, new SaveSlotChangedEventArgs(slot, saveSlot));
        return saveSlot;
    }

    /// <summary>
    ///     Copies the current save file
    /// </summary>
    public string CopyFile(int slot, TextCharacterData textCharacterData)
    {
        if (CurrentSram is null)
        {
            throw new InvalidOperationException("No SRAM file loaded");
        }

        return SRAM.CopyFile(slot, textCharacterData);
    }

    /// <summary>
    ///     Writes changes to the save file
    /// </summary>
    public SaveSlot WriteFile(int slot, TextCharacterData textCharacterData)
    {
        if (CurrentSram is null)
        {
            throw new InvalidOperationException("No SRAM file loaded");
        }

        return SRAM.WriteFile(slot, textCharacterData);
    }

    /// <summary>
    ///     Erases a save file
    /// </summary>
    public void EraseFile(int slot)
    {
        if (CurrentSram is null)
        {
            throw new InvalidOperationException("No SRAM file loaded");
        }

        SRAM.EraseFile(slot);
        var saveSlot = GetSaveSlot(slot);
        saveSlot.SetIsValid(false);
    }

    /// <summary>
    ///     Determines the save region from all slots
    /// </summary>
    private SaveRegion DetermineSaveRegion()
    {
        var slot1 = SRAM.GetSaveSlot(1);
        var slot2 = SRAM.GetSaveSlot(2);
        var slot3 = SRAM.GetSaveSlot(3);

        if (slot1.GetRegion() == SaveRegion.JPN ||
            slot2.GetRegion() == SaveRegion.JPN ||
            slot3.GetRegion() == SaveRegion.JPN)
        {
            return SaveRegion.JPN;
        }

        if (slot1.GetRegion() == SaveRegion.USA ||
            slot2.GetRegion() == SaveRegion.USA ||
            slot3.GetRegion() == SaveRegion.USA)
        {
            return SaveRegion.USA;
        }

        return SaveRegion.EUR;
    }
}

public class SaveSlotChangedEventArgs(int slotNumber, SaveSlot saveSlot) : EventArgs
{
    public int SlotNumber { get; } = slotNumber;
    public SaveSlot SaveSlot { get; } = saveSlot;
}

public class SramLoadedEventArgs(SaveRegion region) : EventArgs
{
    public SaveRegion Region { get; } = region;
}
