using System;
using System.IO;

namespace OCRApp
{
    /// <summary>
    /// Handles API configuration and key management
    /// </summary>
    public static class ApiConfig
    {
        // Path to the API key file
        private static readonly string ApiKeyFilePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "apikey.txt");
        
        /// <summary>
        /// Gets the Gemini API key from the environment variable or the apikey.txt file
        /// </summary>
        /// <returns>The API key or null if not found</returns>
        public static string GetApiKey()
        {
            // First try to get the API key from the environment variable
            string? envApiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
            if (!string.IsNullOrEmpty(envApiKey))
            {
                return envApiKey;
            }
            
            // If not found in environment variable, try to read from the apikey.txt file
            if (File.Exists(ApiKeyFilePath))
            {
                try
                {
                    string apiKey = File.ReadAllText(ApiKeyFilePath).Trim();
                    if (!string.IsNullOrEmpty(apiKey))
                    {
                        return apiKey;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading API key file: {ex.Message}");
                }
            }
            
            // Return null if API key is not found
            return null;
        }
        
        /// <summary>
        /// Checks if the API key is configured
        /// </summary>
        /// <returns>True if the API key is configured, false otherwise</returns>
        public static bool IsApiKeyConfigured()
        {
            return !string.IsNullOrEmpty(GetApiKey());
        }
        
        /// <summary>
        /// Creates a template API key file if it doesn't exist
        /// </summary>
        public static void CreateTemplateApiKeyFile()
        {
            if (!File.Exists(ApiKeyFilePath))
            {
                try
                {
                    File.WriteAllText(ApiKeyFilePath, "YOUR_GEMINI_API_KEY_HERE");
                    Console.WriteLine($"API key template file created at: {ApiKeyFilePath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating API key template file: {ex.Message}");
                }
            }
        }
    }
}
