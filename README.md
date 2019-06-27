# Playing with Azure Functions
This is a .NET Core application to try out some Azure Function features and a collection of useful links.

#### Resources
- [Microsoft Azure documentation](https://docs.microsoft.com/en-us/azure/azure-functions/ "Microsoft Azure documentation").
- [Creating custom Azure Functions Bindings](http://dontcodetired.com/blog/post/Creating-Custom-Azure-Functions-Bindings "Creating custom Azure Functions Bindings") *(DoNotCodeTired)*.
- [host.json reference](https://docs.microsoft.com/en-us/azure/azure-functions/functions-host-json "host.json reference").
- [Blob Storage](https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-storage-blob "Blob Storage")
 - [Getting Blob Metadata](http://dontcodetired.com/blog/post/Getting-Blob-Metadata-When-Using-Azure-Functions-Blob-Storage-Triggers "Getting Blob Metadata") *(DoNotCodeTired)*.
 - [Handling errors and poison blobs](http://dontcodetired.com/blog/post/Handling-Errors-and-Poison-Blobs-in-Azure-Functions-With-Azure-Blob-Storage-Triggers "Handling errors and poison blobs") *(DoNotCodeTired)*, [Azure doc](https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-storage-blob#trigger---poison-blobs "Azure doc").
- Testing Azure Functions
 - [Azure Tips and Tricks](https://microsoft.github.io/AzureTipsAndTricks/blog/tip196.html "Azure Tips and Tricks").

#### OrderFunctions
- This class has some functions with different triggers and outputs changing together.
- TimerTrigger -> HttpTrigger -> BlobTrigger -> QueueTrigger -> Table