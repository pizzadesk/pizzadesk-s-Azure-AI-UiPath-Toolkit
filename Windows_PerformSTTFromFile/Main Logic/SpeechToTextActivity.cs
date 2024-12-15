using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System;
using System.Activities;
using System.ComponentModel;
using System.IO;
using System.Resources;
using System.Threading.Tasks;

namespace Windows.PerformSTTFromFile
{
    [Category("pizzadesk's Azure AI UiPath Toolkit.Perform Azure AI Speech To Text From WAV File")]
    [DisplayName("Perform Speech-To-Text from WAV file")]
    [Description("Leverage Azure AI Speech Resource to transcribe speech to text from a WAV file")]
    public class SpeechToTextActivity : AsyncCodeActivity
    {
        // Define input arguments with RequiredArgument attribute
        [Category("Input")]
        [DisplayName("SubscriptionKey")]
        [Description("Azure Speech Resource subscription key")]
        [RequiredArgument]
        public InArgument<string> SubscriptionKey { get; set; }

        [Category("Input")]
        [DisplayName("ServiceRegion")]
        [Description("Region for the Azure Speech Resource (e.g. northeurope)")]
        [RequiredArgument]
        public InArgument<string> ServiceRegion { get; set; }

        [Category("Input")]
        [DisplayName("AudioFilePath")]
        [Description("Path to the WAV audio file for transcription")]
        [RequiredArgument]
        public InArgument<string> AudioFilePath { get; set; }

        [Category("Input")]
        [DisplayName("Locale")]
        [Description("Locale/language of the speech (e.g., en-US, fr-FR)")]
        [RequiredArgument]
        public InArgument<string> Locale { get; set; }

        // Define output argument
        [Category("Output")]
        [DisplayName("Result")]
        [Description("Transcription result of the audio file.")]
        public OutArgument<string> Result { get; set; }

        // ResourceManager for localized error messages
        private ResourceManager resourceManager = new ResourceManager("AzureAIToolkit.PerformSTTFromFile.Resources", typeof(SpeechToTextActivity).Assembly);

        // Begin asynchronous execution
        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            // Create a TaskCompletionSource with the correct state object
            TaskCompletionSource<string> taskCompletionSource = new TaskCompletionSource<string>(state);

            // Retrieve input arguments
            string subscriptionKey = SubscriptionKey.Get(context);
            string serviceRegion = ServiceRegion.Get(context);
            string audioFilePath = AudioFilePath.Get(context);
            string locale = Locale.Get(context);

            // Validate inputs
            ValidateInputs(subscriptionKey, serviceRegion, audioFilePath, locale);

            // Perform speech recognition asynchronously
            Task.Run(async () =>
            {
                try
                {
                    // Perform the operation and set the result
                    string result = await PerformSpeechRecognition(subscriptionKey, serviceRegion, audioFilePath, locale);
                    taskCompletionSource.SetResult(result); // Set result
                }
                catch (Exception ex)
                {
                    taskCompletionSource.SetException(ex); // Set exception if any error occurs
                }
            }).ContinueWith(_ => callback?.Invoke(taskCompletionSource.Task)); // Invoke callback when the task is complete

            // Return the Task as the IAsyncResult, with the correct state
            return taskCompletionSource.Task;
        }

        // End asynchronous execution
        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
            // Retrieve the task from IAsyncResult
            var task = (Task<string>)result;

            // Handle the result or exception
            if (task.IsFaulted && task.Exception != null)
            {
                throw task.Exception.Flatten(); // Throw the flattened exception if an error occurred
            }
            else if (task.IsCompleted)
            {
                Result.Set(context, task.Result); // Set the output argument with the result
            }
        }

        // Perform speech recognition using Azure Speech SDK
        private async Task<string> PerformSpeechRecognition(string subscriptionKey, string serviceRegion, string audioFilePath, string locale)
        {
            var speechConfig = SpeechConfig.FromSubscription(subscriptionKey, serviceRegion);
            speechConfig.SpeechRecognitionLanguage = locale;

            // Ensure file exists
            if (!File.Exists(audioFilePath))
                throw new FileNotFoundException($"The audio file at path {audioFilePath} does not exist.");

            try
            {
                // Perform speech recognition from the WAV file
                using (var audioConfig = AudioConfig.FromWavFileInput(audioFilePath))
                using (var speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig))
                {
                    var speechRecognitionResult = await speechRecognizer.RecognizeOnceAsync();
                    return HandleSpeechRecognitionResult(speechRecognitionResult);
                }
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException("Invalid argument provided: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred during speech recognition: " + ex.Message);
            }
        }

        // Handle speech recognition result
        private string HandleSpeechRecognitionResult(SpeechRecognitionResult speechRecognitionResult)
        {
            switch (speechRecognitionResult.Reason)
            {
                case ResultReason.RecognizedSpeech:
                    return $"Recognized Text: {speechRecognitionResult.Text}";
                case ResultReason.NoMatch:
                    return "No match: Speech could not be recognized.";
                case ResultReason.Canceled:
                    var cancellation = CancellationDetails.FromResult(speechRecognitionResult);
                    return $"Canceled: Reason={cancellation.Reason}, ErrorCode={cancellation.ErrorCode}, Details={cancellation.ErrorDetails}";
                default:
                    return "Unknown error occurred.";
            }
        }

        // Input validation
        private void ValidateInputs(string subscriptionKey, string serviceRegion, string audioFilePath, string locale)
        {
            if (string.IsNullOrEmpty(subscriptionKey))
                throw new ArgumentNullException(nameof(subscriptionKey), "Subscription key is required.");

            if (string.IsNullOrEmpty(serviceRegion))
                throw new ArgumentNullException(nameof(serviceRegion), "Service region is required.");

            if (string.IsNullOrEmpty(audioFilePath))
                throw new ArgumentNullException(nameof(audioFilePath), "Audio file path is required.");

            if (string.IsNullOrEmpty(locale))
                throw new ArgumentNullException(nameof(locale), "Locale is required.");
        }
    }
}
