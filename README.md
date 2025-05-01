# OCR App

A Windows desktop application that performs Optical Character Recognition (OCR) on images and PDF files using Google's Gemini 2.5 Flash model.

## Features

- Extract text from images (JPG, PNG, GIF, BMP, WEBP, TIFF)
- Extract text from PDF files
- Preview images and PDFs
- Copy extracted text to clipboard
- Save extracted text to a file
- Dark mode UI

## Requirements

- Windows operating system
- .NET 9.0 or later
- Google Gemini API key

## Setup

### Getting a Google Gemini API Key

1. Visit the [Google AI Studio](https://makersuite.google.com/app/apikey) website
2. Sign in with your Google account
3. Create a new API key
4. Copy the API key for use with the OCR App

### Configuring the API Key

There are two ways to configure your API key:

#### Option 1: Using the apikey.txt file

1. Run the application once to generate the template apikey.txt file
2. Open the apikey.txt file located in the application directory
3. Replace "YOUR_GEMINI_API_KEY_HERE" with your actual Gemini API key
4. Save the file

#### Option 2: Using an environment variable

Set the `GEMINI_API_KEY` environment variable to your Gemini API key:

**Windows Command Prompt:**
```
set GEMINI_API_KEY=your_api_key_here
```

**Windows PowerShell:**
```
$env:GEMINI_API_KEY="your_api_key_here"
```

**Permanent Environment Variable:**
1. Open System Properties (Win+Pause/Break or right-click on This PC and select Properties)
2. Click on "Advanced system settings"
3. Click on "Environment Variables"
4. Under "User variables", click "New"
5. Variable name: `GEMINI_API_KEY`
6. Variable value: your API key
7. Click OK

## Usage

1. Launch the application by running `OCRApp.exe` or using the provided batch file
2. Drag and drop an image or PDF file onto the application, or click to browse for a file
3. The application will process the file and extract text using the Gemini API
4. The extracted text will be displayed in the right panel
5. Use the buttons at the bottom to copy or save the extracted text

## Building from Source

1. Clone the repository
2. Open the solution in Visual Studio 2022 or later
3. Build the solution
4. Run the application

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- Google Gemini API for text extraction
- .NET Community for the framework and tools
