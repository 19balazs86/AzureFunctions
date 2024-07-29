# Playing with Azure Functions
- Experiment with various Azure Function features and provides a collection of useful links.
- [2 Azure Function projects](#functions) **In-process** and **Isolated worker process**

#### Resources
- [Azure Functions documentation](https://learn.microsoft.com/en-us/azure/azure-functions/functions-overview) 📚
  - [**Guide for Isolated** Azure Functions](https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide) 📚
  - [Bindings](https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide?tabs=windows#bindings) 📚
  - [host.json reference](https://docs.microsoft.com/en-us/azure/azure-functions/functions-host-json) 📚
  - [Migration to Isolated Durable Functions](https://markheath.net/post/migrating-to-isolated-durable-functions) 📓*Mark Heath*
- [Azure Functions .NET Worker](https://github.com/Azure/azure-functions-dotnet-worker) 👤*Azure*
- [Durable Task .NET Client SDK](https://github.com/microsoft/durabletask-dotnet) 👤*Azure*
- [**Azure SDK** API reference](https://learn.microsoft.com/en-us/dotnet/api/overview/azure) 📚
- [**Azurite emulator** for local Azure Storage development](https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite)
- [Azure Functions Fundamentals](https://app.pluralsight.com/library/courses/azure-functions-fundamentals) 📽️*3h40m - Pluralsight - Mark Heath*
- [Durable Functions](https://learn.microsoft.com/en-ie/azure/azure-functions/durable/durable-functions-overview) 📚
  - [Table of APIs](https://learn.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-dotnet-isolated-overview#public-api-changes) (DurableTaskClient, TaskEntityContext...)📚
  - [Entity functions = **Durable Entities**](https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-entities) 📚*Concept*
    - [Developers guide to durable entities in .NET](https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-dotnet-entities) 📚*How-to guides*
- Custom Function Binding
  - [Creating custom Function Binding](http://dontcodetired.com/blog/post/Creating-Custom-Azure-Functions-Bindings) 📓*DoNotCodeTired*
  - [Token authentication using custom Function binding](https://www.ben-morris.com/custom-token-authentication-in-azure-functions-using-bindings) 📓*Ben Morris*
  - [ON.NET show](https://youtu.be/vKrUn9qiUI8?t=60) 📽️*21m - Matías Quaranta*
- [Blob Storage bindings](https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-storage-blob) 📚
  - [Azure Blob Storage](https://learn.microsoft.com/en-us/azure/storage/blobs/storage-blobs-introduction) 📚
  - [Getting Blob Metadata](http://dontcodetired.com/blog/post/Getting-Blob-Metadata-When-Using-Azure-Functions-Blob-Storage-Triggers) 📓*DoNotCodeTired*
  - [Handling errors and poison blobs](http://dontcodetired.com/blog/post/Handling-Errors-and-Poison-Blobs-in-Azure-Functions-With-Azure-Blob-Storage-Triggers) 📓*DoNotCodeTired*
- Test
  - [Integration testing with Testcontainers](https://www.tpeczek.com/2023/10/azure-functions-integration-testing.html) 📓*Tomasz Pęczek*
  - [Testing Azure Functions](https://microsoft.github.io/AzureTipsAndTricks/blog/tip196.html) 📓*Azure Tips and Tricks*
- Other
  - [Is Serverless really as cheap as everyone claims?](https://dev.to/azure/is-serverless-really-as-cheap-as-everyone-claims-4i9n) 📓*Burke Holland - dev.to*
  - [Links](https://github.com/19balazs86/AzureServiceBus): Processing messages/events in the given order with Azure Functions *(Jeff Hollan)*

#### Functions

###### OrderFunctions
- This class has some functions with different triggers and outputs
- TimerTrigger -> HttpTrigger -> BlobTrigger -> QueueTrigger -> Table

###### SayHelloDurableFunctions
- A simple function generated by the template with slight modification

###### EmailConfirmation
- An email confirmation process with human interaction + time out. Some extra steps just to take advantage of the features

###### EternalDurableFunc
- A simple example to use the ContinueAsNew method

###### DurableHttp
- Example: Serverless HTTP call using the new feature of Durable Functions

###### CounterFunctions
- Try out the basics of Durable Entities