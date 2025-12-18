using System.Drawing;
using System.Drawing.Drawing2D;

namespace JuegoDeAvion
{
    public class PixelArtRedBlueCreature
    {
        public static Bitmap Generate()
    {
        // The original image is 32x32 pixels if we consider the alien's bounding box.
        // Scaled to a smaller size for gameplay
        int scale = 2;
        int width = 32 * scale;
        int height = 32 * scale;

        Bitmap bitmap = new Bitmap(width, height);

        using (Graphics g = Graphics.FromImage(bitmap))
        {
            // Set background to transparent
            g.Clear(Color.Transparent);

            // Set high quality interpolation for better scaling if needed, though for pixel art it might not be strictly necessary
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = PixelOffsetMode.Half;

            // Define colors
            Color red = Color.FromArgb(255, 0, 0);       // Red for the body
            Color yellow = Color.FromArgb(255, 255, 0);  // Yellow for the eyes
            Color blue = Color.FromArgb(0, 0, 255);      // Blue for the legs

            using (SolidBrush redBrush = new SolidBrush(red))
            using (SolidBrush yellowBrush = new SolidBrush(yellow))
            using (SolidBrush blueBrush = new SolidBrush(blue))
            {
                // The alien's body is mostly red.
                // It can be broken down into several rectangles for efficiency.

                // Main body block (center)
                // Original pixel coordinates: x=8, y=10, width=16, height=10
                g.FillRectangle(redBrush, 8 * scale, 10 * scale, 16 * scale, 10 * scale);

                // Top "horns" or antennae (left and right)
                // Left horn: x=6, y=6, width=4, height=4
                g.FillRectangle(redBrush, 6 * scale, 6 * scale, 4 * scale, 4 * scale);
                // Right horn: x=22, y=6, width=4, height=4
                g.FillRectangle(redBrush, 22 * scale, 6 * scale, 4 * scale, 4 * scale);

                // Upper connecting blocks to horns (left and right)
                // Left connector: x=10, y=8, width=2, height=2
                g.FillRectangle(redBrush, 10 * scale, 8 * scale, 2 * scale, 2 * scale);
                // Right connector: x=20, y=8, width=2, height=2
                g.FillRectangle(redBrush, 20 * scale, 8 * scale, 2 * scale, 2 * scale);

                // Inner shoulder blocks (left and right)
                // Left shoulder: x=4, y=12, width=4, height=4
                g.FillRectangle(redBrush, 4 * scale, 12 * scale, 4 * scale, 4 * scale);
                // Right shoulder: x=24, y=12, width=4, height=4
                g.FillRectangle(redBrush, 24 * scale, 12 * scale, 4 * scale, 4 * scale);

                // The bottom part of the body, connecting to legs
                // Original pixel coordinates: x=10, y=20, width=12, height=2
                g.FillRectangle(redBrush, 10 * scale, 20 * scale, 12 * scale, 2 * scale);

                // Eyes (yellow squares)
                // Left eye: x=12, y=12, width=2, height=2
                g.FillRectangle(yellowBrush, 12 * scale, 12 * scale, 2 * scale, 2 * scale);
                // Right eye: x=18, y=12, width=2, height=2
                g.FillRectangle(yellowBrush, 18 * scale, 12 * scale, 2 * scale, 2 * scale);

                // Legs (blue)
                // Left leg (main block): x=6, y=22, width=8, height=6
                g.FillRectangle(blueBrush, 6 * scale, 22 * scale, 8 * scale, 6 * scale);
                // Right leg (main block): x=18, y=22, width=8, height=6
                g.FillRectangle(blueBrush, 18 * scale, 22 * scale, 8 * scale, 6 * scale);
            }
        }

        return bitmap;
    }
    }
}