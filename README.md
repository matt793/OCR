<div align="center">

# 🔍 OCR App

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![License](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![Windows](https://img.shields.io/badge/Platform-Windows-0078D6)](https://www.microsoft.com/windows)

**Extract text from images and PDFs with the power of Google's Gemini 2.5 Flash AI model**

![Separator](https://raw.githubusercontent.com/andreasbm/readme/master/assets/lines/rainbow.png)

</div>

## 📋 Table of Contents

- [✨ Features](#-features)
- [🚀 Quick Start](#-quick-start)
- [📋 Requirements](#-requirements)
- [🔧 Installation](#-installation)
- [🔑 API Key Setup](#-api-key-setup)
- [📝 Usage Guide](#-usage-guide)
- [🛠️ Building from Source](#️-building-from-source)
- [❓ Troubleshooting](#-troubleshooting)
- [🤝 Contributing](#-contributing)
- [📄 License](#-license)
- [🙏 Acknowledgments](#-acknowledgments)

## ✨ Features

- 📷 **Image Processing** - Extract text from various image formats:
  - JPG/JPEG
  - PNG
  - GIF
  - BMP
  - WEBP
  - TIFF
- 📑 **PDF Support** - Extract text from PDF documents
- 👁️ **Preview** - View your images and PDFs directly in the app
- 📋 **Copy to Clipboard** - One-click copying of extracted text
- 💾 **Save Results** - Export extracted text to a file
- 🌙 **Dark Mode** - Easy on the eyes UI for day and night use
- 🧠 **AI-Powered** - Utilizes Google's advanced Gemini 2.5 Flash model for accurate text recognition

## 🚀 Quick Start

1. Download the latest release
2. Get a Google Gemini API key (instructions below)
3. Run the application and enter your API key
4. Drag & drop an image or PDF file
5. View and use the extracted text!

## 📋 Requirements

Before you begin, ensure you have the following:

- 💻 Windows operating system (Windows 10 or later recommended)
- 🔄 .NET 9.0 Runtime or SDK ([Download here](https://dotnet.microsoft.com/download/dotnet/9.0))
- 🔑 Google Gemini API key (free to obtain)
- 🌐 Internet connection (for API communication)

## 🔧 Installation

### Method 1: Using the Installer (Recommended)

1. Download the latest installer from the releases page
2. Run the installer and follow the on-screen instructions
3. Launch the application from your Start menu or desktop shortcut

### Method 2: Portable Version

1. Download the latest ZIP file from the releases page
2. Extract the ZIP file to a location of your choice
3. Run `OCRApp.exe` or the included batch file to start the application

## 🔑 API Key Setup

The OCR App requires a Google Gemini API key to function. Here's how to get one:

### Getting a Google Gemini API Key

1. Visit the [Google AI Studio](https://makersuite.google.com/app/apikey) website
2. Sign in with your Google account (create one if needed)
3. Click on "Create API Key"
4. Copy your new API key for use with the OCR App

### Configuring Your API Key

There are two ways to configure your API key:

<details>
<summary><b>Option 1: Using the apikey.txt file (Recommended)</b></summary>

1. Run the application once to generate the template apikey.txt file
2. Open the apikey.txt file located in the application directory
3. Replace "YOUR_GEMINI_API_KEY_HERE" with your actual Gemini API key
4. Save the file

```
YOUR_ACTUAL_API_KEY_GOES_HERE
```

</details>

<details>
<summary><b>Option 2: Using an environment variable</b></summary>

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
1. Press `Win + R`, type `sysdm.cpl`, and press Enter
2. Go to the "Advanced" tab
3. Click "Environment Variables"
4. Under "User variables", click "New"
5. Variable name: `GEMINI_API_KEY`
6. Variable value: your API key
7. Click OK on all dialogs

</details>

## 📝 Usage Guide

### Basic Operation

1. **Launch the application** by double-clicking `OCRApp.exe` or using the provided batch file
2. **Load a file** using one of these methods:
   - Drag and drop an image or PDF file onto the application window
   - Click the "Browse" button to select a file
   - Use the File menu and select "Open"
3. **View the preview** of your document in the left panel
4. **Wait for processing** - The application will communicate with the Gemini API to extract text
5. **Review the results** displayed in the right panel
6. **Use the extracted text** with the buttons at the bottom:
   - Copy to clipboard
   - Save to a text file

### Keyboard Shortcuts

| Action | Shortcut |
|--------|----------|
| Open File | Ctrl+O |
| Save Text | Ctrl+S |
| Copy Text | Ctrl+C |
| Exit | Alt+F4 |

## 🛠️ Building from Source

If you want to build the application from source:

1. Ensure you have the following prerequisites:
   - Visual Studio 2022 or later
   - .NET 9.0 SDK
   - Git (optional, for cloning the repository)

2. Clone or download the repository:
   ```
   git clone https://github.com/yourusername/OCRApp.git
   ```

3. Open the solution file (`OCR.sln`) in Visual Studio

4. Restore NuGet packages:
   - Right-click on the solution in Solution Explorer
   - Select "Restore NuGet Packages"

5. Build the solution:
   - Select "Build" > "Build Solution" from the menu
   - Or press Ctrl+Shift+B

6. Run the application:
   - Press F5 to start debugging
   - Or Ctrl+F5 to start without debugging

## ❓ Troubleshooting

<details>
<summary><b>Application won't start</b></summary>

- Ensure you have .NET 9.0 Runtime installed
- Check Windows Event Viewer for error details
- Try running as administrator
</details>

<details>
<summary><b>API Key errors</b></summary>

- Verify your API key is correct
- Ensure the apikey.txt file is in the correct location
- Check your internet connection
- Verify the API key has not expired or been revoked
</details>

<details>
<summary><b>File processing issues</b></summary>

- Ensure the file format is supported
- Check that the file is not corrupted
- For large files, allow more time for processing
- For PDFs, ensure they are not password-protected
</details>

<details>
<summary><b>Text extraction quality issues</b></summary>

- Ensure the image is clear and text is legible
- For better results, use higher resolution images
- Some handwritten text or complex layouts may not extract perfectly
</details>

## 🤝 Contributing

Contributions are welcome! If you'd like to contribute:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🙏 Acknowledgments

- Google Gemini API for powerful text extraction capabilities
- .NET Community for the framework and tools
- All contributors and users of this application

---

<div align="center">

Made with ❤️ for the OCR community

</div>
