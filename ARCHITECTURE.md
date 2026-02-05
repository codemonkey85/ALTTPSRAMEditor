# Architecture Refactoring Summary

## Overview

The codebase has been refactored to decouple business logic from the UI layer, enabling code reuse across different presentation layers (Windows Forms, Blazor WASM, etc.).

## New Service Layer

### Directory Structure

```
Library/
??? Services/
?   ??? SramFileService.cs          # File I/O operations
?   ??? GameStateService.cs         # Game state and save slot management
?   ??? ItemManagementService.cs    # Item/equipment logic
?   ??? GameService.cs              # Facade coordinating all services
?   ??? ViewModels.cs               # Data transfer objects
??? Classes/
?   ??? SRAM.cs                     # Existing SRAM data model
?   ??? SaveSlot.cs                 # Existing save slot model
?   ??? Link.cs                     # Existing player model
?   ??? ... (other existing classes)
```

## Service Responsibilities

### 1. SramFileService
**Purpose:** Handles all file I/O operations for SRAM files

**Key Methods:**
- `LoadSramFile(string filePath)` - Loads and validates SRAM files
- `SaveSramFile(string filePath, byte[] data)` - Saves SRAM data to disk
- Validates file sizes and formats
- Returns structured results with success/failure and error messages

**Benefits:**
- Centralized file handling logic
- Consistent error handling
- Easy to mock for testing
- Can be reused across different UI frameworks

### 2. GameStateService
**Purpose:** Manages game state and save slots

**Key Methods:**
- `LoadSram(byte[] data, TextCharacterData)` - Initializes SRAM data
- `SetCurrentSlot(int slot)` - Changes active save slot
- `GetCurrentSaveSlot()` / `GetSaveSlot(int)` - Retrieves save slots
- `GetCurrentPlayer()` - Gets the active player
- `CreateFile()`, `CopyFile()`, `WriteFile()`, `EraseFile()` - Save file operations
- `MergeSaveData()` - Prepares data for writing

**Events:**
- `SaveSlotChanged` - Fired when active slot changes
- `SramLoaded` - Fired when SRAM data is loaded

**Benefits:**
- Centralized state management
- Event-driven architecture for UI updates
- Single source of truth for current game state

### 3. ItemManagementService
**Purpose:** Encapsulates item/equipment business logic

**Key Methods:**
- `GetMaxArrows(int upgradeLevel)` - Calculates max arrow capacity
- `GetMaxBombs(int upgradeLevel)` - Calculates max bomb capacity
- `GetInventoryBottleContents(Link)` - Finds first non-empty bottle
- `NormalizeBottleContent(int)` - Handles bottle content value mapping
- `UpdateSelectedBottle(Link)` - Updates which bottle is selected
- `ToggleItem()`, `TogglePegasusBoots()`, `ToggleZorasFlippers()` - Item toggle logic
- `CycleMagicUpgrade()` - Magic upgrade cycling
- `IncrementHeartPiece()`, `DecrementHeartPiece()` - Heart piece management

**Benefits:**
- Reusable game logic
- Centralized item rules
- Easier to test and modify item behavior
- No UI dependencies

### 4. GameService (Facade)
**Purpose:** Provides a simplified, unified interface for the UI layer

**Key Methods:**
Combines all lower-level services into a cohesive API:
- File operations (open, save)
- Save slot management
- Player data operations (rupees, hearts, magic, name)
- Item operations (get, set, toggle)
- Collectible operations (pendants, crystals)

**Benefits:**
- Single entry point for UI
- Simplified API surface
- Easier dependency management
- Can add cross-cutting concerns (logging, validation)

### 5. ViewModels
**Purpose:** Data transfer objects for UI-service communication

**Key Types:**
- `PlayerStats` - Comprehensive player statistics
- `ItemState` - Item status information
- `BottleState` - All bottle contents
- `CollectibleState` - Pendants and crystals
- `GameState` - Complete game state snapshot

**Benefits:**
- Decouples UI from domain models
- Type-safe data transfer
- Easy to serialize for different frameworks
- Clear contracts between layers

## Changes to MainForm.cs

### Before
```csharp
// Direct manipulation of static fields
private static SRAM? sdat;
private static string fname = string.Empty;

// Direct file I/O
var bytes = File.ReadAllBytes(fname);
sdat = new SRAM(bytes, TextCharacterData);

// Direct state management
var savslot = SRAM.GetSaveSlot(1);
```

### After
```csharp
// Service-based architecture
private readonly GameService _gameService;

// File operations through service
var (success, message, region) = _gameService.OpenFile(filePath, TextCharacterData);

// State management through service
_gameService.SetCurrentSlot(1);
var savslot = _gameService.GetCurrentSaveSlot();
```

### Key Improvements
1. **Removed static fields** - Better testability and encapsulation
2. **Event-driven updates** - Subscribe to `SaveSlotChanged` and `SramLoaded` events
3. **Simplified methods** - UI methods now delegate to services
4. **Consistent error handling** - Services return structured results

## Usage Examples

### Opening a File
```csharp
var (success, message, region) = _gameService.OpenFile(filePath, TextCharacterData);
if (success)
{
    saveRegion = region!.Value;
    UpdateUI();
}
else
{
    MessageBox.Show(message);
}
```

### Managing Items
```csharp
// Get max arrows based on upgrade level
var maxArrows = ItemManagementService.GetMaxArrows(upgradeLevel);

// Toggle an item
_gameService.ToggleItem(Constants.HookshotAddress, 0x1);

// Manage heart pieces
var (newContainers, newPieces) = ItemManagementService.IncrementHeartPiece(
    currentContainers, 
    currentPieces);
```

### Saving
```csharp
var (success, message) = _gameService.SaveFile();
helperText.Text = message;
if (success)
{
    buttonWrite.Enabled = false;
}
```

## Benefits for Blazor WASM

The new architecture enables easy Blazor integration:

### 1. Shared Library
```csharp
// Blazor component can use the same services
@inject GameService GameService

protected override async Task OnInitializedAsync()
{
    GameService.SaveSlotChanged += OnSlotChanged;
    var (success, message, _) = GameService.OpenFile(path, textData);
    // Update UI
}
```

### 2. No Platform Dependencies
- Services have no WinForms dependencies
- File I/O can be replaced with browser storage
- ViewModels work identically in both UIs

### 3. Incremental Migration
- Start with file upload/download in Blazor
- Gradually add UI features
- Share all business logic

## Testing Improvements

### Before
```csharp
// Hard to test - tightly coupled to UI and static state
public void buttonCreate_Click(object sender, EventArgs e)
{
    if (radioFile1.Checked)
        SRAM.CreateFile(1, saveRegion, TextCharacterData);
    // ... UI updates mixed with logic
}
```

### After
```csharp
// Easy to test - services are independent
[Test]
public void CreateFile_ShouldCreateValidSlot()
{
    var gameService = new GameService();
    // ... load test data
    
    var saveSlot = gameService.CreateFile(1, SaveRegion.USA, testData);
    
    Assert.IsTrue(saveSlot.SaveIsValid());
}
```

## Next Steps

### Recommended Improvements

1. **Dependency Injection**
   - Register services in DI container
   - Enable interface-based testing
   - Support multiple implementations (file vs. memory storage)

2. **Interface Extraction**
   ```csharp
   public interface IGameService { ... }
   public interface ISramFileService { ... }
   ```

3. **Async/Await**
   - Make file operations async
   - Better responsiveness in UI
   - Required for Blazor WASM browser storage

4. **Unit Tests**
   - Test services independently
   - Mock dependencies
   - Validate business logic

5. **State Management**
   - Consider adding a state store (Redux-style)
   - Immutable state updates
   - Time-travel debugging

## Migration Checklist

- [x] Create service layer (SramFileService, GameStateService, ItemManagementService, GameService)
- [x] Create ViewModels for data transfer
- [x] Refactor MainForm to use services
- [x] Remove static fields from MainForm
- [x] Add event-driven architecture
- [x] Build successfully
- [ ] Add unit tests for services
- [ ] Extract interfaces
- [ ] Make file operations async
- [ ] Create Blazor WASM project
- [ ] Implement browser storage adapter
- [ ] Share services between WinForms and Blazor

## Conclusion

The refactoring successfully decouples business logic from the UI layer while maintaining all existing functionality. The new architecture:

- ? Enables code sharing across platforms
- ? Improves testability
- ? Follows SOLID principles
- ? Maintains backward compatibility
- ? Builds without errors

The codebase is now ready for Blazor WASM implementation and future enhancements.
