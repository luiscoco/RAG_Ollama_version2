// Copyright (c) Microsoft. All rights reserved.

using System.Globalization;
using Azure;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Data;
using VectorStoreRAG;
using VectorStoreRAG.Options;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

// Configure configuration and load the application configuration.
builder.Configuration.AddUserSecrets<Program>();
builder.Services.Configure<RagConfig>(builder.Configuration.GetSection(RagConfig.ConfigSectionName));
var appConfig = new ApplicationConfig(builder.Configuration);

// Create a cancellation token and source to pass to the application service to allow them
// to request a graceful application shutdown.
CancellationTokenSource appShutdownCancellationTokenSource = new();
CancellationToken appShutdownCancellationToken = appShutdownCancellationTokenSource.Token;
builder.Services.AddKeyedSingleton("AppShutdown", appShutdownCancellationTokenSource);

// ---------------------------------------------------
// ✅ Load Ollama Config from AIServices
var ollamaConfig = new OllamaOptions();
builder.Configuration.GetSection("AIServices:Ollama").Bind(ollamaConfig);

var ollamaEmbeddingsConfig = new OllamaOptions();
builder.Configuration.GetSection("AIServices:OllamaEmbeddings").Bind(ollamaEmbeddingsConfig);

// ✅ Load Ollama Vector Store Config
//var ollamaVectorStoreConfig = new OllamaVectorStoreOptions();
//builder.Configuration.GetSection("VectorStores:Ollama").Bind(ollamaVectorStoreConfig);
// ---------------------------------------------------

// Register the kernel with the dependency injection container
// and add Chat Completion and Text Embedding Generation services.
var kernelBuilder = builder.Services.AddKernel();

switch (appConfig.RagConfig.AIChatService)
{
    case "AzureOpenAI":
        kernelBuilder.AddAzureOpenAIChatCompletion(
            appConfig.AzureOpenAIConfig.ChatDeploymentName,
            appConfig.AzureOpenAIConfig.Endpoint,
            appConfig.AzureOpenAIConfig.ApiKey);
        break;
    case "OpenAI":
        kernelBuilder.AddOpenAIChatCompletion(
            appConfig.OpenAIConfig.ModelId,
            appConfig.OpenAIConfig.ApiKey,
            appConfig.OpenAIConfig.OrgId);
        break;
    case "Ollama":
        builder.Services.AddOllamaChatCompletion(ollamaConfig.Endpoint, ollamaConfig.Model);
        break;
    default:
        throw new NotSupportedException($"AI Chat Service type '{appConfig.RagConfig.AIChatService}' is not supported.");
}

switch (appConfig.RagConfig.AIEmbeddingService)
{
    case "AzureOpenAIEmbeddings":
        kernelBuilder.AddAzureOpenAITextEmbeddingGeneration(
            appConfig.AzureOpenAIEmbeddingsConfig.DeploymentName,
            appConfig.AzureOpenAIEmbeddingsConfig.Endpoint,
            appConfig.AzureOpenAIEmbeddingsConfig.ApiKey);
        break;
    case "OpenAIEmbeddings":
        kernelBuilder.AddOpenAITextEmbeddingGeneration(
            appConfig.OpenAIEmbeddingsConfig.ModelId,
            appConfig.OpenAIEmbeddingsConfig.ApiKey,
            appConfig.OpenAIEmbeddingsConfig.OrgId);
        break;
    case "OllamaEmbeddings":
        builder.Services.AddOllamaTextEmbeddingGeneration(
            ollamaEmbeddingsConfig.Endpoint,
            ollamaEmbeddingsConfig.Model);
        break;
    default:
        throw new NotSupportedException($"AI Embedding Service type '{appConfig.RagConfig.AIEmbeddingService}' is not supported.");
}

// ✅ Add the configured vector store record collection type to the dependency injection container
switch (appConfig.RagConfig.VectorStoreType)
{
    case "AzureAISearch":
        kernelBuilder.AddAzureAISearchVectorStoreRecordCollection<TextSnippet<string>>(
            appConfig.RagConfig.CollectionName,
            new Uri(appConfig.AzureAISearchConfig.Endpoint),
            new AzureKeyCredential(appConfig.AzureAISearchConfig.ApiKey));
        break;
    case "AzureCosmosDBMongoDB":
        kernelBuilder.AddAzureCosmosDBMongoDBVectorStoreRecordCollection<TextSnippet<string>>(
            appConfig.RagConfig.CollectionName,
            appConfig.AzureCosmosDBMongoDBConfig.ConnectionString,
            appConfig.AzureCosmosDBMongoDBConfig.DatabaseName);
        break;
    case "AzureCosmosDBNoSQL":
        kernelBuilder.AddAzureCosmosDBNoSQLVectorStoreRecordCollection<TextSnippet<string>>(
            appConfig.RagConfig.CollectionName,
            appConfig.AzureCosmosDBNoSQLConfig.ConnectionString,
            appConfig.AzureCosmosDBNoSQLConfig.DatabaseName);
        break;
    case "InMemory":
        kernelBuilder.AddInMemoryVectorStoreRecordCollection<string, TextSnippet<string>>(
            appConfig.RagConfig.CollectionName);
        break;
    case "Qdrant":
        kernelBuilder.AddQdrantVectorStoreRecordCollection<Guid, TextSnippet<Guid>>(
            appConfig.RagConfig.CollectionName,
            appConfig.QdrantConfig.Host,
            appConfig.QdrantConfig.Port,
            appConfig.QdrantConfig.Https,
            appConfig.QdrantConfig.ApiKey);
        break;
    case "Redis":
        kernelBuilder.AddRedisJsonVectorStoreRecordCollection<TextSnippet<string>>(
            appConfig.RagConfig.CollectionName,
            appConfig.RedisConfig.ConnectionConfiguration);
        break;
    case "Weaviate":
        kernelBuilder.AddWeaviateVectorStoreRecordCollection<TextSnippet<Guid>>(
            char.ToUpper(appConfig.RagConfig.CollectionName[0], CultureInfo.InvariantCulture) + appConfig.RagConfig.CollectionName.Substring(1),
            null,
            new() { Endpoint = new Uri(appConfig.WeaviateConfig.Endpoint) });
        break;
    //case "Ollama":  // ✅ Properly Register Ollama Vector Store
    //    builder.Services.AddOllamaVectorStoreRecordCollection<string, TextSnippet<string>>(
    //        ollamaVectorStoreConfig.CollectionName);
    //    break;
    default:
        throw new NotSupportedException($"Vector store type '{appConfig.RagConfig.VectorStoreType}' is not supported.");
}

// ✅ Register all the other required services.
switch (appConfig.RagConfig.VectorStoreType)
{
    case "AzureAISearch":
    case "AzureCosmosDBMongoDB":
    case "AzureCosmosDBNoSQL":
    case "InMemory":
    case "Redis":
        RegisterServices<string>(builder, kernelBuilder, appConfig);
        break;
    case "Ollama": // ✅ We use string-based keys for Ollama
        RegisterServices<string>(builder, kernelBuilder, appConfig);
        break;
    case "Qdrant":
    case "Weaviate":
        RegisterServices<Guid>(builder, kernelBuilder, appConfig);
        break;
    default:
        throw new NotSupportedException($"Vector store type '{appConfig.RagConfig.VectorStoreType}' is not supported.");
}

// ✅ Build and run the host.
using IHost host = builder.Build();
await host.RunAsync(appShutdownCancellationToken).ConfigureAwait(false);

// ✅ Fix RegisterServices to correctly register Vectorized Search
static void RegisterServices<TKey>(HostApplicationBuilder builder, IKernelBuilder kernelBuilder, ApplicationConfig vectorStoreRagConfig)
    where TKey : notnull
{
    // Add a text search implementation that uses the registered vector store record collection for search.
    kernelBuilder.AddVectorStoreTextSearch<TextSnippet<TKey>>(
        new TextSearchStringMapper((result) => (result as TextSnippet<TKey>)!.Text!),
        new TextSearchResultMapper((result) =>
        {
            var castResult = result as TextSnippet<TKey>;
            return new TextSearchResult(value: castResult!.Text!) { Name = castResult.ReferenceDescription, Link = castResult.ReferenceLink };
        }));

    // Add the key generator and data loader to the dependency injection container.
    builder.Services.AddSingleton<UniqueKeyGenerator<Guid>>(new UniqueKeyGenerator<Guid>(() => Guid.NewGuid()));
    builder.Services.AddSingleton<UniqueKeyGenerator<string>>(new UniqueKeyGenerator<string>(() => Guid.NewGuid().ToString()));
    builder.Services.AddSingleton<IDataLoader, DataLoader<TKey>>();

    // ✅ Register Vectorized Search
    //builder.Services.AddSingleton<IVectorizedSearch<TextSnippet<TKey>>, OllamaVectorStoreRecordCollection<TKey, TextSnippet<TKey>>>(
    //    sp => new OllamaVectorStoreRecordCollection<TKey, TextSnippet<TKey>>(vectorStoreRagConfig.RagConfig.CollectionName));

    // ✅ Add the main service for this application.
    builder.Services.AddHostedService<RAGChatService<TKey>>();
}
