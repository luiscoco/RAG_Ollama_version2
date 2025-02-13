# Efficiently Searching PDF Content with AI: Leveraging LLMs and RAG for Enhanced Retrieval

This sample demonstrates how to **ingest** text from **PDF** files into a **vector store**

Also ask questions about the content using an **LLM(Large Language Model)** while using **RAG(Retrieval Augment Generate)** to supplement the LLM with additional information from the vector store

![image](https://github.com/user-attachments/assets/0b17e275-2d3d-4a52-a287-5781d7542c85)

In short, this code sets up a **Hosted Service** within a .NET Core application, suitable for a **background application** or API server that needs to **run continuously** and manage external service dependencies

A **Console Application**, on the other hand, is more suited to simpler, finite tasks that don’t require such extensive dependency management or runtime control

## 1. Configuring the Application

The sample can be configured in various ways depending on the **appsettings.json** file content:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "None"
    }
  },
  "AIServices": {
    "AzureOpenAI": {
      "Endpoint": "https://luisaiservice.openai.azure.com/",
      "ChatDeploymentName": "gpt-4o",
      "ApiKey": ""
    },
    "AzureOpenAIEmbeddings": {
      "Endpoint": "https://luisaiservice.openai.azure.com/",
      "DeploymentName": "text-embedding-ada-002",
      "ApiKey": ""
    },
    "OpenAI": {
      "ModelId": "gpt-4o",
      "ApiKey": "",
      "OrgId": null
    },
    "OpenAIEmbeddings": {
      "ModelId": "text-embedding-3-small",
      "ApiKey": "",
      "OrgId": null
    }
  },
  "VectorStores": {
    "AzureAISearch": {
      "Endpoint": "",
      "ApiKey": ""
    },
    "AzureCosmosDBMongoDB": {
      "ConnectionString": "",
      "DatabaseName": ""
    },
    "AzureCosmosDBNoSQL": {
      "ConnectionString": "",
      "DatabaseName": ""
    },
    "Qdrant": {
      "Host": "localhost",
      "Port": 6334,
      "Https": false,
      "ApiKey": ""
    },
    "Redis": {
      "ConnectionConfiguration": "localhost:6379"
    },
    "Weaviate": {
      "Endpoint": "http://localhost:8080/v1/"
    }
  },
  "Rag": {
    "AIChatService": "AzureOpenAI",
    "AIEmbeddingService": "AzureOpenAIEmbeddings",
    "BuildCollection": true,
    "CollectionName": "pdfcontent",
    "DataLoadingBatchSize": 10,
    "DataLoadingBetweenBatchDelayInMilliseconds": 1000,
    "PdfFilePaths": [ "C:\\AskAboutYourPDF-VectorStoreRAG\\BOE-A-1980-8650-consolidado.pdf" ],
    "VectorStoreType": "InMemory"
  }
}
```

### 1.1 Choose the Vector Store

Choose your preferred vector store by setting the **Rag:VectorStoreType** configuration setting in the **appsettings.json** file to one of the following values:
   
AzureAISearch
   
AzureCosmosDBMongoDB
   
AzureCosmosDBNoSQL
   
InMemory

Qdrant
   
Redis
   
Weaviate

### 1.2. Choose the AI Chat Service

You can choose your preferred AI Chat service by settings the **Rag:AIChatService** configuration setting in the **appsettings.json** file to one of the following values:
  
AzureOpenAI
   
OpenAI

### 1.3. Choose the AI Embedding Service 

You can choose your preferred AI Embedding service by settings the **Rag:AIEmbeddingService** configuration setting in the **appsettings.json** file to one of the following values:
   
AzureOpenAIEmbeddings
   
OpenAIEmbeddings

### 1.4. Load data into the vector store or Loaded data previously

You can choose whether to load data into the vector store by setting the **Rag:BuildCollection** configuration setting in the **appsettings.json** file to **true**

If you set this to **false**, the sample will assume that data was already loaded previously and it will go straight into the chat experience

### 1.5. Input the CollectionName

You can choose the name of the collection to use by setting the **Rag:CollectionName** configuration setting in the **appsettings.json** file

### 1.6. Choose the PDF File to load

You can choose the pdf file to load into the vector store by setting the **Rag:PdfFilePaths** array in the **appsettings.json** file

### 1.7. Set the number of records to process

You can choose the number of records to process per batch when loading data into the vector store by setting the **Rag:DataLoadingBatchSize** configuration setting in the **appsettings.json** file

### 1.8. Set the time to wait between batches

You can choose the number of milliseconds to wait between batches when loading data into the vector store by setting the **Rag:DataLoadingBetweenBatchDelayInMilliseconds** configuration setting in the **appsettings.json** file

## 2. Dependency Setup

To run this sample, you need to setup your source data, setup your vector store and AI services, and setup secrets for these

### 2.1. Source PDF File

You will need to supply some source **pdf files** to load into the **vector store**

Once you have a file ready, update the **PdfFilePaths** array in the **appsettings.json** file with the path to the file

```json
{
    "Rag": {
        "PdfFilePaths": [ "sourcedocument.pdf" ],
    }
}
```

Why not try the semantic kernel documentation as your input

You can download it as a PDF from the https://learn.microsoft.com/en-us/semantic-kernel/overview/ page

See the Download PDF button at the bottom of the page.

### 2.2. Azure OpenAI Chat Completion

For **Azure OpenAI Chat Completion**, you need to add the following secrets:

```cli
dotnet user-secrets set "AIServices:AzureOpenAI:Endpoint" "https://<yourservice>.openai.azure.com"
dotnet user-secrets set "AIServices:AzureOpenAI:ChatDeploymentName" "<your deployment name>"
```

Note that the code doesn't use an **API Key** to communicate with Azure Open AI, but rather an **AzureCliCredential** so no api key secret is required

### 2.3. OpenAI Chat Completion

For **OpenAI Chat Completion**, you need to add the following secrets:

```cli
dotnet user-secrets set "AIServices:OpenAI:ModelId" "<your model id>"
dotnet user-secrets set "AIServices:OpenAI:ApiKey" "<your api key>"
```

Optionally, you can also provide an **Org Id**

```cli
dotnet user-secrets set "AIServices:OpenAI:OrgId" "<your org id>"
```

### 2.4. Azure OpenAI Embeddings

For **Azure OpenAI Embeddings**, you need to add the following secrets:

```cli
dotnet user-secrets set "AIServices:AzureOpenAIEmbeddings:Endpoint" "https://<yourservice>.openai.azure.com"
dotnet user-secrets set "AIServices:AzureOpenAIEmbeddings:DeploymentName" "<your deployment name>"
```

Note that the code doesn't use an **API Key** to communicate with Azure Open AI, but rather an **AzureCliCredential** so no api key secret is required

### 2.5. OpenAI Embeddings

For **OpenAI Embeddings**, you need to add the following secrets:

```cli
dotnet user-secrets set "AIServices:OpenAIEmbeddings:ModelId" "<your model id>"
dotnet user-secrets set "AIServices:OpenAIEmbeddings:ApiKey" "<your api key>"
```

Optionally, you can also provide an **Org Id**

```cli
dotnet user-secrets set "AIServices:OpenAIEmbeddings:OrgId" "<your org id>"
```

### 2.6. Azure AI Search

If you want to use **Azure AI Search** as your **Vector Store**, you will need to create an instance of Azure AI Search and add the following **secrets** here:

```cli
dotnet user-secrets set "VectorStores:AzureAISearch:Endpoint" "https://<yourservice>.search.windows.net"
dotnet user-secrets set "VectorStores:AzureAISearch:ApiKey" "<yoursecret>"
```

### 2.7. Azure CosmosDB MongoDB

If you want to use **Azure CosmosDB MongoDB** as your **Vector Store**, you will need to create an instance of Azure CosmosDB MongoDB and add the following **secrets** here:

```cli
dotnet user-secrets set "VectorStores:AzureCosmosDBMongoDB:ConnectionString" "<yourconnectionstring>"
dotnet user-secrets set "VectorStores:AzureCosmosDBMongoDB:DatabaseName" "<yourdbname>"
```

### 2.8. Azure CosmosDB NoSQL

If you want to use **Azure CosmosDB NoSQL** as your **Vector Store**, you will need to create an instance of Azure CosmosDB NoSQL and add the following **secrets** here:

```cli
dotnet user-secrets set "VectorStores:AzureCosmosDBNoSQL:ConnectionString" "<yourconnectionstring>"
dotnet user-secrets set "VectorStores:AzureCosmosDBNoSQL:DatabaseName" "<yourdbname>"
```

### 2.9. Qdrant

If you want to use **Qdrant** as your **Vector Store**, you will need to have an instance of Qdrant available

You can use the following command to start a **Qdrant instance in Docker**, and this will work with the default configured settings:

```cli
docker run -d --name qdrant -p 6333:6333 -p 6334:6334 qdrant/qdrant:latest
```

If you want to use a **different instance of Qdrant**, you can update the **appsettings.json** file or add the following secrets to reconfigure:

```cli
dotnet user-secrets set "VectorStores:Qdrant:Host" "<yourservice>"
dotnet user-secrets set "VectorStores:Qdrant:Port" "6334"
dotnet user-secrets set "VectorStores:Qdrant:Https" "true"
dotnet user-secrets set "VectorStores:Qdrant:ApiKey" "<yoursecret>"
```

### 2.10. Redis

If you want to use **Redis** as your **Vector Store**, you will need to have an instance of Redis available

You can use the following command to start a **Redis instance in Docker**, and this will work with the default configured settings:

```cli
docker run -d --name redis-stack -p 6379:6379 -p 8001:8001 redis/redis-stack:latest
```

If you want to use a **different instance of Redis**, you can update the appsettings.json file or add the following secret to reconfigure:

```cli
dotnet user-secrets set "VectorStores:Redis:ConnectionConfiguration" "<yourredisconnectionconfiguration>"
```

### 2.11. Weaviate

If you want to use **Weaviate** as your **Vector Store**, you will need to have an instance of Weaviate available.

You can use the following command to start a **Weaviate instance in Docker**, and this will work with the default configured settings:

```cli
docker run -d --name weaviate -p 8080:8080 -p 50051:50051 cr.weaviate.io/semitechnologies/weaviate:1.26.4
```

If you want to use a **different instance of Weaviate**, you can update the **appsettings.json** file or add the following secret to reconfigure:

```cli
dotnet user-secrets set "VectorStores:Weaviate:Endpoint" "<yourweaviateurl>"
```

## 3. Application folders and files structure

![image](https://github.com/user-attachments/assets/0688ffc3-7c6b-40c0-8c52-878f688b7d51)

Looking at the structure of the VectorStoreRAG solution, here’s a brief overview of the folders and files based on common conventions in a .NET project:

### 3.1. Options Folder

This folder contains configuration classes related to different **external services**

Each of these classes likely maps to sections in the **appsettings.json** configuration file, allowing the application to read and manage settings for various service integrations

**ApplicationConfig.cs**: Probably the main configuration class that brings together various configuration settings

**AzureAISearchConfig.cs**, **AzureCosmosDBConfig.cs**, **etc**: These files define settings for specific services (Azure AI Search, CosmosDB, OpenAI, etc.), making it easier to manage configurations for multiple vector stores and AI services

**RagConfig.cs**: This might be a central configuration class that aggregates settings for the RAG (Retrieval-Augmented Generation) setup, possibly determining which AI or vector store service to use

### 3.2. Root Files

**.gitattributes** and **.gitignore**: These files are for Git configuration. .gitignore specifies files and folders that Git should ignore (e.g., build outputs, secrets), while .gitattributes manages Git’s handling of text and binary files.

**appsettings.json**: The main configuration file for the application, where most settings are defined, such as API keys, endpoints, and other configurable values

**BOE-A-1980-8650-consolidado.pdf**: This PDF file might be a document referenced in the application or for project documentation purposes

### 3.3. Data and Service Classes

**DataLoader.cs** and **IDataLoader.cs**: IDataLoader is likely an interface, while DataLoader is its implementation. These classes might be responsible for loading data into the vector store or retrieving data based on embeddings, facilitating search and retrieval operations

**TextSnippet.cs**: This class might define the data structure used for storing text snippets or records in the vector store, including fields like Text or ReferenceLink

### 3.4. Program.cs

The **main entry point** of the application. This file contains the setup code that configures the host, dependency injection, services, and other application-level configurations before running the main hosted service.

### 3.5. RAGChatService.cs

This likely defines the main hosted service in the application.

**RAGChatService** could be responsible for managing chat interactions using the Retrieval-Augmented Generation model, handling requests, generating responses, and interacting with the vector store for retrieval tasks.

### 3.6. UniqueKeyGenerator.cs

This class likely provides functionality to generate unique keys, potentially for indexing items in the vector store or assigning IDs to new records in the database.

**Summary**

This solution structure shows a typical .NET Core hosted service application, focused on a **Retrieval-Augmented Generation (RAG)** model with a vector store

Configuration files in the Options folder manage different AI and vector store settings, while core files like RAGChatService.cs and DataLoader.cs handle data management and processing

## 4. Nuget Packages

![image](https://github.com/user-attachments/assets/62beeedb-2fba-4a23-bef4-9c54b749535b)

![image](https://github.com/user-attachments/assets/7e635a04-856e-4a79-b16c-cb317d8c2e7e)

## 5. Middleware (Program.cs) explanation

**Application Configuration**: the code begins by configuring the **host application**, setting up **HostApplicationBuilder** to load user secrets and read settings from configuration files

It loads various service settings, such as those for AI services and vector storage

**Dependency Injection Setup**:

**builder.Services.Configure<RagConfig>** loads the settings from the configuration and makes them available for injection across the application

**builder.Services.AddKeyedSingleton("AppShutdown", appShutdownCancellationTokenSource);** sets up a cancellation token to manage graceful application shutdown, allowing the app to end processes smoothly when required.

**AI and Embedding Service Registration**: Based on the configuration, the code sets up either Azure OpenAI or OpenAI as the Chat and Embedding service, depending on the user’s preferences in the configuration file

The code selects the appropriate API and endpoint details for connecting to these services

**Vector Store Registration**: The code supports multiple vector storage options (e.g., Azure AI Search, CosmosDB, In-Memory, Qdrant, Redis, Weaviate)

Based on the configuration, it adds the relevant vector storage provider to the dependency injection containe

This flexibility allows the application to store and retrieve vectorized data (text embeddings) from different backends, depending on the setup

**Registering Additional Services**: It defines a **RegisterServices<TKey>** method to set up specific dependencies, like **UniqueKeyGenerator** and **DataLoader**, used by the main application

This method also registers the main **RAGChatService<TKey>**, a hosted service that **runs continuously** and handles interactions (likely for a Retrieval-Augmented Generation chat service) in the background

**Running the Application**: Finally, the **host.RunAsync(appShutdownCancellationToken).ConfigureAwait(false)**; line builds and starts the **host application**, running the service continuously until the cancellation token signals a shutdown.

Overall, this code sets up a flexible, **background service** with dependency injection and configurations for different AI and vector store providers

It’s designed to support an **AI-driven Chat Service** or similar long-running process that leverages embeddings and vector search

**Program.cs**:

```csharp
// Copyright (c) Microsoft. All rights reserved.

using System.Globalization;
using Azure;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
    default:
        throw new NotSupportedException($"AI Embedding Service type '{appConfig.RagConfig.AIEmbeddingService}' is not supported.");
}

// Add the configured vector store record collection type to the
// dependency injection container.
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
            // Weaviate collection names must start with an upper case letter.
            char.ToUpper(appConfig.RagConfig.CollectionName[0], CultureInfo.InvariantCulture) + appConfig.RagConfig.CollectionName.Substring(1),
            null,
            new() { Endpoint = new Uri(appConfig.WeaviateConfig.Endpoint) });
        break;
    default:
        throw new NotSupportedException($"Vector store type '{appConfig.RagConfig.VectorStoreType}' is not supported.");
}

// Register all the other required services.
switch (appConfig.RagConfig.VectorStoreType)
{
    case "AzureAISearch":
    case "AzureCosmosDBMongoDB":
    case "AzureCosmosDBNoSQL":
    case "InMemory":
    case "Redis":
        RegisterServices<string>(builder, kernelBuilder, appConfig);
        break;
    case "Qdrant":
    case "Weaviate":
        RegisterServices<Guid>(builder, kernelBuilder, appConfig);
        break;
    default:
        throw new NotSupportedException($"Vector store type '{appConfig.RagConfig.VectorStoreType}' is not supported.");
}

// Build and run the host.
using IHost host = builder.Build();
await host.RunAsync(appShutdownCancellationToken).ConfigureAwait(false);

static void RegisterServices<TKey>(HostApplicationBuilder builder, IKernelBuilder kernelBuilder, ApplicationConfig vectorStoreRagConfig)
    where TKey : notnull
{
    // Add a text search implementation that uses the registered vector store record collection for search.
    kernelBuilder.AddVectorStoreTextSearch<TextSnippet<TKey>>(
        new TextSearchStringMapper((result) => (result as TextSnippet<TKey>)!.Text!),
        new TextSearchResultMapper((result) =>
        {
            // Create a mapping from the Vector Store data type to the data type returned by the Text Search.
            // This text search will ultimately be used in a plugin and this TextSearchResult will be returned to the prompt template
            // when the plugin is invoked from the prompt template.
            var castResult = result as TextSnippet<TKey>;
            return new TextSearchResult(value: castResult!.Text!) { Name = castResult.ReferenceDescription, Link = castResult.ReferenceLink };
        }));

    // Add the key generator and data loader to the dependency injection container.
    builder.Services.AddSingleton<UniqueKeyGenerator<Guid>>(new UniqueKeyGenerator<Guid>(() => Guid.NewGuid()));
    builder.Services.AddSingleton<UniqueKeyGenerator<string>>(new UniqueKeyGenerator<string>(() => Guid.NewGuid().ToString()));
    builder.Services.AddSingleton<IDataLoader, DataLoader<TKey>>();

    // Add the main service for this application.
    builder.Services.AddHostedService<RAGChatService<TKey>>();
}
```

## 6. Data Loader

This class that loads text from a **PDF file into a Vector Store**

This code defines a **DataLoader** class, which loads content (text and images) from a **PDF** into a **Vector Database** (or vector store)

It uses Microsoft libraries for vector data management, embeddings, and chat completion and leverages the PdfPig library for PDF parsing

**Class Definition and Generics**: The **DataLoader** class is a generic type, where TKey represents a unique identifier for each record in the vector store

The class requires instances of a key generator, a vector store collection, an embedding generation service, and a chat completion service to function

**Loading and Processing PDFs (LoadPdf method)**:

This method takes a PDF path and parameters for batch processing, such as batch size and delay between batches

It reads text and images from the PDF in batches, converting any images to text using the chatCompletionService

For each text snippet, it generates a unique key, associates a reference to the PDF page, and generates an embedding for the text

This data is then upserted (inserted or updated) into the vector database.

**Extracting Content from PDFs (LoadTextAndImages method)**: This helper method uses **PdfPig** to extract pages and their content (text and images) from the PDF

Text is segmented into blocks and images are processed and stored along with the page number

**Retry Mechanisms**: Two methods include retry logic: **GenerateEmbeddingsWithRetryAsync** and **ConvertImageToTextWithRetryAsync**

These retries handle transient failures (e.g., rate-limiting errors indicated by TooManyRequests status) and delay before retrying up to three times

**Data Models**: RawContent is a private class that represents the content of a PDF page, storing either text or an image and the page number

Overall, this class reads and **processes PDF data to generate Embeddings and load them into a Vector Store**, enabling efficient storage and retrieval in applications like search or **Retrieval-Augmented Generation (RAG)** systems

```csharp
// Copyright (c) Microsoft. All rights reserved.

using System.Net;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Embeddings;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.DocumentLayoutAnalysis.PageSegmenter;

namespace VectorStoreRAG;

/// <summary>
/// Class that loads text from a PDF file into a vector store.
/// </summary>
/// <typeparam name="TKey">The type of the data model key.</typeparam>
/// <param name="uniqueKeyGenerator">A function to generate unique keys with.</param>
/// <param name="vectorStoreRecordCollection">The collection to load the data into.</param>
/// <param name="textEmbeddingGenerationService">The service to use for generating embeddings from the text.</param>
/// <param name="chatCompletionService">The chat completion service to use for generating text from images.</param>
internal sealed class DataLoader<TKey>(
    UniqueKeyGenerator<TKey> uniqueKeyGenerator,
    IVectorStoreRecordCollection<TKey, TextSnippet<TKey>> vectorStoreRecordCollection,
    ITextEmbeddingGenerationService textEmbeddingGenerationService,
    IChatCompletionService chatCompletionService) : IDataLoader where TKey : notnull
{
    /// <inheritdoc/>
    public async Task LoadPdf(string pdfPath, int batchSize, int betweenBatchDelayInMs, CancellationToken cancellationToken)
    {
        // Create the collection if it doesn't exist.
        await vectorStoreRecordCollection.CreateCollectionIfNotExistsAsync(cancellationToken).ConfigureAwait(false);

        // Load the text and images from the PDF file and split them into batches.
        var sections = LoadTextAndImages(pdfPath, cancellationToken);
        var batches = sections.Chunk(batchSize);

        // Process each batch of content items.
        foreach (var batch in batches)
        {
            // Convert any images to text.
            var textContentTasks = batch.Select(async content =>
            {
                if (content.Text != null)
                {
                    return content;
                }

                var textFromImage = await ConvertImageToTextWithRetryAsync(
                    chatCompletionService,
                    content.Image!.Value,
                    cancellationToken).ConfigureAwait(false);
                return new RawContent { Text = textFromImage, PageNumber = content.PageNumber };
            });
            var textContent = await Task.WhenAll(textContentTasks).ConfigureAwait(false);

            // Map each paragraph to a TextSnippet and generate an embedding for it.
            var recordTasks = textContent.Select(async content => new TextSnippet<TKey>
            {
                Key = uniqueKeyGenerator.GenerateKey(),
                Text = content.Text,
                ReferenceDescription = $"{new FileInfo(pdfPath).Name}#page={content.PageNumber}",
                ReferenceLink = $"{new Uri(new FileInfo(pdfPath).FullName).AbsoluteUri}#page={content.PageNumber}",
                TextEmbedding = await GenerateEmbeddingsWithRetryAsync(textEmbeddingGenerationService, content.Text!, cancellationToken: cancellationToken).ConfigureAwait(false)
            });

            // Upsert the records into the vector store.
            var records = await Task.WhenAll(recordTasks).ConfigureAwait(false);
            var upsertedKeys = vectorStoreRecordCollection.UpsertBatchAsync(records, cancellationToken: cancellationToken);
            await foreach (var key in upsertedKeys.ConfigureAwait(false))
            {
                Console.WriteLine($"Upserted record '{key}' into VectorDB");
            }

            await Task.Delay(betweenBatchDelayInMs, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Read the text and images from each page in the provided PDF file.
    /// </summary>
    /// <param name="pdfPath">The pdf file to read the text and images from.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for cancellation requests.</param>
    /// <returns>The text and images from the pdf file, plus the page number that each is on.</returns>
    private static IEnumerable<RawContent> LoadTextAndImages(string pdfPath, CancellationToken cancellationToken)
    {
        using (PdfDocument document = PdfDocument.Open(pdfPath))
        {
            foreach (Page page in document.GetPages())
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                foreach (var image in page.GetImages())
                {
                    if (image.TryGetPng(out var png))
                    {
                        yield return new RawContent { Image = png, PageNumber = page.Number };
                    }
                    else
                    {
                        Console.WriteLine($"Unsupported image format on page {page.Number}");
                    }
                }

                var blocks = DefaultPageSegmenter.Instance.GetBlocks(page.GetWords());
                foreach (var block in blocks)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }

                    yield return new RawContent { Text = block.Text, PageNumber = page.Number };
                }
            }
        }
    }

    /// <summary>
    /// Add a simple retry mechanism to embedding generation.
    /// </summary>
    /// <param name="textEmbeddingGenerationService">The embedding generation service.</param>
    /// <param name="text">The text to generate the embedding for.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for cancellation requests.</param>
    /// <returns>The generated embedding.</returns>
    private static async Task<ReadOnlyMemory<float>> GenerateEmbeddingsWithRetryAsync(ITextEmbeddingGenerationService textEmbeddingGenerationService, string text, CancellationToken cancellationToken)
    {
        var tries = 0;

        while (true)
        {
            try
            {
                return await textEmbeddingGenerationService.GenerateEmbeddingAsync(text, cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            catch (HttpOperationException ex) when (ex.StatusCode == HttpStatusCode.TooManyRequests)
            {
                tries++;

                if (tries < 3)
                {
                    Console.WriteLine($"Failed to generate embedding. Error: {ex}");
                    Console.WriteLine("Retrying embedding generation...");
                    await Task.Delay(10_000, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    throw;
                }
            }
        }
    }

    /// <summary>
    /// Add a simple retry mechanism to image to text.
    /// </summary>
    /// <param name="chatCompletionService">The chat completion service to use for generating text from images.</param>
    /// <param name="imageBytes">The image to generate the text for.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for cancellation requests.</param>
    /// <returns>The generated text.</returns>
    private static async Task<string> ConvertImageToTextWithRetryAsync(
        IChatCompletionService chatCompletionService,
        ReadOnlyMemory<byte> imageBytes,
        CancellationToken cancellationToken)
    {
        var tries = 0;

        while (true)
        {
            try
            {
                var chatHistory = new ChatHistory();
                chatHistory.AddUserMessage([
                    new TextContent("What’s in this image?"),
                    new ImageContent(imageBytes, "image/png"),
                ]);
                var result = await chatCompletionService.GetChatMessageContentsAsync(chatHistory, cancellationToken: cancellationToken).ConfigureAwait(false);
                return string.Join("\n", result.Select(x => x.Content));
            }
            catch (HttpOperationException ex) when (ex.StatusCode == HttpStatusCode.TooManyRequests)
            {
                tries++;

                if (tries < 3)
                {
                    Console.WriteLine($"Failed to generate text from image. Error: {ex}");
                    Console.WriteLine("Retrying text to image conversion...");
                    await Task.Delay(10_000, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    throw;
                }
            }
        }
    }

    /// <summary>
    /// Private model for returning the content items from a PDF file.
    /// </summary>
    private sealed class RawContent
    {
        public string? Text { get; init; }

        public ReadOnlyMemory<byte>? Image { get; init; }

        public int PageNumber { get; init; }
    }
}
```

## 7. RAG Chat Service

This code defines a .NET class, **RAGChatService**, which serves as the core service for a system utilizing **Retrieval-Augmented Generation (RAG)** to interact with **Large Language Models (LLMs)** and manage data using vector storage

**Imports and Namespace**:

The code includes necessary namespaces from Microsoft, focusing on dependency injection, hosting, options configuration, and various Semantic Kernel components for handling vector storage and LLM interaction.

The **RAGChatService** class is generic, allowing for different types of data model keys (TKey)

It implements IHostedService, making it compatible with .NET's generic hosting environment for long-running background services.

The **class constructor** accepts dependencies like IDataLoader (for data loading), VectorStoreTextSearch (for vector-based text searching), and Kernel (to interact with the LLM).

**StartAsync**: This method is **triggered when the Host Service Starts**. It initializes data **loading** for configured **PDFs into a Vector Store**, preparing the data for vector-based searches

Then, it begins a continuous chat loop (ChatLoopAsync) that waits for user input

**StopAsync**: Ends the service, though currently, it doesn’t perform any specific cleanup

**ChatLoopAsync**: This asynchronous method **handles user queries**. It waits for PDFs to finish loading and then prompts the user to enter questions

Upon receiving a question, it uses the **SearchPlugin** to **query** the **vector store** for relevant content

If relevant content is found, it is displayed as a response to the user. Otherwise, it provides a fallback message, or the LLM tries to answer based on general knowledge

**LoadDataAsync**: This method iteratively **loads PDFs** specified in the configuration **into the vector store**

Each PDF is processed in batches, respecting delay and batch size settings to optimize the process and handle large files

**Error Handling and Messaging**: Errors in data loading or LLM calls are captured and displayed, providing clear feedback when issues arise, such as failed PDF loading or unhandled questions

This setup allows users to **query content within PDFs** through a **chat interface**, leveraging a **vector store** to improve retrieval accuracy for large documents, and is designed for scalability and graceful shutdown within a host environment

```csharp
// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Data;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using VectorStoreRAG.Options;

namespace VectorStoreRAG;

/// <summary>
/// Main service class for the application.
/// </summary>
/// <typeparam name="TKey">The type of the data model key.</typeparam>
/// <param name="dataLoader">Used to load data into the vector store.</param>
/// <param name="vectorStoreTextSearch">Used to search the vector store.</param>
/// <param name="kernel">Used to make requests to the LLM.</param>
/// <param name="ragConfigOptions">The configuration options for the application.</param>
/// <param name="appShutdownCancellationTokenSource">Used to gracefully shut down the entire application when cancelled.</param>
internal sealed class RAGChatService<TKey>(
    IDataLoader dataLoader,
    VectorStoreTextSearch<TextSnippet<TKey>> vectorStoreTextSearch,
    Kernel kernel,
    IOptions<RagConfig> ragConfigOptions,
    [FromKeyedServices("AppShutdown")] CancellationTokenSource appShutdownCancellationTokenSource) : IHostedService
{
    private Task? _dataLoaded;
    private Task? _chatLoop;

    /// <summary>
    /// Start the service.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for cancellation requests.</param>
    /// <returns>An async task that completes when the service is started.</returns>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Start to load all the configured PDFs into the vector store.
        if (ragConfigOptions.Value.BuildCollection)
        {
            this._dataLoaded = this.LoadDataAsync(cancellationToken);
        }
        else
        {
            this._dataLoaded = Task.CompletedTask;
        }

        // Start the chat loop.
        this._chatLoop = this.ChatLoopAsync(cancellationToken);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Stop the service.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for cancellation requests.</param>
    /// <returns>An async task that completes when the service is stopped.</returns>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task ChatLoopAsync(CancellationToken cancellationToken)
    {
        var pdfFiles = string.Join(", ", ragConfigOptions.Value.PdfFilePaths ?? []);

        // Wait for the data to be loaded before starting the chat loop.
        while (this._dataLoaded != null && !this._dataLoaded.IsCompleted && !cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(1_000, cancellationToken).ConfigureAwait(false);
        }

        if (this._dataLoaded != null && this._dataLoaded.IsFaulted)
        {
            Console.WriteLine("Failed to load data");
            return;
        }

        Console.WriteLine("PDF loading complete\n");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Assistant > Press enter with no prompt to exit.");

        kernel.Plugins.Add(vectorStoreTextSearch.CreateWithGetTextSearchResults("SearchPlugin"));

        while (!cancellationToken.IsCancellationRequested)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Assistant > What would you like to know from the loaded PDFs: ({pdfFiles})?");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("User > ");
            var question = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(question))
            {
                appShutdownCancellationTokenSource.Cancel();
                break;
            }

            // Execute the vector store search plugin and check if it returned any results.
            var response = kernel.InvokePromptStreamingAsync(
                promptTemplate: """
                Please use this information to answer the question:
                {{#with (SearchPlugin-GetTextSearchResults question)}}
                    {{#if this}}
                        Please provide the full content or excerpt related to: "{{question}}".
                        {{#each this}}
                            Name: {{Name}}
                            Value: {{Value}}
                            Link: {{Link}}
                            -----------------
                        {{/each}}
                    {{else}}
                        No relevant information was found in the loaded PDFs. Please answer based on general knowledge if possible.
                    {{/if}}
                {{/with}}
                
                Question: {{question}}
            """,
                arguments: new KernelArguments()
                {
                { "question", question },
                },
                templateFormat: "handlebars",
                promptTemplateFactory: new HandlebarsPromptTemplateFactory(),
                cancellationToken: cancellationToken);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("\nAssistant > ");

            try
            {
                bool hasOutput = false;

                await foreach (var message in response.ConfigureAwait(false))
                {
                    hasOutput = true;
                    Console.Write(message);
                }

                if (!hasOutput)
                {
                    Console.WriteLine("No relevant information was found in the vector store. The assistant could not retrieve the requested content.");
                }

                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Call to LLM failed with error: {ex}");
            }
        }
    }


    /// <summary>
    /// Load all configured PDFs into the vector store.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for cancellation requests.</param>
    /// <returns>An async task that completes when the loading is complete.</returns>
    private async Task LoadDataAsync(CancellationToken cancellationToken)
    {
        try
        {
            foreach (var pdfFilePath in ragConfigOptions.Value.PdfFilePaths ?? [])
            {
                Console.WriteLine($"Loading PDF into vector store: {pdfFilePath}");
                await dataLoader.LoadPdf(
                    pdfFilePath,
                    ragConfigOptions.Value.DataLoadingBatchSize,
                    ragConfigOptions.Value.DataLoadingBetweenBatchDelayInMilliseconds,
                    cancellationToken).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load PDFs: {ex}");
            throw;
        }
    }
}
```
