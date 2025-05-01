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

namespace OCR
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Gemini API endpoint for the 2.5 Flash Preview model
        private const string GeminiApiEndpoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash-preview-04-17:generateContent";
        
        // API key
        private string _apiKey = "AIzaSyB2ne6U7D9SSi2ip3tX3UUKy7Tp_WFCPp8"; // Fallback API key
        
        // Current image path
        private string? _currentImagePath;
        
        public MainWindow()
        {
            InitializeComponent();
            
            // Try to get API key from environment variable
            string? envApiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
            if (!string.IsNullOrEmpty(envApiKey))
            {
                _apiKey = envApiKey;
            }
        }
        
        /// <summary>
        /// Handles the DragOver event for the image drop area
        /// </summary>
        private void Border_DragOver(object sender, DragEventArgs e)
        {
            // Check if the dragged data contains files
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Get the files
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                
                // Check if there's at least one file and it's an image
                if (files.Length > 0 && IsImageFile(files[0]))
                {
                    e.Effects = DragDropEffects.Copy;
                }
                else
                {
                    e.Effects = DragDropEffects.None;
                }
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            
            e.Handled = true;
        }
        
        /// <summary>
        /// Handles the Drop event for the image drop area
        /// </summary>
        private void Border_Drop(object sender, DragEventArgs e)
        {
            // Check if the dropped data contains files
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Get the files
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                
                // Check if there's at least one file and it's an image
                if (files.Length > 0 && IsImageFile(files[0]))
                {
                    // Process the image
                    ProcessImage(files[0]);
                }
            }
        }
        
        /// <summary>
        /// Handles the MouseLeftButtonDown event for the image drop area
        /// </summary>
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Open file dialog to browse for an image
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp;*.webp;*.tiff;*.tif",
                Title = "Select an Image"
            };
            
            if (openFileDialog.ShowDialog() == true)
            {
                // Process the selected image
                ProcessImage(openFileDialog.FileName);
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
                Clipboard.SetText(ExtractedTextBox.Text);
                StatusText.Text = "Text copied to clipboard";
            }
        }
        
        /// <summary>
        /// Handles the Click event for the Save Text button
        /// </summary>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Save the extracted text to a file
            if (!string.IsNullOrEmpty(ExtractedTextBox.Text))
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
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
                        MessageBox.Show($"Error saving file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                // Store the current image path
                _currentImagePath = imagePath;
                
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
                MessageBox.Show($"Error processing image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show($"Error displaying image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
    
    // API models moved to GeminiApiModels.cs
}
