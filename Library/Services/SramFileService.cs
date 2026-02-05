namespace Library.Services;

/// <summary>
///     Service responsible for SRAM file I/O operations
/// </summary>
public class SramFileService
{
    private const int SrmSize = 0x2000;

    /// <summary>
    ///     Loads and validates an SRAM file
    /// </summary>
    public FileLoadResult LoadSramFile(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                return new FileLoadResult(false, ErrorMessage: "File does not exist.");
            }

            var bytes = File.ReadAllBytes(filePath);
            var fileSize = new FileInfo(filePath).Length;

            return fileSize switch
            {
                SrmSize => new FileLoadResult(true, bytes, FileSize: fileSize),
                > SrmSize => ValidateLargerFile(bytes, fileSize),
                _ => new FileLoadResult(false, ErrorMessage: "Invalid SRAM File.")
            };
        }
        catch (IOException ex)
        {
            return new FileLoadResult(false, ErrorMessage: $"File reading conflict: {ex.Message}");
        }
        catch (Exception ex)
        {
            return new FileLoadResult(false, ErrorMessage: $"The file could not be read: {ex.Message}");
        }
    }

    /// <summary>
    ///     Saves SRAM data to a file
    /// </summary>
    public FileSaveResult SaveSramFile(string filePath, byte[] data)
    {
        try
        {
            File.WriteAllBytes(filePath, data);
            return new FileSaveResult(true);
        }
        catch (IOException ex)
        {
            return new FileSaveResult(false, $"File writing conflict: {ex.Message}");
        }
        catch (Exception ex)
        {
            return new FileSaveResult(false, $"Could not save file: {ex.Message}");
        }
    }

    private static FileLoadResult ValidateLargerFile(byte[] bytes, long fileSize)
    {
        if (fileSize > 0x8000)
        {
            return new FileLoadResult(false,
                ErrorMessage: "Invalid SRAM File. (Randomizer saves aren't supported. Maybe one day...?)");
        }

        for (var i = 0x2000; i < 0x8000; i++)
        {
            if (bytes[i] != 0x0)
            {
                return new FileLoadResult(false,
                    ErrorMessage: "Invalid SRAM File. (Randomizer saves aren't supported. Maybe one day...?)");
            }
        }

        return new FileLoadResult(true, bytes, FileSize: fileSize);
    }

    public record FileLoadResult(
        bool Success,
        byte[]? Data = null,
        string? ErrorMessage = null,
        long? FileSize = null);

    public record FileSaveResult(
        bool Success,
        string? ErrorMessage = null);
}
