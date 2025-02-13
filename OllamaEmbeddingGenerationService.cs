using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel;
using Microsoft.Extensions.Hosting;

//VERY IMPORTANT Testing EndPoint for Ollama phi3 embedding service: 
//curl - X POST http://localhost:11434/api/embeddings -H "Content-Type: application/json" -d "{ \"model\": \"phi3\", \"prompt\": \"The quick brown fox jumps over the lazy dog.\" }"

namespace VectorStoreRAG
{
    /// <summary>
    /// Implements Ollama for text embeddings in Semantic Kernel.
    /// </summary>
    public class OllamaEmbeddingGenerationService : IEmbeddingGenerationService<string, float>, ITextEmbeddingGenerationService
    {
        private readonly HttpClient _httpClient;
        private readonly string _endpoint;
        private readonly string _model;

        public OllamaEmbeddingGenerationService(string endpoint, string model)
        {
            _httpClient = new HttpClient();
            _endpoint = endpoint.TrimEnd('/');
            _model = model;
        }

        /// <summary>
        /// Provides metadata for the AI service.
        /// </summary>
        public IReadOnlyDictionary<string, object> Attributes { get; } = new Dictionary<string, object>
        {
            { "Provider", "Ollama" },
            { "Model", "phi3" } // Updated model name
        };

        /// <summary>
        /// Generates an embedding for a single text input.
        /// </summary>
        public async Task<ReadOnlyMemory<float>> GenerateEmbeddingAsync(
            string text,
            CancellationToken cancellationToken = default)
        {
            // Create request payload (using "prompt" instead of "input")
            var requestBody = new
            {
                model = _model,
                prompt = text  // ✅ Adjusted field name to match working curl command
            };

            var requestJson = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            // ✅ Updated API endpoint for embeddings
            var requestUrl = $"{_endpoint}/api/embeddings";

            using var response = await _httpClient.PostAsync(requestUrl, content, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Ollama API failed: {response.StatusCode}");
            }

            var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

            try
            {
                var doc = JsonDocument.Parse(responseString);
                // ✅ Adjusted to match expected JSON response for embeddings
                if (doc.RootElement.TryGetProperty("embedding", out var embeddingJson))
                {
                    var floats = embeddingJson.EnumerateArray().Select(x => x.GetSingle()).ToArray();
                    return floats;
                }
                else
                {
                    throw new JsonException("No 'embedding' property found in the response.");
                }
            }
            catch (JsonException ex)
            {
                throw new JsonException($"Failed to parse JSON response: {ex.Message}");
            }
        }

        /// <summary>
        /// Generates embeddings for multiple text inputs.
        /// </summary>
        public async Task<IList<ReadOnlyMemory<float>>> GenerateEmbeddingsAsync(
            IList<string> texts,
            Kernel? kernel = null,
            CancellationToken cancellationToken = default)
        {
            var tasks = texts.Select(text => GenerateEmbeddingAsync(text, cancellationToken));
            return await Task.WhenAll(tasks);
        }
    }
}
