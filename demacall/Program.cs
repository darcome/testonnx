#pragma warning disable SKEXP0001
#pragma warning disable SKEXP0003
#pragma warning disable SKEXP0010
#pragma warning disable SKEXP0011
#pragma warning disable SKEXP0020
#pragma warning disable SKEXP0050
#pragma warning disable SKEXP0052

Stopwatch sw = new ();
sw.Start ();

// system message
string systemMessage = "You are a helpful assistant. You reply in short and precise answers, and you explain your responses. If you don't know an answer, you reply 'I don't know'";

string question = "How old is Luca?";
//string question = "Are there any warnings to know when trying to remove the engine?";
//string question = "Give me some informations about the engine";

//string modelPath = @"D:\programmazione\NET\demacall\models\Phi-3.5-mini-instruct-onnx-cpu";
string modelPath = @"D:\programmazione\NET\demacall\models\Phi-3-mini-128k-instruct-onnx-directml";

// Create a chat completion service
var builder = Kernel.CreateBuilder();
builder.AddOnnxRuntimeGenAIChatCompletion(modelPath: modelPath);
builder.AddLocalTextEmbeddingGeneration();
Kernel kernel = builder.Build();
var chat = kernel.GetRequiredService<IChatCompletionService>();

ChatHistory history = new ();
history.AddSystemMessage(systemMessage);

// get the embeddings generator service
var embeddingGenerator = kernel.Services.GetRequiredService<ITextEmbeddingGenerationService>();
SqliteMemoryStore sqliteMemoryStore = await SqliteMemoryStore.ConnectAsync ("memories.db");
var memory = new SemanticTextMemory(sqliteMemoryStore, embeddingGenerator);

// add facts to the collection
const string MemoryCollectionName = "Test";

//DirectoryInfo di = new (@"D:\programmazione\NET\demacall\demacall\bin\Debug\net8.0\infos");

//foreach (FileInfo file in di.GetFiles ())
//{
//	string text = await File.ReadAllTextAsync (file.FullName);
//	string id = file.Name.Replace (file.Extension, "");

//	await memory.SaveInformationAsync
//	(
//		collection: MemoryCollectionName, 
//		id: id, 
//		text: text
//	);
//}

await memory.SaveInformationAsync
(
	collection: MemoryCollectionName,
	id: "Luca001",
	text: "Luca was born on 9th of May 1982"
);

TextMemoryPlugin memoryPlugin = new(memory);

// Import the text memory plugin into the Kernel.
kernel.ImportPluginFromObject(memoryPlugin);

OpenAIPromptExecutionSettings settings = new()
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
};

MemoryQueryResult? response = await memory.SearchAsync (MemoryCollectionName, question, limit: 1, minRelevanceScore: 0.3).FirstOrDefaultAsync ();

if (response == null)
{
	Console.WriteLine ("I don't know");
	return;
}

var prompt = @"Question: {{$input}}
    Answer the question using the memory content: {{$response}}";

history.AddUserMessage (prompt);

var arguments = new KernelArguments(settings)
{
    { "input", question },
    { "messages", history },
	{ "response", response.Metadata.Text }
};

Console.WriteLine ($"Question: {question}");
Console.WriteLine ($"Answer:");

IAsyncEnumerable<StreamingChatMessageContent> newResponse = kernel.InvokePromptStreamingAsync<StreamingChatMessageContent> (prompt, arguments);
await foreach (StreamingChatMessageContent result in newResponse)
{
	Console.Write (result.ToString ());
}

Console.WriteLine ($"");

//Console.WriteLine ($"Info used to answer the question: {response.Metadata.Id} => {response.Metadata.Text}");
Console.WriteLine ($"Info used to answer the question: {response.Metadata.Id}");

sw.Stop ();
Console.WriteLine ($"Elapsed time: {sw.ElapsedMilliseconds} ms");

Console.Read ();