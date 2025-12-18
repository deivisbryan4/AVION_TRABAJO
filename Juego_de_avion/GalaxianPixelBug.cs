using System.Drawing;
using System.Drawing.Drawing2D;

namespace JuegoDeAvion
{
    public class GalaxianPixelBug
    {
        public static Bitmap Generate()
    {
        // The original image is approximately 31 pixels wide and 30 pixels high.
        // Scaled to a smaller size for gameplay
        int width = 62; 
        int height = 60; 
        Bitmap bitmap = new Bitmap(width, height);

        using (Graphics g = Graphics.FromImage(bitmap))
        {
            // Set smoother graphics interpolation mode for potentially better lines/curves if used
            g.InterpolationMode = InterpolationMode.NearestNeighbor; // To keep the pixelated feel if we were to scale up actual pixels, but we're drawing shapes.
                                                                     // For crisp lines on scaled shapes, AntiAlias might be better, but given the pixel art, NearestNeighbor is conceptually closer.
                                                                     // However, since we're drawing primitives, AntiAlias will make them smooth.
                                                                     // Let's use HighQualityBicubic for a standard rendering.
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.AntiAlias; // Smooth out the edges of drawn shapes.

            // Clear the background to transparent (alpha 0) as requested.
            g.Clear(Color.Transparent); 

            // Define colors based on the image
            Color green = Color.FromArgb(255, 0, 150, 0); // R:0, G:150, B:0
            Color red = Color.FromArgb(255, 200, 0, 0);   // R:200, G:0, B:0
            Color orange = Color.FromArgb(255, 255, 180, 0); // R:255, G:180, B:0

            using (SolidBrush greenBrush = new SolidBrush(green))
            using (SolidBrush redBrush = new SolidBrush(red))
            using (SolidBrush orangeBrush = new SolidBrush(orange))
            {
                // Define a scaling factor to translate pixel art coordinates to our larger bitmap
                float scaleX = width / 31.0f; 
                float scaleY = height / 30.0f; 

                // --- Main Body (Green) ---
                // Top antennae/head piece
                g.FillRectangle(greenBrush, 14 * scaleX, 0 * scaleY, 3 * scaleX, 2 * scaleY); // Top center antenna

                // Main body segments (approximating the irregular shape with multiple rectangles/polygons)
                PointF[] mainBody = new PointF[]
                {
                    new PointF(13 * scaleX, 2 * scaleY),
                    new PointF(18 * scaleX, 2 * scaleY),
                    new PointF(18 * scaleX, 10 * scaleY),
                    new PointF(19 * scaleX, 10 * scaleY), // Right inner tip
                    new PointF(19 * scaleX, 21 * scaleY),
                    new PointF(20 * scaleX, 21 * scaleY), // Right wing start (outer)
                    new PointF(20 * scaleX, 26 * scaleY),
                    new PointF(19 * scaleX, 26 * scaleY), // Right wing inner lower
                    new PointF(19 * scaleX, 27 * scaleY),
                    new PointF(20 * scaleX, 27 * scaleY), // Right lower-outer corner
                    new PointF(20 * scaleX, 30 * scaleY),
                    new PointF(19 * scaleX, 30 * scaleY), // Right lower-inner corner
                    new PointF(19 * scaleX, 29 * scaleY),
                    new PointF(18 * scaleX, 29 * scaleY), // Right lower-mid
                    new PointF(18 * scaleX, 28 * scaleY),
                    new PointF(17 * scaleX, 28 * scaleY),
                    new PointF(17 * scaleX, 29 * scaleY),
                    new PointF(16 * scaleX, 29 * scaleY), // Bottom center
                    new PointF(15 * scaleX, 29 * scaleY),
                    new PointF(14 * scaleX, 29 * scaleY),
                    new PointF(14 * scaleX, 28 * scaleY),
                    new PointF(13 * scaleX, 28 * scaleY),
                    new PointF(13 * scaleX, 29 * scaleY),
                    new PointF(12 * scaleX, 29 * scaleY),
                    new PointF(12 * scaleX, 30 * scaleY),
                    new PointF(11 * scaleX, 30 * scaleY), // Left lower-inner corner
                    new PointF(11 * scaleX, 27 * scaleY),
                    new PointF(10 * scaleX, 27 * scaleY), // Left lower-outer corner
                    new PointF(10 * scaleX, 26 * scaleY),
                    new PointF(11 * scaleX, 26 * scaleY), // Left wing inner lower
                    new PointF(11 * scaleX, 21 * scaleY),
                    new PointF(10 * scaleX, 21 * scaleY), // Left wing start (outer)
                    new PointF(10 * scaleX, 10 * scaleY),
                    new PointF(12 * scaleX, 10 * scaleY),
                    new PointF(12 * scaleX, 2 * scaleY)
                };
                g.FillPolygon(greenBrush, mainBody);

                // Filling in the gaps that are not perfectly covered by the polygon in the middle-top section
                g.FillRectangle(greenBrush, 12 * scaleX, 3 * scaleY, 7 * scaleX, 7 * scaleY);
                g.FillRectangle(greenBrush, 11 * scaleX, 10 * scaleY, 9 * scaleX, 11 * scaleY);

                // --- Red Details ---
                // Top 'eyes' or weapon ports
                g.FillRectangle(redBrush, 13 * scaleX, 3 * scaleY, 2 * scaleX, 2 * scaleY);
                g.FillRectangle(redBrush, 16 * scaleX, 3 * scaleY, 2 * scaleX, 2 * scaleY);

                // Lower 'legs' or weapon protrusions
                g.FillRectangle(redBrush, 14 * scaleX, 22 * scaleY, 1 * scaleX, 5 * scaleY);
                g.FillRectangle(redBrush, 16 * scaleX, 22 * scaleY, 1 * scaleX, 5 * scaleY);

                // --- Orange Central Piece ---
                g.FillRectangle(orangeBrush, 14 * scaleX, 11 * scaleY, 3 * scaleX, 6 * scaleY);
            }
        }
        return bitmap;
    }
    }
}