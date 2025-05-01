using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace OCR
{
    /// <summary>
    /// A utility class to create a test image with text for OCR testing.
    /// </summary>
    public class CreateTestImage
    {
        public static void CreateImage()
        {
            // Create a bitmap with white background
            using (Bitmap bitmap = new Bitmap(800, 400))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    // Fill the background with white
                    graphics.Clear(Color.White);

                    // Create a font for the text
                    using (Font font = new Font("Arial", 24))
                    {
                        // Create a brush for the text
                        using (SolidBrush brush = new SolidBrush(Color.Black))
                        {
                            // Draw the text on the bitmap
                            graphics.DrawString("This is a test image for OCR.", font, brush, 50, 50);
                            graphics.DrawString("It contains multiple lines of text", font, brush, 50, 100);
                            graphics.DrawString("to test the OCR capabilities", font, brush, 50, 150);
                            graphics.DrawString("of the Google Gemini 2.5 Flash model.", font, brush, 50, 200);
                            graphics.DrawString("1234567890", font, brush, 50, 250);
                            graphics.DrawString("!@#$%^&*()", font, brush, 50, 300);
                        }
                    }
                }

                // Save the bitmap as a PNG file
                bitmap.Save("test_image.png", ImageFormat.Png);
                Console.WriteLine("Test image created: test_image.png");
            }
        }
    }
}
