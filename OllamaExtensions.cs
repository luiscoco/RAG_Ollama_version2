using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.Data;
using Microsoft.Extensions.VectorData;

namespace VectorStoreRAG;

/// <summary>
/// Extension methods to register Ollama-based AI services.
/// </summary>
public static class OllamaExtensions
{
    /// <summary>
    /// Registers Ollama chat completion service.
    /// </summary>
    public static IServiceCollection AddOllamaChatCompletion(
        this IServiceCollection services,
        string endpoint,
        string model)
    {
        services.AddSingleton<IChatCompletionService>(sp =>
            new OllamaChatCompletionService(endpoint, model));

        return services;
    }

    public static IServiceCollection AddOllamaTextEmbeddingGeneration(
        this IServiceCollection services,
        string endpoint,
        string model)
    {
        var serviceInstance = new OllamaEmbeddingGenerationService(endpoint, model);

        services.AddSingleton<IEmbeddingGenerationService<string, float>>(serviceInstance);

        services.AddSingleton<ITextEmbeddingGenerationService>(sp =>
            sp.GetRequiredService<IEmbeddingGenerationService<string, float>>() as ITextEmbeddingGenerationService
            ?? throw new InvalidOperationException("Failed to register ITextEmbeddingGenerationService"));

        return services;
    }


    /// <summary>
    /// Registers the custom Ollama vector store record collection.
    /// </summary>
    //public static IServiceCollection AddOllamaVectorStoreRecordCollection<TKey, TValue>(
    //    this IServiceCollection services,
    //    string collectionName)
    //    where TKey : notnull
    //    where TValue : class
    //{
    //    services.AddSingleton<IVectorStoreRecordCollection<TKey, TValue>>(sp =>
    //        new OllamaVectorStoreRecordCollection<TKey, TValue>(collectionName));

    //    services.AddSingleton<IVectorizedSearch<TValue>>(sp =>
    //        sp.GetRequiredService<IVectorStoreRecordCollection<TKey, TValue>>() as IVectorizedSearch<TValue> ??
    //        throw new InvalidOperationException("Failed to register IVectorizedSearch"));

    //    return services;
    //}
}
