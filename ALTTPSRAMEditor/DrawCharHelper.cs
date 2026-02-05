// ReSharper disable InconsistentNaming

namespace ALTTPSRAMEditor;

public static class DrawCharHelper
{
    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility",
        Justification = "This is a Windows Forms application.")]
    public static Image GetCharTexture(Bitmap fnt, int tileId, SaveRegion saveRegion = SaveRegion.JPN,
        bool hugLeft = false, int scale = 1)
    {
        var tileset_width = saveRegion switch
        {
            SaveRegion.JPN => 20, // Japanese Font
            SaveRegion.USA => 27, // English Font
            _ => 27
        };
        const int tile_w = 8;
        const int tile_h = 16;
        var x = tileId % tileset_width * tile_w;
        var y = tileId / tileset_width * tile_h;
        const int width = 8;
        const int height = 16;
        var crop = new Rectangle(x, y, width, height); // Crop the original tile size
        var tex = new Bitmap(width * scale, height * scale); // Create bitmap at scaled size

        using var charGr = Graphics.FromImage(tex);
        charGr.InterpolationMode = InterpolationMode.NearestNeighbor;
        charGr.PixelOffsetMode = PixelOffsetMode.Half;
        charGr.DrawImage(fnt, new Rectangle(0, 0, tex.Width, tex.Height), crop, GraphicsUnit.Pixel); // Draw scaled

        if (hugLeft)
        {
            return tex;
        }

        var bmp = new Bitmap(tex.Width * 2, tex.Height); // Create wider bitmap for padding

        using var hugRightGr = Graphics.FromImage(bmp);
        hugRightGr.InterpolationMode = InterpolationMode.NearestNeighbor;
        hugRightGr.PixelOffsetMode = PixelOffsetMode.Half;
        hugRightGr.Clear(Color.Black);
        hugRightGr.DrawImage(tex, width * scale, 2); // Position the scaled character with padding
        return bmp;
    }
}