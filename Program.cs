using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OCR
{
    /// <summary>
    /// A console application that performs OCR on a local image file using Google's Gemini 2.5 Flash model.
    /// 
    /// Usage:
    /// 1. The application includes a fallback API key, but you can also set your own GEMINI_API_KEY environment variable:
    ///    - Windows (Command Prompt): set GEMINI_API_KEY=your_api_key_here
    ///    - Windows (PowerShell): $env:GEMINI_API_KEY = "your_api_key_here"
    ///    - Linux/macOS: export GEMINI_API_KEY=your_api_key_here
    /// 
    /// 2. Run the application with the path to the image file as a command-line argument:
    ///    dotnet run -- "path/to/your/image.jpg"
    /// </summary>
    public class Program
    {
        // Gemini API endpoint for the 1.5 Flash model
        private const string GeminiApiEndpoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash-latest:generateContent";
        
        public static async Task Main(string[] args)
        {
            try
            {
                // Validate command-line arguments
                if (args.Length != 1)
                {
                    Console.WriteLine("Error: Please provide exactly one argument - the path to the image file.");
                    Console.WriteLine("Usage: OCR.exe \"path/to/your/image.jpg\"");
                    return;
                }

                string imagePath = args[0];

                // Validate that the file exists
                if (!File.Exists(imagePath))
                {
                    Console.WriteLine($"Error: Image file not found at '{imagePath}'");
                    return;
                }

                // Get the API key from environment variable or use the fallback key
                string? apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
                if (string.IsNullOrEmpty(apiKey))
                {
                    // Fallback to the hardcoded API key
                    apiKey = "AIzaSyB2ne6U7D9SSi2ip3tX3UUKy7Tp_WFCPp8";
                    Console.WriteLine("Using fallback API key.");
                }

                // Process the image and extract text
                await ProcessImageAsync(imagePath, apiKey);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: An unexpected error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Processes the image file and extracts text using the Gemini API.
        /// </summary>
        /// <param name="imagePath">Path to the image file</param>
        /// <param name="apiKey">Google Gemini API key</param>
        private static async Task ProcessImageAsync(string imagePath, string apiKey)
        {
            Console.WriteLine($"Reading image: {imagePath}");

            try
            {
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
                Console.WriteLine("Sending request to Gemini API...");
                string extractedText = await CallGeminiApiAsync(request, apiKey);
                
                // Print the extracted text
                Console.WriteLine("Extracted Text:");
                Console.WriteLine(extractedText);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error: Failed to read the image file: {ex.Message}");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error: API request failed. Status Code: {ex.StatusCode}. Reason: {ex.Message}");
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error: Failed to parse API response: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: An unexpected error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Calls the Gemini API to extract text from the image.
        /// </summary>
        /// <param name="request">The request object to send to the API</param>
        /// <param name="apiKey">Google Gemini API key</param>
        /// <returns>The extracted text from the image</returns>
        private static async Task<string> CallGeminiApiAsync(GeminiRequest request, string apiKey)
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
        /// Gets the MIME type based on the file extension.
        /// </summary>
        /// <param name="extension">The file extension including the dot</param>
        /// <returns>The MIME type</returns>
        private static string GetMimeType(string extension)
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
