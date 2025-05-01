using System;
using System.Text.Json.Serialization;

namespace OCRApp
{
    /// <summary>
    /// Represents a request to the Gemini API.
    /// </summary>
    public class GeminiRequest
    {
        public Content[] Contents { get; set; } = Array.Empty<Content>();
    }

    /// <summary>
    /// Represents the content of a Gemini API request.
    /// </summary>
    public class Content
    {
        public Part[] Parts { get; set; } = Array.Empty<Part>();
    }

    /// <summary>
    /// Represents a part of the content in a Gemini API request.
    /// </summary>
    public class Part
    {
        public string? Text { get; set; }
        public InlineData? InlineData { get; set; }
    }

    /// <summary>
    /// Represents inline data (e.g., an image) in a Gemini API request.
    /// </summary>
    public class InlineData
    {
        public string MimeType { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents a response from the Gemini API.
    /// </summary>
    public class GeminiResponse
    {
        public Candidate[] Candidates { get; set; } = Array.Empty<Candidate>();
    }

    /// <summary>
    /// Represents a candidate in a Gemini API response.
    /// </summary>
    public class Candidate
    {
        public Content? Content { get; set; }
        public string? FinishReason { get; set; }
    }
}
