# Playing with Azure Functions
This is a .NET Core application to try out some Azure Function features and a collection of useful links.

#### Resources
- [Microsoft Azure documentation](https://docs.microsoft.com/en-us/azure/azure-functions/ "Microsoft Azure documentation")
  - [Dependency Injection](https://docs.microsoft.com/en-us/azure/azure-functions/functions-dotnet-dependency-injection "Dependency Injection") | [ON.NET show](https://youtu.be/LTPbaNzJd18?t=60)
  - [host.json reference](https://docs.microsoft.com/en-us/azure/azure-functions/functions-host-json "host.json reference"), [host.json schema](http://json.schemastore.org/host) *(schemastore.org)*
  - [Develop Azure Functions V3 using .NET Core 3.1](https://dev.to/azure/develop-azure-functions-using-net-core-3-0-gcm) *(Jeff Hollan)*
- Pluralsight: [Azure Functions Fundamentals](https://app.pluralsight.com/library/courses/azure-functions-fundamentals) *(Mark Heath)*
- Custom Function Binding
  - [Creating custom Function Binding](http://dontcodetired.com/blog/post/Creating-Custom-Azure-Functions-Bindings "Creating custom Function Binding") *(DoNotCodeTired)*
  - [Token authentication using custom Function binding](https://www.ben-morris.com/custom-token-authentication-in-azure-functions-using-bindings/ "Token authentication using custom Function binding") *(Ben Morris)*
  - [ON.NET show](https://youtu.be/vKrUn9qiUI8?t=60) *(Matías Quaranta)*
- [Blob Storage](https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-storage-blob "Blob Storage")
  - [Getting Blob Metadata](http://dontcodetired.com/blog/post/Getting-Blob-Metadata-When-Using-Azure-Functions-Blob-Storage-Triggers "Getting Blob Metadata") *(DoNotCodeTired)*
  - [Handling errors and poison blobs](http://dontcodetired.com/blog/post/Handling-Errors-and-Poison-Blobs-in-Azure-Functions-With-Azure-Blob-Storage-Triggers "Handling errors and poison blobs") *(DoNotCodeTired)*, [Azure doc](https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-storage-blob#trigger---poison-blobs "Azure doc")
- Durable Functions 2.x 
  - [Durable Functions versions](https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-versions) *(overview, migration)*
  - [Consuming HTTP APIs](https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-http-features#consuming-http-apis) *(concept)* | [Serverless HTTP With Durable Functions](https://blog.jeremylikness.com/blog/serverless-http-with-durable-functions) *(Jeremy Likness)*
  - [Entity functions](https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-entities) *(concept)*
  - [Developers guide to durable entities in .NET](https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-dotnet-entities) *(how-to guides)*
- [Durable Functions](https://docs.microsoft.com/en-ie/azure/azure-functions/durable/) 1.x
  - Pluralsight: [Azure Durable Functions Fundamentals](https://app.pluralsight.com/library/courses/azure-durable-functions-fundamentals) *(Mark Heath)*
  - Durable Functions [series](http://dontcodetired.com/blog/?tag=durfuncseries) *(DoNotCodeTired)*
    - [Part 1](http://dontcodetired.com/blog/post/Understanding-Azure-Durable-Functions-Part-1-Overview): Overview | [Part 2](http://dontcodetired.com/blog/post/Understanding-Azure-Durable-Functions-Part-2-Creating-Your-First-Durable-Function): Creating your first Durable Function
    - [Part 3](http://dontcodetired.com/blog/post/Understanding-Azure-Durable-Functions-Part-3-What-Is-Durability): Behind the scenes | [Part 4](http://dontcodetired.com/blog/post/Understanding-Azure-Durable-Functions-Part-4-Passing-Input-To-Orchestrations-and-Activities): Passing input
    - [Part 5](http://dontcodetired.com/blog/post/Understanding-Azure-Durable-Functions-Part-5-Getting-Results-from-Orchestrations): Getting results from Orchestrations (StatusQueryGetUri)
    - [Part 6](http://dontcodetired.com/blog/post/Understanding-Azure-Durable-Functions-Part-6-Activity-Functions-with-Additional-Input-Bindings): Activity Functions with Additional Input Bindings
    - Patterns: [Part 7](http://dontcodetired.com/blog/post/Understanding-Azure-Durable-Functions-Part-7-The-Function-Chaining-Pattern): Chaining | [Part 8](http://dontcodetired.com/blog/post/Understanding-Azure-Durable-Functions-Part-8-The-Fan-OutFan-In-Pattern): Fan Out/Fan In
    - [Part 9](http://dontcodetired.com/blog/post/Understanding-Azure-Durable-Functions-Part-9-The-Asynchronous-HTTP-API-Pattern): Custom HTTP endpoint to check status
    - [Part 10](http://dontcodetired.com/blog/post/Understanding-Azure-Durable-Functions-Part-10-The-Monitor-Pattern): Monitor pattern | [Part 11](http://dontcodetired.com/blog/post/Understanding-Azure-Durable-Functions-Part-11-The-Asynchronous-Human-Interaction-Pattern): Human Interaction Pattern
  - [Call activity with retry](https://www.serverlessnotes.com/docs/retries-with-azure-durable-functions) *(ServerlessNotes)*
  - [Presentation](https://www.youtube.com/watch?v=QvaPka0lmdU): Advanced Serverless Workflows with Durable Functions *(Jeremy Likness)*
  - ON .NET Show: [Building workflows with the Durable Task Framework](https://www.youtube.com/watch?v=11a4FMm5BHU)
- Testing Azure Functions
  - [Azure Tips and Tricks](https://microsoft.github.io/AzureTipsAndTricks/blog/tip196.html "Azure Tips and Tricks")
- Other
  - [Is Serverless really as cheap as everyone claims?](https://dev.to/azure/is-serverless-really-as-cheap-as-everyone-claims-4i9n) *(Burke Holland - dev.to)*
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