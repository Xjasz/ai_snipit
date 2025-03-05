# AI Snipit

## Overview

Snipit is a Windows application that allows users to capture screen snippets, extract text using Tesseract OCR, and optionally process the extracted text with OpenAI's ChatGPT if an API key is provided. The application provides a user-friendly overlay for selecting screen areas and can save extracted text for further use.

## Features

- **Screen Snippet Capture**: Select an area of the screen to capture.
- **Tesseract OCR Integration**: Extract text from captured images using Tesseract.
- **ChatGPT Integration**: Optionally send extracted text to ChatGPT for further analysis.
- **Live Extraction Mode**: Enables continuous text extraction when enabled.
- **User-Friendly UI**: Simple start/stop mechanism with an interactive response display.

## Requirements

- **Windows OS** (Tested on Windows 10/11)
- **.NET Framework** (Version required as per `Snipit.csproj`)
- **Tesseract OCR** installed locally with necessary language data
- **OpenAI API Key** (optional, for ChatGPT integration)

## Installation

1. Clone the repository:
   ```sh
   git clone https://github.com/yourusername/snipit.git
   cd snipit
   ```
2. Open `Snipit.sln` in Visual Studio.
3. Ensure that dependencies are installed (Tesseract OCR, required NuGet packages).
4. Build and run the project.

## Usage

1. **Start the application**:
   - Click the `Start` button to enable snippet capture.
2. **Capture a snippet**:
   - Click the `Snip It` button or use the shortcut key.
   - Select the desired area of the screen.
3. **Extract text**:
   - Enable `Text Extraction` to use Tesseract OCR.
   - Enable `ChatGPT Extraction` (optional) if you have an OpenAI API key.
4. **View results**:
   - Extracted text appears in the response panel.
   - If ChatGPT is enabled, it will analyze the text and return a response.

## Configuration

- **Tesseract Setup**:
  - Ensure `tesseract.exe` is installed and `tessdata` directory is accessible.
  - Update paths in `Program.cs` if needed.
- **ChatGPT API Key**:
  - Replace `APIKEY` in `MainForm.cs` with your actual API key.

## Project Structure

```
Snipit/
├── Snipit.sln           # Solution file
├── Snipit.csproj        # Project configuration
├── Program.cs           # Main application logic
├── MainForm.cs          # UI and event handlers
├── OverlayForm.cs       # Overlay for selecting screen snippets
├── packages.config      # NuGet dependencies
└── README.md            # Project documentation
```

## Dependencies

- **Tesseract OCR**: Used for extracting text from images.
- **OpenAI API**: For optional ChatGPT integration.
- **.NET Framework**: Required for running the application.

## Future Enhancements

- Improve OCR accuracy with better preprocessing.
- Add support for additional languages.
- Enhance UI for better usability.

## License

This project is licensed under the MIT License.

## Contact

For questions, reach out via GitHub Issues or email at `codalata@gmail.com`.



