# AzureAI UiPath Toolkit - Perform STT From WAV File (Windows-Legacy)

## Overview

This project contains a custom UiPath activity for performing Speech-to-Text (STT) operations using the Microsoft Cognitive Services Speech SDK. It processes a given WAV audio file and returns the recognized text.

## Components

### 1. **SpeechToTextActivity.cs**

This file contains the `SpeechToTextActivity` class, which leverages the Microsoft Cognitive Services Speech SDK to perform STT on a provided WAV file.

#### Key Methods:
- **BeginExecute**: Starts the asynchronous speech recognition operation. It retrieves input arguments, validates them, and initiates the speech recognition task using `TaskCompletionSource<T>` to ensure compatibility with UiPath's `AsyncCodeActivity`.
- **EndExecute**: Handles the completion of the asynchronous operation. It retrieves the result or handles exceptions if any occurred.
- **PerformSpeechRecognition**: Executes the speech recognition using Azure Cognitive Services Speech SDK and returns the recognized text or an error message.
- **HandleSpeechRecognitionResult**: Interprets the Speech SDK's results and formats them for output.

## Setup and Usage

### Prerequisites
- Visual Studio (or compatible IDE)
- Microsoft Cognitive Services Speech SDK (version 1.30.0 or newer)
- .NET Framework 4.6.1 or newer
- A valid Azure subscription with access to Azure Speech Services

### Installation

1. Clone or download the repository.
2. Open the solution in Visual Studio.
3. Restore NuGet packages by right-clicking on the solution in Solution Explorer and selecting "Restore NuGet Packages."
4. Build the project to generate the necessary assemblies.

### Running the Application

1. Compile the project to create the assembly or deploy it as a custom activity in UiPath.
2. Use UiPath Studio to create a workflow that utilizes the custom `SpeechToTextActivity` activity.
3. Provide the required input arguments in UiPath Studio:
   - **SubscriptionKey**: Your Microsoft Cognitive Services subscription key.
   - **ServiceRegion**: The region where your Speech Service is hosted (e.g., `westus`).
   - **AudioFilePath**: The full path to the WAV file you want to transcribe.
   - **Locale**: The language locale for speech recognition (e.g., `en-US`).

### Output

The activity will output the result of the speech recognition process:
- **RECOGNIZED**: Displays the recognized text from the audio file.
- **NOMATCH**: Indicates that speech could not be recognized from the audio file.
- **CANCELED**: Provides details if the operation was canceled, including the reason and error details.
- **UNKNOWN ERROR**: Displays an error message if the recognition fails for unspecified reasons.

### Example

- **SubscriptionKey**: `<your_subscription_key>`
- **ServiceRegion**: `<your_service_region>`
- **AudioFilePath**: `<path_to_audio_file>`
- **Locale**: `<language_locale>`

**Speech Recognition Result**:
- **RECOGNIZED**: `Recognized Text: <recognized_text>`
- **NOMATCH**: Speech could not be recognized.
- **CANCELED**: Reason=`<cancellation_reason>`, ErrorCode=`<error_code>`, Details=`<error_details>`.
- **UNKNOWN ERROR**: Unable to process speech.

### Error Handling

- **FileNotFoundException**: If the specified audio file does not exist, an error will be thrown.
- **ArgumentException**: Thrown if invalid arguments are provided.
- **Exception**: Any other unexpected errors will be captured and returned with details.

### Notes

- Ensure that your WAV file is properly formatted for Azure Speech SDK.
- Check the Azure Speech SDK documentation for supported locales and regions.

### License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
