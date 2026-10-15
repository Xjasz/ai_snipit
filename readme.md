# AI Snipit

## Overview

Snipit is a Windows application that allows users to capture screen snippets, extract text using Tesseract OCR, and optionally send the snip image plus a typed question to OpenAI's GPT-4o when an API key is set. The application provides a user-friendly overlay for selecting screen areas and can save extracted text for further use.

## Features

- **Screen Snippet Capture**: Select an area of the screen to capture.
- **Tesseract OCR Integration**: Extract text from captured images using Tesseract.
- **ChatGPT Integration**: Optionally send the snip image and your question to GPT-4o.
- **Live Extraction Mode**: Enables continuous text extraction when enabled.
- **User-Friendly UI**: Simple start/stop mechanism with an interactive response display.

## Requirements

- **Windows OS** (Tested on Windows 10/11)
- **.NET 7 SDK** (`net7.0-windows`, see `Snipit.csproj`) and Visual Studio 2022
- Nothing to install for OCR: the Tesseract 5.2 NuGet package and the English `tessdata` ship with the repo
- **OpenAI API key** in the `OPENAI_API_KEY` environment variable (optional, for the ChatGPT step)

## Installation

1. Clone the repository:
   ```sh
   git clone https://github.com/Xjasz/ai_snipit.git
   cd ai_snipit
   ```
2. Open `Snipit.sln` in Visual Studio.
3. Let Visual Studio restore the NuGet packages.
4. Build and run the project.

## Usage

1. **Start the application**:
   - Click the `Start` button to enable snippet capture.
2. **Capture a snippet**:
   - Click the `Snip It` button or press `Ctrl+/`.
   - Select the desired area of the screen.
3. **Extract text**:
   - Enable `Text Extraction` to use Tesseract OCR.
   - Enable `ChatGPT Extraction` (optional) if you have an OpenAI API key.
4. **View results**:
   - Extracted text appears in the response panel.
   - If ChatGPT is enabled, it will analyze the text and return a response.

## Configuration

- **Tesseract**: bundled; `tesseract_files/tessdata` is copied next to the executable at build time. English only.
- **ChatGPT API key**: set the `OPENAI_API_KEY` environment variable before starting Snipit. The key is never stored in the source.
- **Saved files**: every snip (`.png`), its OCR text and the GPT response are written under `%USERPROFILE%\.snipit` and are not deleted automatically.

## Project Structure

```
ai_snipit/
├── Snipit.sln                 # Solution file
├── readme.md                  # Project documentation
└── Snipit/
    ├── Snipit.csproj          # Project configuration (NuGet: Tesseract, Newtonsoft.Json)
    ├── Program.cs             # Hotkey hook, screenshot, OCR
    ├── MainForm.cs            # UI and the OpenAI request
    ├── OverlayForm.cs         # Overlay for selecting screen snippets
    └── tesseract_files/       # Bundled tessdata
```

## Dependencies

- **Tesseract OCR**: Used for extracting text from images.
- **OpenAI API**: For optional ChatGPT integration.
- **.NET 7 (Windows)**: Required for running the application.

## Future Enhancements

- Improve OCR accuracy with better preprocessing.
- Add support for additional languages.
- Enhance UI for better usability.

## License

This project is licensed under the MIT License.

## Contact

For questions, reach out via GitHub Issues or email at `codalata@gmail.com`.



