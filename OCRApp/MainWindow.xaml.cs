using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace OCRApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Gemini API endpoint for the 1.5 Flash model
        private const string GeminiApiEndpoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash-latest:generateContent";
        
        // Current file path
        private string? _currentFilePath;
        
        // API key
        private string _apiKey;
        
        public MainWindow()
        {
            InitializeComponent();
            
            // Create a template API key file if it doesn't exist
            ApiConfig.CreateTemplateApiKeyFile();
            
            // Get the API key
            _apiKey = ApiConfig.GetApiKey();
            
            // Check if the API key is configured
            if (string.IsNullOrEmpty(_apiKey))
            {
                // Show a message to the user
                StatusText.Text = "API key not configured. Please check the README for instructions.";
                System.Windows.MessageBox.Show(
                    "API key not configured. Please edit the apikey.txt file in the application directory or set the GEMINI_API_KEY environment variable.",
                    "API Key Not Found",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
            }
        }
        
        /// <summary>
        /// Handles the DragOver event for the file drop area
        /// </summary>
        private void Border_DragOver(object sender, System.Windows.DragEventArgs e)
        {
            // Check if the dragged data contains files
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                // Get the files
                string[] files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
                
                // Check if there's at least one file and it's an image or PDF
                if (files.Length > 0 && (IsImageFile(files[0]) || PdfProcessor.IsPdfFile(files[0])))
                {
                    e.Effects = System.Windows.DragDropEffects.Copy;
                }
                else
                {
                    e.Effects = System.Windows.DragDropEffects.None;
                }
            }
            else
            {
                e.Effects = System.Windows.DragDropEffects.None;
            }
            
            e.Handled = true;
        }
        
        /// <summary>
        /// Handles the Drop event for the file drop area
        /// </summary>
        private void Border_Drop(object sender, System.Windows.DragEventArgs e)
        {
            // Check if the dropped data contains files
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                // Get the files
                string[] files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
                
                // Check if there's at least one file and it's an image or PDF
                if (files.Length > 0)
                {
                    string filePath = files[0];
                    
                    if (IsImageFile(filePath))
                    {
                        // Process the image
                        ProcessImage(filePath);
                    }
                    else if (PdfProcessor.IsPdfFile(filePath))
                    {
                        // Process the PDF
                        ProcessPdf(filePath);
                    }
                }
            }
        }
        
        /// <summary>
        /// Handles the MouseLeftButtonDown event for the file drop area
        /// </summary>
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Open file dialog to browse for an image or PDF
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image and PDF Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp;*.webp;*.tiff;*.tif;*.pdf|Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp;*.webp;*.tiff;*.tif|PDF Files|*.pdf",
                Title = "Select an Image or PDF"
            };
            
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                
                if (IsImageFile(filePath))
                {
                    // Process the image
                    ProcessImage(filePath);
                }
                else if (PdfProcessor.IsPdfFile(filePath))
                {
                    // Process the PDF
                    ProcessPdf(filePath);
                }
            }
        }
        
        /// <summary>
        /// Handles the Click event for the Copy to Clipboard button
        /// </summary>
        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            // Copy the extracted text to clipboard
            if (!string.IsNullOrEmpty(ExtractedTextBox.Text))
            {
                System.Windows.Clipboard.SetText(ExtractedTextBox.Text);
                StatusText.Text = "Text copied to clipboard";
            }
        }
        
        /// <summary>
        /// Handles the Click event for the Clear/Refresh button
        /// </summary>
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear the current file path
            _currentFilePath = null;
            
            // Reset the UI
            PreviewImage.Source = null;
            PreviewImage.Visibility = Visibility.Collapsed;
            
            DropHintText.Text = "Drag and drop an image or PDF here or click to browse";
            DropHintText.Visibility = Visibility.Visible;
            
            ExtractedTextBox.Text = string.Empty;
            StatusText.Text = "Ready";
            
            // Disable buttons
            CopyButton.IsEnabled = false;
            SaveButton.IsEnabled = false;
        }
        
        /// <summary>
        /// Handles the Click event for the Save Text button
        /// </summary>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Save the extracted text to a file
            if (!string.IsNullOrEmpty(ExtractedTextBox.Text))
            {
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Text Files|*.txt",
                    Title = "Save Extracted Text",
                    DefaultExt = "txt",
                    AddExtension = true
                };
                
                if (saveFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        File.WriteAllText(saveFileDialog.FileName, ExtractedTextBox.Text);
                        StatusText.Text = "Text saved to file";
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show($"Error saving file: {ex.Message}", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    }
                }
            }
        }
        
        /// <summary>
        /// Processes the image and extracts text using the Gemini API
        /// </summary>
        private async void ProcessImage(string imagePath)
        {
            try
            {
                // Store the current file path
                _currentFilePath = imagePath;
                
                // Update UI
                StatusText.Text = $"Processing image: {Path.GetFileName(imagePath)}";
                ExtractedTextBox.Text = "Processing...";
                CopyButton.IsEnabled = false;
                SaveButton.IsEnabled = false;
                
                // Display the image
                DisplayImage(imagePath);
                
                // Read the image file
                byte[] imageBytes = await File.ReadAllBytesAsync(imagePath);
                
                // Get the MIME type based on file extension
                string mimeType = GetMimeType(Path.GetExtension(imagePath));
                
                // Convert image to Base64
                string base64Image = Convert.ToBase64String(imageBytes);
                
                // Create the request to the Gemini API
                var request = new GeminiRequest
                {
                    Contents = new[]
                    {
                        new Content
                        {
                            Parts = new[]
                            {
                                new Part { Text = "Extract all text from this image." },
                                new Part
                                {
                                    InlineData = new InlineData
                                    {
                                        MimeType = mimeType,
                                        Data = base64Image
                                    }
                                }
                            }
                        }
                    }
                };
                
                // Send the request to the Gemini API
                StatusText.Text = "Sending request to Gemini API...";
                string extractedText = await CallGeminiApiAsync(request, _apiKey);
                
                // Update UI with the extracted text
                ExtractedTextBox.Text = extractedText;
                StatusText.Text = "Text extraction complete";
                CopyButton.IsEnabled = true;
                SaveButton.IsEnabled = true;
            }
            catch (Exception ex)
            {
                // Display error message
                ExtractedTextBox.Text = $"Error: {ex.Message}";
                StatusText.Text = "Error processing image";
                System.Windows.MessageBox.Show($"Error processing image: {ex.Message}", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
        
        /// <summary>
        /// Processes the PDF and extracts text using the Gemini API
        /// </summary>
        private async void ProcessPdf(string pdfPath)
        {
            try
            {
                // Store the current file path
                _currentFilePath = pdfPath;
                
                // Update UI
                StatusText.Text = $"Processing PDF: {Path.GetFileName(pdfPath)}";
                ExtractedTextBox.Text = "Processing...";
                CopyButton.IsEnabled = false;
                SaveButton.IsEnabled = false;
                
                // Create a preview of the first page of the PDF
                DisplayPdfPreview(pdfPath);
                
                // Read the PDF file
                byte[] pdfBytes = await File.ReadAllBytesAsync(pdfPath);
                
                // Convert PDF to Base64
                string base64Pdf = Convert.ToBase64String(pdfBytes);
                
                // Create the request to the Gemini API
                var request = new GeminiRequest
                {
                    Contents = new[]
                    {
                        new Content
                        {
                            Parts = new[]
                            {
                                new Part { Text = "Extract all text from this PDF document." },
                                new Part
                                {
                                    InlineData = new InlineData
                                    {
                                        MimeType = "application/pdf",
                                        Data = base64Pdf
                                    }
                                }
                            }
                        }
                    }
                };
                
                // Send the request to the Gemini API
                StatusText.Text = "Sending request to Gemini API...";
                string extractedText = await CallGeminiApiAsync(request, _apiKey);
                
                // Update UI with the extracted text
                ExtractedTextBox.Text = extractedText;
                StatusText.Text = "Text extraction complete";
                CopyButton.IsEnabled = true;
                SaveButton.IsEnabled = true;
            }
            catch (Exception ex)
            {
                // Display error message
                ExtractedTextBox.Text = $"Error: {ex.Message}";
                StatusText.Text = "Error processing PDF";
                System.Windows.MessageBox.Show($"Error processing PDF: {ex.Message}", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
        
        /// <summary>
        /// Displays a preview of the first page of a PDF
        /// </summary>
        private void DisplayPdfPreview(string pdfPath)
        {
            try
            {
                // Create a temporary file for the PDF preview image
                string tempImagePath = Path.Combine(Path.GetTempPath(), $"pdf_preview_{Path.GetFileNameWithoutExtension(pdfPath)}.png");
                
                // Render the first page of the PDF as an image
                PdfProcessor.RenderPdfPageAsImage(pdfPath, tempImagePath);
                
                // Display the rendered image
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(tempImagePath);
                bitmap.CacheOption = BitmapCacheOption.OnLoad; // Load the image when initialized
                bitmap.EndInit();
                
                // Set the image source and make it visible
                PreviewImage.Source = bitmap;
                PreviewImage.Visibility = Visibility.Visible;
                
                // Hide the drop hint text
                DropHintText.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                // If there's an error rendering the PDF, fall back to showing just the filename
                DropHintText.Text = $"PDF: {Path.GetFileName(pdfPath)}";
                DropHintText.Visibility = Visibility.Visible;
                PreviewImage.Visibility = Visibility.Collapsed;
                
                // Log the error but don't show it to the user
                Console.WriteLine($"Error displaying PDF preview: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Displays the image in the UI
        /// </summary>
        private void DisplayImage(string imagePath)
        {
            try
            {
                // Create a BitmapImage from the file
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imagePath);
                bitmap.CacheOption = BitmapCacheOption.OnLoad; // Load the image when initialized
                bitmap.EndInit();
                
                // Set the image source and make it visible
                PreviewImage.Source = bitmap;
                PreviewImage.Visibility = Visibility.Visible;
                
                // Hide the drop hint text
                DropHintText.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error displaying image: {ex.Message}", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
        
        /// <summary>
        /// Calls the Gemini API to extract text from the image
        /// </summary>
        private async Task<string> CallGeminiApiAsync(GeminiRequest request, string apiKey)
        {
            using var httpClient = new HttpClient();
            
            // Create the request URL with the API key
            string requestUrl = $"{GeminiApiEndpoint}?key={apiKey}";
            
            // Serialize the request to JSON
            string jsonRequest = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });
            
            // Create the HTTP request
            using var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            using var httpResponse = await httpClient.PostAsync(requestUrl, content);
            
            // Ensure the request was successful
            httpResponse.EnsureSuccessStatusCode();
            
            // Read the response
            string jsonResponse = await httpResponse.Content.ReadAsStringAsync();
            
            // Deserialize the response
            var response = JsonSerializer.Deserialize<GeminiResponse>(jsonResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            
            // Check if the response contains any candidates
            if (response?.Candidates == null || response.Candidates.Length == 0)
            {
                throw new Exception("The API response did not contain any candidates.");
            }
            
            var firstCandidate = response.Candidates[0];
            
            // Check if the response contains any error
            if (firstCandidate.Content == null || 
                firstCandidate.Content.Parts == null || 
                firstCandidate.Content.Parts.Length == 0)
            {
                if (firstCandidate.FinishReason == "SAFETY")
                {
                    throw new Exception("The request was blocked due to safety concerns.");
                }
                
                throw new Exception("The API response did not contain any content.");
            }
            
            // Extract the text from the response
            return firstCandidate.Content?.Parts?[0]?.Text ?? string.Empty;
        }
        
        /// <summary>
        /// Checks if a file is an image based on its extension
        /// </summary>
        private bool IsImageFile(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();
            return extension == ".jpg" || extension == ".jpeg" || extension == ".png" || 
                   extension == ".gif" || extension == ".bmp" || extension == ".webp" || 
                   extension == ".tiff" || extension == ".tif";
        }
        
        /// <summary>
        /// Gets the MIME type based on the file extension
        /// </summary>
        private string GetMimeType(string extension)
        {
            return extension.ToLower() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                ".tiff" or ".tif" => "image/tiff",
                _ => "application/octet-stream" // Default MIME type
            };
        }
    }
}
