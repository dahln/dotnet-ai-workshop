using Azure.AI.OpenAI;
using Chat;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.ClientModel;

// Set up DI etc
var hostBuilder = Host.CreateApplicationBuilder(args);
hostBuilder.Configuration.AddUserSecrets<Program>();
hostBuilder.Services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Trace));

// Register an IChatClient
var azureOpenAiConfig = hostBuilder.Configuration.GetRequiredSection("AzureOpenAI");
var innerChatClient = new AzureOpenAIClient(new Uri(azureOpenAiConfig["Endpoint"]!), new ApiKeyCredential(azureOpenAiConfig["Key"]!))
    .GetChatClient("gpt-4o-mini")
    .AsIChatClient();

hostBuilder.Services.AddChatClient(innerChatClient)
    .UseFunctionInvocation();
    // .UsePromptBasedFunctionCalling(); // Improves function calling for models that need it

// Run the app
var app = hostBuilder.Build();
var chatClient = app.Services.GetRequiredService<IChatClient>();

// Uncomment one of these to decide which to run
await BasicCompletion.RunAsync(chatClient);
//await StructuredOutput.RunAsync(chatClient);
//await ChatBot.RunAsync(chatClient);
