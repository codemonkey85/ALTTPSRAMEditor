namespace ALTTPSRAMEditor.BlazorWasm.Pages;

// ReSharper disable once ClassNeverInstantiated.Global
public partial class Home
{
    private const long MaxSrmSize = 0x8000;

    private string StatusMessage { get; set; } = string.Empty;

    private Enums.SaveRegion? CurrentRegion { get; set; }

    private int SelectedSlot
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;

            if (!GameService.HasLoadedFile)
            {
                return;
            }

            GameService.SetCurrentSlot(value);
            LoadPlayerStats();
        }
    } = 1;

    private string PlayerName { get; set; } = string.Empty;

    private int Rupees { get; set; }

    private int HeartContainers { get; set; }

    private int CurrentMagic { get; set; }

    private int ArrowUpgrades { get; set; }

    private int ArrowsHeld { get; set; }

    private int BombUpgrades { get; set; }

    private int BombsHeld { get; set; }

    private int MagicUpgrade { get; set; }

    private int HeartPieces { get; set; }

    private ViewModels.CollectibleState? Collectibles { get; set; }

    private ViewModels.BottleState? Bottles { get; set; }

    private List<InventorySlot> InventorySlots { get; set; } = [];

    private List<InventoryStateGroup> InventoryStateGroups { get; set; } = [];

    private List<CollectibleSlot> PendantSlots { get; set; } = [];

    private List<CollectibleSlot> CrystalSlots { get; set; } = [];

    private int MaxArrows => ItemManagementService.GetMaxArrows(ArrowUpgrades);

    private int MaxBombs => ItemManagementService.GetMaxBombs(BombUpgrades);

    private int DisplayHearts => Math.Clamp(HeartContainers / 8, 0, 20);

    private static readonly IReadOnlyList<BottleOption> BottleOptions =
    [
        new BottleOption("None", (int)Enums.BottleContents.NONE),
        new BottleOption("Empty", (int)Enums.BottleContents.EMPTY),
        new BottleOption("Red Potion", (int)Enums.BottleContents.RED_POTION),
        new BottleOption("Green Potion", (int)Enums.BottleContents.GREEN_POTION),
        new BottleOption("Blue Potion", (int)Enums.BottleContents.BLUE_POTION),
        new BottleOption("Fairy", (int)Enums.BottleContents.FAERIE),
        new BottleOption("Bee", (int)Enums.BottleContents.BEE),
        new BottleOption("Good Bee", (int)Enums.BottleContents.GOOD_BEE),
        new BottleOption("Mushroom", (int)Enums.BottleContents.MUSHROOM)
    ];

    protected override void OnInitialized()
    {
        base.OnInitialized();
        BuildInventoryStateGroups();
    }

    private async Task HandleFileSelected(InputFileChangeEventArgs args)
    {
        var file = args.File;

        if (file.Size > MaxSrmSize)
        {
            StatusMessage = "Invalid SRAM File. (Randomizer saves aren't supported. Maybe one day...?)";
            return;
        }

        await using var stream = file.OpenReadStream(MaxSrmSize);
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        var data = memoryStream.ToArray();

        var result = GameService.LoadSramData(data, TextCharacterData, file.Name);
        if (!result.success)
        {
            StatusMessage = result.message;
            return;
        }

        CurrentRegion = result.region;
        StatusMessage = result.message;
        SelectedSlot = GameService.CurrentSlot;
        LoadPlayerStats();
    }

    private void LoadPlayerStats()
    {
        var stats = GameService.GetPlayerStats();
        PlayerName = stats.PlayerName;
        Rupees = stats.Rupees;
        HeartContainers = stats.HeartContainers;
        CurrentMagic = stats.CurrentMagic;
        ArrowUpgrades = stats.ArrowUpgrades;
        ArrowsHeld = stats.ArrowsHeld;
        BombUpgrades = stats.BombUpgrades;
        BombsHeld = stats.BombsHeld;
        MagicUpgrade = stats.MagicUpgrade;
        HeartPieces = stats.HeartPieces;
        Collectibles = GameService.GetCollectibleState();
        Bottles = GameService.GetBottleState();
        BuildInventorySlots();
        BuildCollectibleSlots();
    }

    private void BuildInventoryStateGroups() =>
        InventoryStateGroups =
        [
            new InventoryStateGroup("Bow", Constants.BowAddress,
            [
                new InventoryStateOption("None", 0x0),
                new InventoryStateOption("Bow", 0x1),
                new InventoryStateOption("Bow & Arrows", 0x2),
                new InventoryStateOption("Silver Bow", 0x3),
                new InventoryStateOption("Bow & Silver Arrows", 0x4)
            ]),
            new InventoryStateGroup("Boomerang", Constants.BoomerangAddress,
            [
                new InventoryStateOption("None", 0x0),
                new InventoryStateOption("Boomerang", 0x1),
                new InventoryStateOption("Magical Boomerang", 0x2)
            ]),
            new InventoryStateGroup("Mushroom/Powder", Constants.MushroomPowderAddress,
            [
                new InventoryStateOption("None", 0x0),
                new InventoryStateOption("Mushroom", 0x1),
                new InventoryStateOption("Magic Powder", 0x2)
            ]),
            new InventoryStateGroup("Shovel/Flute", Constants.ShovelFluteAddress,
            [
                new InventoryStateOption("None", 0x0),
                new InventoryStateOption("Shovel", 0x1),
                new InventoryStateOption("Flute (Inactive)", 0x2),
                new InventoryStateOption("Flute (Activated)", 0x3)
            ]),
            new InventoryStateGroup("Gloves", Constants.GlovesAddress,
            [
                new InventoryStateOption("None", 0x0),
                new InventoryStateOption("Power Glove", 0x1),
                new InventoryStateOption("Titan's Mitt", 0x2)
            ]),
            new InventoryStateGroup("Sword", Constants.SwordAddress,
            [
                new InventoryStateOption("None", 0x0),
                new InventoryStateOption("Fighter's Sword", 0x1),
                new InventoryStateOption("Master Sword", 0x2),
                new InventoryStateOption("Tempered Sword", 0x3),
                new InventoryStateOption("Golden Sword", 0x4)
            ]),
            new InventoryStateGroup("Shield", Constants.ShieldAddress,
            [
                new InventoryStateOption("None", 0x0),
                new InventoryStateOption("Fighter's Shield", 0x1),
                new InventoryStateOption("Red Shield", 0x2),
                new InventoryStateOption("Mirror Shield", 0x3)
            ]),
            new InventoryStateGroup("Armor", Constants.ArmorAddress,
            [
                new InventoryStateOption("Green Tunic", 0x0),
                new InventoryStateOption("Blue Tunic", 0x1),
                new InventoryStateOption("Red Tunic", 0x2)
            ])
        ];

    private void ApplyPlayerName()
    {
        GameService.SetPlayerName(PlayerName);
        StatusMessage = "Updated player name.";
    }

    private void ResetDeaths()
    {
        GameService.ResetDeaths(true);
        StatusMessage = "Reset deaths for the selected file.";
    }

    private void CreateFile()
    {
        var region = CurrentRegion ?? Enums.SaveRegion.USA;
        GameService.CreateFile(SelectedSlot, region, TextCharacterData);
        LoadPlayerStats();
        StatusMessage = $"Created File {SelectedSlot}.";
    }

    private void CopyFile()
    {
        var message = GameService.CopyFile(SelectedSlot, TextCharacterData);
        StatusMessage = string.IsNullOrWhiteSpace(message)
            ? $"Copied File {SelectedSlot}."
            : message;
    }

    private void WriteFile()
    {
        GameService.WriteFile(SelectedSlot, TextCharacterData);
        LoadPlayerStats();
        StatusMessage = $"Wrote to File {SelectedSlot}.";
    }

    private async Task EraseFile()
    {
        var confirm = await JsRuntime.InvokeAsync<bool>("confirm",
            $"You are about to PERMANENTLY ERASE File {SelectedSlot}! Are you sure?");
        if (!confirm)
        {
            return;
        }

        GameService.EraseFile(SelectedSlot);
        StatusMessage = $"Erased File {SelectedSlot}.";
    }

    private void ApplyPlayerChanges()
    {
        GameService.SetPlayerName(PlayerName);
        GameService.SetRupees(Rupees);
        GameService.SetHeartContainers(HeartContainers);
        GameService.SetMagic(CurrentMagic);
        GameService.SetArrowUpgrades(ArrowUpgrades);
        GameService.SetArrows(ArrowsHeld, ArrowUpgrades);
        GameService.SetBombUpgrades(BombUpgrades);
        GameService.SetBombs(BombsHeld, BombUpgrades);

        LoadPlayerStats();
        StatusMessage = "Applied player updates.";
    }

    private void CycleMagicUpgrade()
    {
        MagicUpgrade = GameService.CycleMagicUpgrade();
        StatusMessage = "Updated magic upgrade.";
    }

    private async Task SaveFile()
    {
        if (!GameService.HasLoadedFile)
        {
            StatusMessage = "Load a file first!";
            return;
        }

        var data = GameService.GetSaveData();
        var base64 = Convert.ToBase64String(data);
        var outputName = string.IsNullOrWhiteSpace(GameService.CurrentFilePath)
            ? "alttp.srm"
            : Path.GetFileName(GameService.CurrentFilePath);

        await JsRuntime.InvokeVoidAsync("alttpSramEditor.downloadFile", outputName, base64);
        StatusMessage = $"Prepared {outputName} for download.";
    }

    private void BuildInventorySlots() =>
        InventorySlots =
        [
            new InventorySlot("Bow", GetBowImage, () => CycleItem(Constants.BowAddress, 0x0, 0x1, 0x2, 0x3, 0x4)),
            new InventorySlot("Boomerang", GetBoomerangImage, () => CycleItem(Constants.BoomerangAddress, 0x0, 0x1, 0x2)),
            new InventorySlot("Hookshot", () => GetBinaryItemImage(Constants.HookshotAddress, 0x1, "Hookshot.png", "D Hookshot.png"),
                () => ToggleBinaryItem(Constants.HookshotAddress, 0x1)),
            new InventorySlot("Bombs", () => GetCountItemImage(Constants.BombCountAddress, "Bomb.png", "D Bomb.png"), ToggleBombCount),
            new InventorySlot("Mushroom/Powder", GetMushroomPowderImage,
                () => CycleItem(Constants.MushroomPowderAddress, 0x0, 0x1, 0x2)),
            new InventorySlot("Fire Rod", () => GetBinaryItemImage(Constants.FireRodAddress, 0x1, "Fire Rod.png", "D Fire Rod.png"),
                () => ToggleBinaryItem(Constants.FireRodAddress, 0x1)),
            new InventorySlot("Ice Rod", () => GetBinaryItemImage(Constants.IceRodAddress, 0x1, "Ice Rod.png", "D Ice Rod.png"),
                () => ToggleBinaryItem(Constants.IceRodAddress, 0x1)),
            new InventorySlot("Bombos", () => GetBinaryItemImage(Constants.BombosMedallionAddress, 0x1, "Bombos.png", "D Bombos.png"),
                () => ToggleBinaryItem(Constants.BombosMedallionAddress, 0x1)),
            new InventorySlot("Ether", () => GetBinaryItemImage(Constants.EtherMedallionAddress, 0x1, "Ether.png", "D Ether.png"),
                () => ToggleBinaryItem(Constants.EtherMedallionAddress, 0x1)),
            new InventorySlot("Quake", () => GetBinaryItemImage(Constants.QuakeMedallionAddress, 0x1, "Quake.png", "D Quake.png"),
                () => ToggleBinaryItem(Constants.QuakeMedallionAddress, 0x1)),
            new InventorySlot("Lamp", () => GetBinaryItemImage(Constants.LampAddress, 0x1, "Lamp.png", "D Lamp.png"),
                () => ToggleBinaryItem(Constants.LampAddress, 0x1)),
            new InventorySlot("Hammer", () => GetBinaryItemImage(Constants.MagicHammerAddress, 0x1, "Magic Hammer.png", "D Magic Hammer.png"),
                () => ToggleBinaryItem(Constants.MagicHammerAddress, 0x1)),
            new InventorySlot("Shovel/Flute", GetShovelFluteImage,
                () => CycleItem(Constants.ShovelFluteAddress, 0x0, 0x1, 0x2, 0x3)),
            new InventorySlot("Bug Net", () => GetBinaryItemImage(Constants.BugNetAddress, 0x1, "Bug-Catching Net.png", "D Bug-Catching Net.png"),
                () => ToggleBinaryItem(Constants.BugNetAddress, 0x1)),
            new InventorySlot("Book", () => GetBinaryItemImage(Constants.BookAddress, 0x1, "Book of Mudora.png", "D Book of Mudora.png"),
                () => ToggleBinaryItem(Constants.BookAddress, 0x1)),
            new InventorySlot("Bottle", GetBottleImage, ToggleBottle),
            new InventorySlot("Cane of Somaria", () => GetBinaryItemImage(Constants.CaneOfSomariaAddress, 0x1, "Cane of Somaria.png", "D Cane of Somaria.png"),
                () => ToggleBinaryItem(Constants.CaneOfSomariaAddress, 0x1)),
            new InventorySlot("Cane of Byrna", () => GetBinaryItemImage(Constants.CaneOfByrnaAddress, 0x1, "Cane of Byrna.png", "D Cane of Byrna.png"),
                () => ToggleBinaryItem(Constants.CaneOfByrnaAddress, 0x1)),
            new InventorySlot("Magic Cape", () => GetBinaryItemImage(Constants.MagicCapeAddress, 0x1, "Magic Cape.png", "D Magic Cape.png"),
                () => ToggleBinaryItem(Constants.MagicCapeAddress, 0x1)),
            new InventorySlot("Magic Mirror", () => GetBinaryItemImage(Constants.MagicMirrorAddress, 0x2, "Magic Mirror.png", "D Magic Mirror.png"),
                () => ToggleBinaryItem(Constants.MagicMirrorAddress, 0x2)),
            new InventorySlot("Gloves", GetGlovesImage, () => CycleItem(Constants.GlovesAddress, 0x0, 0x1, 0x2)),
            new InventorySlot("Pegasus Boots", () => GetBinaryItemImage(Constants.PegasusBootsAddress, 0x1, "Pegasus Boots.png", "D Pegasus Boots.png"),
                GameService.TogglePegasusBoots),
            new InventorySlot("Zora's Flippers", () => GetBinaryItemImage(Constants.ZorasFlippersAddress, 0x1, "Zora's Flippers.png", "D Zora's Flippers.png"),
                GameService.ToggleZorasFlippers),
            new InventorySlot("Moon Pearl", () => GetBinaryItemImage(Constants.MoonPearlAddress, 0x1, "Moon Pearl.png", "D Moon Pearl.png"),
                () => ToggleBinaryItem(Constants.MoonPearlAddress, 0x1)),
            new InventorySlot("Sword", GetSwordImage, () => CycleItem(Constants.SwordAddress, 0x0, 0x1, 0x2, 0x3, 0x4)),
            new InventorySlot("Shield", GetShieldImage, () => CycleItem(Constants.ShieldAddress, 0x0, 0x1, 0x2, 0x3)),
            new InventorySlot("Armor", GetArmorImage, () => CycleItem(Constants.ArmorAddress, 0x0, 0x1, 0x2))
        ];

    private void BuildCollectibleSlots()
    {
        PendantSlots =
        [
            new CollectibleSlot("Pendant of Power", Constants.RedPendantAddress,
                () => GetPendantImage(Constants.RedPendantAddress, "Red Pendant.png")),
            new CollectibleSlot("Pendant of Wisdom", Constants.BluePendantAddress,
                () => GetPendantImage(Constants.BluePendantAddress, "Blue Pendant.png")),
            new CollectibleSlot("Pendant of Courage", Constants.GreenPendantAddress,
                () => GetPendantImage(Constants.GreenPendantAddress, "Green Pendant.png"))
        ];

        CrystalSlots =
        [
            new CollectibleSlot("Crystal 1", Constants.CrystalMMAddress, () => GetCrystalImage(Constants.CrystalMMAddress)),
            new CollectibleSlot("Crystal 2", Constants.CrystalPoD, () => GetCrystalImage(Constants.CrystalPoD)),
            new CollectibleSlot("Crystal 3", Constants.CrystalIPAddress, () => GetCrystalImage(Constants.CrystalIPAddress)),
            new CollectibleSlot("Crystal 4", Constants.CrystalTRAddress, () => GetCrystalImage(Constants.CrystalTRAddress)),
            new CollectibleSlot("Crystal 5", Constants.CrystalSPAddress, () => GetCrystalImage(Constants.CrystalSPAddress, true)),
            new CollectibleSlot("Crystal 6", Constants.CrystalTTAddress, () => GetCrystalImage(Constants.CrystalTTAddress, true)),
            new CollectibleSlot("Crystal 7", Constants.CrystalSWAddress, () => GetCrystalImage(Constants.CrystalSWAddress))
        ];
    }

    private static string GetResourceUrl(string fileName) => $"resources/{Uri.EscapeDataString(fileName)}";

    private string GetFilenamePanelImage() => CurrentRegion == Enums.SaveRegion.JPN
        ? "filename_JP.png"
        : "filename_USA.png";

    private string GetBowImage()
    {
        var value = GameService.GetItem(Constants.BowAddress);
        return value switch
        {
            0x1 => GetResourceUrl("Bow.png"),
            0x2 => GetResourceUrl("Bow and Arrow.png"),
            0x3 or 0x4 => GetResourceUrl("Bow and Light Arrow.png"),
            _ => GetResourceUrl("D Bow.png")
        };
    }

    private string GetBoomerangImage()
    {
        var value = GameService.GetItem(Constants.BoomerangAddress);
        return value switch
        {
            0x1 => GetResourceUrl("Boomerang.png"),
            0x2 => GetResourceUrl("Magical Boomerang.png"),
            _ => GetResourceUrl("D Boomerang.png")
        };
    }

    private string GetMushroomPowderImage()
    {
        var value = GameService.GetItem(Constants.MushroomPowderAddress);
        return value switch
        {
            0x1 => GetResourceUrl("Mushroom.png"),
            0x2 => GetResourceUrl("Magic Powder.png"),
            _ => GetResourceUrl("D Mushroom.png")
        };
    }

    private string GetShovelFluteImage()
    {
        var value = GameService.GetItem(Constants.ShovelFluteAddress);
        return value switch
        {
            0x1 => GetResourceUrl("Shovel.png"),
            0x2 or 0x3 => GetResourceUrl("Flute.png"),
            _ => GetResourceUrl("D Shovel.png")
        };
    }

    private string GetGlovesImage()
    {
        var value = GameService.GetItem(Constants.GlovesAddress);
        return value switch
        {
            0x1 => GetResourceUrl("Power Glove.png"),
            0x2 => GetResourceUrl("Titan's Mitt.png"),
            _ => GetResourceUrl("D Power Glove.png")
        };
    }

    private string GetSwordImage()
    {
        var value = GameService.GetItem(Constants.SwordAddress);
        return value switch
        {
            0x1 => GetResourceUrl("Fighter's Sword.png"),
            0x2 => GetResourceUrl("Master Sword.png"),
            0x3 => GetResourceUrl("Tempered Sword.png"),
            0x4 => GetResourceUrl("Golden Sword.png"),
            _ => GetResourceUrl("D Fighter's Sword.png")
        };
    }

    private string GetShieldImage()
    {
        var value = GameService.GetItem(Constants.ShieldAddress);
        return value switch
        {
            0x1 => GetResourceUrl("Fighter's Shield.png"),
            0x2 => GetResourceUrl("Red Shield.png"),
            0x3 => GetResourceUrl("Mirror Shield.png"),
            _ => GetResourceUrl("D Fighter's Shield.png")
        };
    }

    private string GetArmorImage()
    {
        var value = GameService.GetItem(Constants.ArmorAddress);
        return value switch
        {
            0x1 => GetResourceUrl("Blue Tunic.png"),
            0x2 => GetResourceUrl("Red Tunic.png"),
            _ => GetResourceUrl("Green Tunic.png")
        };
    }

    private string GetBottleImage()
    {
        var bottleContents = Bottles?.InventoryBottleDisplay ?? 0;
        return GetBottleContentImage(bottleContents);
    }

    private string GetBottleContentImage(int contents) => contents switch
    {
        (int)Enums.BottleContents.EMPTY => GetResourceUrl("Bottle.png"),
        (int)Enums.BottleContents.RED_POTION => GetResourceUrl("Red Potion.png"),
        (int)Enums.BottleContents.GREEN_POTION => GetResourceUrl("Green Potion.png"),
        (int)Enums.BottleContents.BLUE_POTION => GetResourceUrl("Blue Potion.png"),
        (int)Enums.BottleContents.FAERIE => GetResourceUrl("Fairy.png"),
        (int)Enums.BottleContents.BEE => GetResourceUrl("Bee.png"),
        (int)Enums.BottleContents.GOOD_BEE => GetResourceUrl("Bee.png"),
        (int)Enums.BottleContents.MUSHROOM => GetResourceUrl("Mushroom.png"),
        _ => GetResourceUrl("D Bottle.png")
    };

    private void ToggleInventoryItem(InventorySlot slot)
    {
        slot.ToggleAction?.Invoke();
        RefreshUiState();
    }

    private int GetItemValue(int address) => GameService.GetItem(address);

    private void SetItemValue(int address, int value)
    {
        GameService.SetItem(address, (byte)value);
        RefreshUiState();
    }

    private int GetBottleContents(int bottleNumber) => bottleNumber switch
    {
        1 => Bottles?.Bottle1Contents ?? 0,
        2 => Bottles?.Bottle2Contents ?? 0,
        3 => Bottles?.Bottle3Contents ?? 0,
        4 => Bottles?.Bottle4Contents ?? 0,
        _ => 0
    };

    private void SetBottleContentsValue(int bottleNumber, int value)
    {
        GameService.SetBottleContents(bottleNumber, (byte)value);
        RefreshUiState();
    }

    private int Bottle1Value
    {
        get => GetBottleContents(1);
        set => SetBottleContentsValue(1, value);
    }

    private int Bottle2Value
    {
        get => GetBottleContents(2);
        set => SetBottleContentsValue(2, value);
    }

    private int Bottle3Value
    {
        get => GetBottleContents(3);
        set => SetBottleContentsValue(3, value);
    }

    private int Bottle4Value
    {
        get => GetBottleContents(4);
        set => SetBottleContentsValue(4, value);
    }

    private void TogglePendant(CollectibleSlot slot)
    {
        GameService.TogglePendant(slot.BitValue);
        RefreshUiState();
    }

    private void ToggleCrystal(CollectibleSlot slot)
    {
        GameService.ToggleCrystal(slot.BitValue);
        RefreshUiState();
    }

    private void RefreshUiState()
    {
        Collectibles = GameService.GetCollectibleState();
        Bottles = GameService.GetBottleState();
        BuildInventorySlots();
        BuildCollectibleSlots();
        StateHasChanged();
    }

    private void ToggleBinaryItem(int address, int enabledValue) =>
        GameService.ToggleItem(address, enabledValue);

    private void ToggleBombCount()
    {
        var current = GameService.GetItem(Constants.BombCountAddress);
        var next = current > 0
            ? 0
            : 1;
        GameService.SetItem(Constants.BombCountAddress, (byte)next);
    }

    private void ToggleBottle()
    {
        var contents = Bottles?.Bottle1Contents ?? 0;
        var next = contents > 0
            ? (byte)0
            : (byte)Enums.BottleContents.EMPTY;
        GameService.SetBottleContents(1, next);
    }

    private void CycleItem(int address, params int[] values)
    {
        var current = GameService.GetItem(address);
        var index = Array.IndexOf(values, current);
        var nextIndex = index < 0
            ? 0
            : (index + 1) % values.Length;
        GameService.SetItem(address, (byte)values[nextIndex]);
    }

    private string GetBinaryItemImage(int address, int enabledValue, string onImage, string offImage)
    {
        var value = GameService.GetItem(address);
        return value == enabledValue
            ? GetResourceUrl(onImage)
            : GetResourceUrl(offImage);
    }

    private string GetCountItemImage(int address, string onImage, string offImage)
    {
        var value = GameService.GetItem(address);
        return value > 0
            ? GetResourceUrl(onImage)
            : GetResourceUrl(offImage);
    }

    private string GetPendantImage(int pendantBit, string enabledImage) =>
        GetCollectibleImage(Collectibles?.Pendants ?? 0, pendantBit, enabledImage, "Clear Pendant.png");

    private string GetCrystalImage(int crystalBit, bool isRed = false) =>
        GetCollectibleImage(Collectibles?.Crystals ?? 0, crystalBit,
            isRed
                ? "Red Crystal.png"
                : "Blue Crystal.png",
            "Clear Crystal.png");

    private string GetCollectibleImage(byte value, int bitNumber, string enabledImage, string disabledImage) =>
        GetBit(value, bitNumber)
            ? GetResourceUrl(enabledImage)
            : GetResourceUrl(disabledImage);

    private string GetMagicBarImage() => MagicUpgrade switch
    {
        0x1 => "lttp_magic_bar_halved.png",
        0x2 => "lttp_magic_bar_quarter.png",
        _ => "lttp_magic_bar.png"
    };

    private string GetMagicFillStyle()
    {
        var steps = GetMagicFillSteps();
        return $"left: 4px; width: 8px; height: {steps}px; bottom: 5px; background-color: #21C329;";
    }

    private string GetMagicHighlightStyle()
    {
        var steps = GetMagicFillSteps();
        return $"left: 5px; width: 6px; height: 1px; bottom: {steps + 4}px; background-color: #FFFBFF;";
    }

    private int GetMagicFillSteps()
    {
        const int maxMagic = 128;
        var clamped = Math.Clamp(CurrentMagic, 0, maxMagic);
        return (clamped + 3) / 4;
    }

    private string GetHeartPieceImage() => (HeartPieces % 4) switch
    {
        1 => "Piece of Heart Quarter.png",
        2 => "Piece of Heart Half.png",
        3 => "Piece of Heart Three Quarters.png",
        _ => "Piece of Heart Empty.png"
    };

    private static bool GetBit(byte value, int bitNumber)
    {
        bitNumber++;
        return (value & 1 << bitNumber - 1) != 0;
    }

    private record InventorySlot(string Name, Func<string> ImageSelector, Action? ToggleAction = null);

    private record InventoryStateGroup(string Name, int Address, IReadOnlyList<InventoryStateOption> Options);

    private record InventoryStateOption(string Label, int Value);

    private record BottleOption(string Label, int Value);

    private record CollectibleSlot(string Name, int BitValue, Func<string> ImageSelector);
}
