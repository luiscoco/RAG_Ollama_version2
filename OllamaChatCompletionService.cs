using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Runtime.CompilerServices;
using Microsoft.SemanticKernel;

namespace VectorStoreRAG
{
    /// <summary>
    /// Implements Ollama as a chat completion service for Semantic Kernel.
    /// </summary>
    public class OllamaChatCompletionService : IChatCompletionService
    {
        private readonly HttpClient _httpClient;
        private readonly string _endpoint;
        private readonly string _model;

        public OllamaChatCompletionService(string endpoint, string model)
        {
            _httpClient = new HttpClient();
            _endpoint = endpoint.TrimEnd('/');
            _model = model;
        }

        /// <summary>
        /// Provides metadata for the service.
        /// </summary>
        public IReadOnlyDictionary<string, object> Attributes => new Dictionary<string, object>();

        /// <summary>
        /// Calls the Ollama API to generate a response.
        /// </summary>
        private async Task<string> GenerateMessageAsync(string message, CancellationToken cancellationToken = default)
        {
            var requestBody = new { prompt = message, model = _model };
            var requestJson = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            using var response = await _httpClient.PostAsync($"{_endpoint}/api/generate", content, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            var formattedResponse = new StringBuilder();
            bool lastCharIsSpace = false; // Flag to handle spacing properly

            try
            {
                using var reader = new StreamReader(responseStream);
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (string.IsNullOrWhiteSpace(line)) continue; // Skip empty lines

                    try
                    {
                        var doc = JsonDocument.Parse(line);
                        if (doc.RootElement.TryGetProperty("response", out var responseText))
                        {
                            string word = responseText.GetString() ?? "";

                            // Handle spacing issues
                            if (!lastCharIsSpace && formattedResponse.Length > 0 && !word.StartsWith(".") && !word.StartsWith(","))
                            {
                                formattedResponse.Append(" ");
                            }

                            formattedResponse.Append(word);
                            lastCharIsSpace = word.EndsWith(" ");
                        }
                    }
                    catch (JsonException)
                    {
                        Console.WriteLine($"⚠️ Warning: Skipped invalid JSON: {line}");
                    }
                }

                string finalResponse = formattedResponse.ToString().Trim();
                return string.IsNullOrEmpty(finalResponse) ? "No valid response received." : finalResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error reading Ollama response: {ex.Message}");
                return "Failed to retrieve a valid response.";
            }
        }


        /// <summary>
        /// Retrieves the assistant's response as a list of ChatMessageContent objects.
        /// </summary>
        public async Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(
            ChatHistory chatHistory,
            PromptExecutionSettings? settings = null,
            Kernel? kernel = null,
            CancellationToken cancellationToken = default)
        {
            var userMessages = new List<string>();

            // Iterate over chatHistory
            foreach (var message in chatHistory)
            {
                if (message.Role == AuthorRole.User)
                {
                    userMessages.Add(message.Content);
                }
            }

            var prompt = string.Join("\n", userMessages);
            var response = await GenerateMessageAsync(prompt, cancellationToken);

            // Return the assistant's response
            return new List<ChatMessageContent> { new ChatMessageContent(AuthorRole.Assistant, response) };
        }

        /// <summary>
        /// Provides a streaming chat response.
        /// </summary>
        public async IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(
            ChatHistory chatHistory,
            PromptExecutionSettings? settings = null,
            Kernel? kernel = null,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var messages = await GetChatMessageContentsAsync(chatHistory, settings, kernel, cancellationToken);
            foreach (var msg in messages)
            {
                yield return new StreamingChatMessageContent(msg.Role, msg.Content);
            }
        }
    }
}
