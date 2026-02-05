# Blazor WASM Implementation Guide

## Quick Start for Blazor Integration

This guide shows how to use the refactored service layer in a Blazor WASM application.

## Step 1: Create Blazor Project

```bash
dotnet new blazorwasm -o ALTTPSRAMEditor.Blazor
cd ALTTPSRAMEditor.Blazor
dotnet add reference ../Library/Library.csproj
```

## Step 2: Setup Dependency Injection

**Program.cs:**
```csharp
using Library.Services;
using Library.Classes;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

// Register services
builder.Services.AddSingleton<GameService>();
builder.Services.AddSingleton<TextCharacterData>();

// Optional: Add browser storage adapter
builder.Services.AddScoped<IBrowserFileService, BrowserFileService>();

await builder.Build().RunAsync();
```

## Step 3: Create File Upload Component

**Pages/FileManager.razor:**
```razor
@page "/file-manager"
@inject GameService GameService
@inject TextCharacterData TextCharacterData

<h3>ALTTP SRAM Editor</h3>

<InputFile OnChange="HandleFileSelection" accept=".srm,.SaveRAM" />

@if (!string.IsNullOrEmpty(message))
{
    <div class="alert @alertClass">@message</div>
}

@code {
    private string message = "";
    private string alertClass = "";

    private async Task HandleFileSelection(InputFileChangeEventArgs e)
    {
        var file = e.File;
        
        if (file == null) return;

        try
        {
            // Read file into byte array
            var buffer = new byte[file.Size];
            await file.OpenReadStream(maxAllowedSize: 512000)
                .ReadAsync(buffer);

            // Use the same service as WinForms!
            var (success, msg, region) = GameService.OpenFile(
                file.Name, 
                TextCharacterData);

            if (success)
            {
                message = $"Loaded {file.Name} successfully!";
                alertClass = "alert-success";
                StateHasChanged();
            }
            else
            {
                message = msg;
                alertClass = "alert-danger";
            }
        }
        catch (Exception ex)
        {
            message = $"Error: {ex.Message}";
            alertClass = "alert-danger";
        }
    }
}
```

## Step 4: Create Save Slot Component

**Components/SaveSlotEditor.razor:**
```razor
@inject GameService GameService

@if (GameService.HasLoadedFile && GameService.IsCurrentSlotValid())
{
    <div class="save-slot-editor">
        <h4>Editing Slot @GameService.CurrentSlot</h4>
        
        <div class="stat-group">
            <label>Player Name:</label>
            <span>@playerStats?.PlayerName</span>
            <button @onclick="ChangeName">Change Name</button>
        </div>

        <div class="stat-group">
            <label>Rupees:</label>
            <input type="number" 
                   @bind="rupees" 
                   @bind:event="oninput"
                   @onchange="UpdateRupees"
                   min="0" 
                   max="999" />
        </div>

        <div class="stat-group">
            <label>Heart Containers:</label>
            <input type="number" 
                   @bind="heartContainers" 
                   @bind:event="oninput"
                   @onchange="UpdateHeartContainers"
                   min="8" 
                   max="160"
                   step="8" />
        </div>

        <button @onclick="SaveFile">Save Changes</button>
    </div>
}

@code {
    private ViewModels.PlayerStats? playerStats;
    private int rupees;
    private int heartContainers;

    protected override void OnInitialized()
    {
        // Subscribe to events
        GameService.SaveSlotChanged += OnSlotChanged;
        
        if (GameService.HasLoadedFile)
        {
            LoadPlayerData();
        }
    }

    private void OnSlotChanged(object? sender, SaveSlotChangedEventArgs e)
    {
        LoadPlayerData();
        StateHasChanged();
    }

    private void LoadPlayerData()
    {
        playerStats = GameService.GetPlayerStats();
        rupees = playerStats.Rupees;
        heartContainers = playerStats.HeartContainers;
    }

    private void UpdateRupees()
    {
        GameService.SetRupees(rupees);
    }

    private void UpdateHeartContainers()
    {
        GameService.SetHeartContainers(heartContainers);
    }

    private async Task SaveFile()
    {
        var (success, message) = GameService.SaveFile();
        
        // Since we're in browser, we need to trigger download
        if (success)
        {
            await DownloadSaveFile();
        }
    }

    private async Task DownloadSaveFile()
    {
        // Use JS interop to download file
        var data = GameService.GetCurrentSaveData();
        await JSRuntime.InvokeVoidAsync("downloadFile", 
            GameService.CurrentFilePath, 
            data);
    }

    public void Dispose()
    {
        GameService.SaveSlotChanged -= OnSlotChanged;
    }
}
```

## Step 5: Create Item Manager Component

**Components/ItemManager.razor:**
```razor
@inject GameService GameService

<div class="item-grid">
    @foreach (var item in items)
    {
        <div class="item-slot" @onclick="() => ToggleItem(item)">
            <img src="@GetItemImage(item)" 
                 alt="@item.Name" 
                 class="@(item.IsEnabled ? "" : "disabled")" />
        </div>
    }
</div>

@code {
    private List<ItemDisplay> items = new();

    protected override void OnInitialized()
    {
        LoadItems();
    }

    private void LoadItems()
    {
        items = new List<ItemDisplay>
        {
            new("Hookshot", Constants.HookshotAddress, 0x1),
            new("Fire Rod", Constants.FireRodAddress, 0x1),
            new("Ice Rod", Constants.IceRodAddress, 0x1),
            // ... add more items
        };

        RefreshItemStates();
    }

    private void RefreshItemStates()
    {
        if (!GameService.HasLoadedFile) return;

        foreach (var item in items)
        {
            item.IsEnabled = GameService.GetItem(item.Address) == item.EnabledValue;
        }
    }

    private void ToggleItem(ItemDisplay item)
    {
        GameService.ToggleItem(item.Address, item.EnabledValue);
        RefreshItemStates();
        StateHasChanged();
    }

    private string GetItemImage(ItemDisplay item)
    {
        // Return appropriate image path
        return $"/images/items/{item.Name.ToLower()}.png";
    }

    record ItemDisplay(string Name, int Address, int EnabledValue)
    {
        public bool IsEnabled { get; set; }
    }
}
```

## Step 6: Add JavaScript Interop for File Download

**wwwroot/index.html:**
```html
<script>
    window.downloadFile = function(filename, byteArray) {
        const blob = new Blob([byteArray], { type: 'application/octet-stream' });
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = filename;
        link.click();
        window.URL.revokeObjectURL(url);
    };
</script>
```

## Step 7: Browser Storage Adapter (Optional)

For persisting saves in browser storage:

**Services/BrowserFileService.cs:**
```csharp
public interface IBrowserFileService
{
    Task<byte[]?> LoadFromStorageAsync(string key);
    Task SaveToStorageAsync(string key, byte[] data);
}

public class BrowserFileService : IBrowserFileService
{
    private readonly IJSRuntime _js;

    public BrowserFileService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<byte[]?> LoadFromStorageAsync(string key)
    {
        var json = await _js.InvokeAsync<string>("localStorage.getItem", key);
        if (string.IsNullOrEmpty(json)) return null;
        
        return JsonSerializer.Deserialize<byte[]>(json);
    }

    public async Task SaveToStorageAsync(string key, byte[] data)
    {
        var json = JsonSerializer.Serialize(data);
        await _js.InvokeVoidAsync("localStorage.setItem", key, json);
    }
}
```

## Key Differences from WinForms

### 1. Async File Operations
```csharp
// WinForms (sync)
var bytes = File.ReadAllBytes(path);

// Blazor (async)
var buffer = new byte[file.Size];
await file.OpenReadStream().ReadAsync(buffer);
```

### 2. File Downloads
```csharp
// WinForms (direct)
File.WriteAllBytes(path, data);

// Blazor (JS interop)
await JSRuntime.InvokeVoidAsync("downloadFile", filename, data);
```

### 3. State Management
```csharp
// WinForms (immediate)
UpdateUI();

// Blazor (explicit)
StateHasChanged();
```

## Sharing Assets

Both WinForms and Blazor can share:
- ? All service layer code
- ? Business logic
- ? ViewModels
- ? Constants and enums
- ? Data models (SRAM, SaveSlot, Link)

What needs to be different:
- ? File I/O (WinForms uses File.*, Blazor uses InputFile)
- ? UI components (Forms vs. Razor)
- ? Image rendering (PictureBox vs. img tag)

## Example: Complete Blazor Page

**Pages/Editor.razor:**
```razor
@page "/editor"
@inject GameService GameService
@inject TextCharacterData TextCharacterData
@implements IDisposable

<div class="editor-container">
    @if (!GameService.HasLoadedFile)
    {
        <FileUpload />
    }
    else
    {
        <div class="editor-layout">
            <div class="file-selector">
                <h4>Select Save File</h4>
                @for (int i = 1; i <= 3; i++)
                {
                    var slot = i;
                    <button class="@GetSlotClass(slot)" 
                            @onclick="() => SelectSlot(slot)">
                        File @slot
                    </button>
                }
            </div>

            @if (GameService.IsCurrentSlotValid())
            {
                <div class="main-editor">
                    <SaveSlotEditor />
                    <ItemManager />
                    <CollectibleManager />
                </div>
            }
            else
            {
                <div class="empty-slot">
                    <p>This slot is empty</p>
                    <button @onclick="CreateFile">Create New File</button>
                </div>
            }
        </div>
    }
</div>

@code {
    protected override void OnInitialized()
    {
        GameService.SaveSlotChanged += OnSlotChanged;
        GameService.SramLoaded += OnSramLoaded;
    }

    private void OnSlotChanged(object? sender, SaveSlotChangedEventArgs e)
    {
        StateHasChanged();
    }

    private void OnSramLoaded(object? sender, SramLoadedEventArgs e)
    {
        StateHasChanged();
    }

    private void SelectSlot(int slot)
    {
        GameService.SetCurrentSlot(slot);
    }

    private void CreateFile()
    {
        var region = SaveRegion.USA; // Or prompt user
        GameService.CreateFile(GameService.CurrentSlot, region, TextCharacterData);
        StateHasChanged();
    }

    private string GetSlotClass(int slot)
    {
        return slot == GameService.CurrentSlot ? "active" : "";
    }

    public void Dispose()
    {
        GameService.SaveSlotChanged -= OnSlotChanged;
        GameService.SramLoaded -= OnSramLoaded;
    }
}
```

## CSS Styling (Optional)

**wwwroot/css/app.css:**
```css
.editor-container {
    max-width: 1200px;
    margin: 0 auto;
    padding: 20px;
}

.editor-layout {
    display: grid;
    grid-template-columns: 200px 1fr;
    gap: 20px;
}

.item-grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(64px, 1fr));
    gap: 10px;
}

.item-slot img.disabled {
    opacity: 0.3;
    filter: grayscale(100%);
}
```

## Testing

```csharp
public class GameServiceTests
{
    [Fact]
    public void CreateFile_CreatesValidSlot()
    {
        // Arrange
        var gameService = new GameService();
        var textData = new TextCharacterData();
        
        // This works identically in both WinForms and Blazor!
        
        // Act
        var slot = gameService.CreateFile(1, SaveRegion.USA, textData);
        
        // Assert
        Assert.True(slot.SaveIsValid());
    }
}
```

## Conclusion

The refactored architecture allows you to:
1. ? Reuse 100% of business logic
2. ? Use the same services in both UIs
3. ? Share ViewModels and data transfer objects
4. ? Maintain consistent behavior
5. ? Test once, run everywhere

The main adaptation needed is handling file I/O differences between desktop and web environments.
