# Playing with Azure Functions
This is a .NET application, experiment with various Azure Function features and provides a collection of useful links.

#### Resources
- [**Azurite emulator** for local Azure Storage development](https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite)
  - `docker run --name azurite -v azurite-volume:/data -p 10000:10000 -p 10001:10001 -p 10002:10002 -d mcr.microsoft.com/azure-storage/azurite`

- [Azure Functions documentation](https://learn.microsoft.com/en-us/azure/azure-functions) 📚
  - [Dependency Injection](https://docs.microsoft.com/en-us/azure/azure-functions/functions-dotnet-dependency-injection)
  - [host.json reference](https://docs.microsoft.com/en-us/azure/azure-functions/functions-host-json), [host.json schema](http://json.schemastore.org/host) *(schemastore.org)*
- [Azure Functions Fundamentals](https://app.pluralsight.com/library/courses/azure-functions-fundamentals) 📽️*3h40m - Pluralsight - Mark Heath*
- Custom Function Binding
  - [Creating custom Function Binding](http://dontcodetired.com/blog/post/Creating-Custom-Azure-Functions-Bindings) 📓*DoNotCodeTired*
  - [Token authentication using custom Function binding](https://www.ben-morris.com/custom-token-authentication-in-azure-functions-using-bindings) 📓*Ben Morris*
  - [ON.NET show](https://youtu.be/vKrUn9qiUI8?t=60) 📽️*21m - Matías Quaranta*
- [Blob Storage](https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-storage-blob) 📚
  - [Getting Blob Metadata](http://dontcodetired.com/blog/post/Getting-Blob-Metadata-When-Using-Azure-Functions-Blob-Storage-Triggers) 📓*DoNotCodeTired*
  - [Handling errors and poison blobs](http://dontcodetired.com/blog/post/Handling-Errors-and-Poison-Blobs-in-Azure-Functions-With-Azure-Blob-Storage-Triggers) 📓*DoNotCodeTired*
- Durable Functions 2.x 
  - [Durable Functions versions](https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-versions) 📚*overview, migration*
  - [Consuming HTTP APIs](https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-http-features#consuming-http-apis) 📚*Concept* | [Serverless HTTP With Durable Functions](https://blog.jeremylikness.com/blog/serverless-http-with-durable-functions) 📓*Jeremy Likness*
  - [Entity functions = Durable Entities](https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-entities) 📚*Concept*
  - [Developers guide to durable entities in .NET](https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-dotnet-entities) 📚*How-to guides*
- [Durable Functions](https://docs.microsoft.com/en-ie/azure/azure-functions/durable) 1.x 📚
  - [Azure Durable Functions Fundamentals](https://app.pluralsight.com/library/courses/azure-durable-functions-fundamentals) *2h53m - Pluralsight - Mark Heath*
  - Durable Functions [series](http://dontcodetired.com/blog/?tag=durfuncseries) 📓*DoNotCodeTired*
    - [Part 1](http://dontcodetired.com/blog/post/Understanding-Azure-Durable-Functions-Part-1-Overview): Overview | [Part 2](http://dontcodetired.com/blog/post/Understanding-Azure-Durable-Functions-Part-2-Creating-Your-First-Durable-Function): Creating your first Durable Function
    - [Part 3](http://dontcodetired.com/blog/post/Understanding-Azure-Durable-Functions-Part-3-What-Is-Durability): Behind the scenes | [Part 4](http://dontcodetired.com/blog/post/Understanding-Azure-Durable-Functions-Part-4-Passing-Input-To-Orchestrations-and-Activities): Passing input
    - [Part 5](http://dontcodetired.com/blog/post/Understanding-Azure-Durable-Functions-Part-5-Getting-Results-from-Orchestrations): Getting results from Orchestrations (StatusQueryGetUri)
    - [Part 6](http://dontcodetired.com/blog/post/Understanding-Azure-Durable-Functions-Part-6-Activity-Functions-with-Additional-Input-Bindings): Activity Functions with Additional Input Bindings
    - Patterns: [Part 7](http://dontcodetired.com/blog/post/Understanding-Azure-Durable-Functions-Part-7-The-Function-Chaining-Pattern): Chaining | [Part 8](http://dontcodetired.com/blog/post/Understanding-Azure-Durable-Functions-Part-8-The-Fan-OutFan-In-Pattern): Fan Out/Fan In
    - [Part 9](http://dontcodetired.com/blog/post/Understanding-Azure-Durable-Functions-Part-9-The-Asynchronous-HTTP-API-Pattern): Custom HTTP endpoint to check status
    - [Part 10](http://dontcodetired.com/blog/post/Understanding-Azure-Durable-Functions-Part-10-The-Monitor-Pattern): Monitor pattern | [Part 11](http://dontcodetired.com/blog/post/Understanding-Azure-Durable-Functions-Part-11-The-Asynchronous-Human-Interaction-Pattern): Human Interaction Pattern
  - [Call activity with retry](https://www.serverlessnotes.com/docs/retries-with-azure-durable-functions) 📓*ServerlessNotes*
  - [Advanced Serverless Workflows with Durable Functions 📽️*58m - Jeremy Likness*
  - [Building workflows with the Durable Task Framework](https://www.youtube.com/watch?v=11a4FMm5BHU) 📽️*29m - ON .NET Show*
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

- Try out the basics of Durable Entities as a new feature of Durable Functions