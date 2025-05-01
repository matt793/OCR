using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace OCRApp
{
    /// <summary>
    /// Utility class for processing PDF files
    /// </summary>
    public static class PdfProcessor
    {
        /// <summary>
        /// Extracts text from a PDF file
        /// </summary>
        /// <param name="pdfPath">Path to the PDF file</param>
        /// <returns>Extracted text from the PDF</returns>
        public static string ExtractTextFromPdf(string pdfPath)
        {
            // Note: This method is now just a placeholder. The actual PDF text extraction
            // is done using the Gemini API in the MainWindow.cs file.
            return string.Empty;
        }
        
        /// <summary>
        /// Renders the first page of a PDF as an image
        /// </summary>
        /// <param name="pdfPath">Path to the PDF file</param>
        /// <param name="outputImagePath">Path where the image will be saved</param>
        /// <param name="dpi">DPI for the rendered image (default: 150)</param>
        /// <returns>Path to the rendered image</returns>
        public static string RenderPdfPageAsImage(string pdfPath, string outputImagePath, int dpi = 150)
        {
            try
            {
                // For now, we'll just create a simple image with text indicating it's a PDF
                using (Bitmap bitmap = new Bitmap(800, 600))
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        // Fill the background
                        g.Clear(Color.White);
                        
                        // Draw a border
                        using (Pen pen = new Pen(Color.Gray, 2))
                        {
                            g.DrawRectangle(pen, 10, 10, bitmap.Width - 20, bitmap.Height - 20);
                        }
                        
                        // Draw the PDF icon or text
                        string fileName = Path.GetFileName(pdfPath);
                        
                        // Draw the PDF preview title
                        using (System.Drawing.Font titleFont = new System.Drawing.Font("Arial", 28, FontStyle.Bold))
                        {
                            g.DrawString("PDF Preview", titleFont, Brushes.Black, 50, 50);
                        }
                        
                        // Add more space between title and filename
                        int filenameY = 150; // Increased from 100 to 150 for more spacing
                        
                        // Draw the filename with word wrapping to prevent it from running off the edge
                        using (System.Drawing.Font fileNameFont = new System.Drawing.Font("Arial", 18))
                        {
                            // Calculate the maximum width for text
                            int maxWidth = bitmap.Width - 100; // 50px margin on each side
                            
                            // Create a rectangle for the text
                            RectangleF rect = new RectangleF(50, filenameY, maxWidth, 200);
                            
                            // Draw a subtle separator line
                            using (Pen separatorPen = new Pen(Color.LightGray, 1))
                            {
                                g.DrawLine(separatorPen, 50, filenameY - 20, bitmap.Width - 50, filenameY - 20);
                            }
                            
                            // Draw the filename with word wrapping
                            using (StringFormat format = new StringFormat())
                            {
                                format.Alignment = StringAlignment.Near;
                                format.LineAlignment = StringAlignment.Near;
                                format.Trimming = StringTrimming.EllipsisCharacter;
                                
                                g.DrawString($"File: {fileName}", fileNameFont, Brushes.Black, rect, format);
                            }
                        }
                    }
                    
                    // Save the image
                    bitmap.Save(outputImagePath, ImageFormat.Png);
                }
                
                return outputImagePath;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error rendering PDF page as image: {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// Checks if a file is a PDF based on its extension
        /// </summary>
        /// <param name="filePath">Path to the file</param>
        /// <returns>True if the file is a PDF, false otherwise</returns>
        public static bool IsPdfFile(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();
            return extension == ".pdf";
        }
    }
}
