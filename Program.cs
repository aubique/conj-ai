using conj_v2.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();
app.UseHttpsRedirection();

app.MapGet("/{verb}", async (string verb, IConfiguration config, CancellationToken ct) =>
{
    const string prompt = "Conjugate the given French verb in these tenses";
    var chat = new ChatHistory();
    var executionSettings = new OpenAIPromptExecutionSettings
    {
        ResponseFormat = typeof(MultiTenseFrenchConjugation),
        Temperature = 0,
    };
    var modelId = config["OpenRouter:Model"];
    var endpoint = config["OpenRouter:BaseUrl"];
    var apiKey = config["OpenRouter:ApiKey"];

    try
    {
        List<Exception> exceptions = [];
        if (modelId is null) exceptions.Add(new Exception("Model not found"));
        if (endpoint is null) exceptions.Add(new Exception("BaseUrl not found"));
        if (apiKey is null) exceptions.Add(new Exception("ApiKey not found"));
        if (exceptions.Count > 0) throw new AggregateException(exceptions);
    }
    catch (AggregateException aggregateEx)
    {
        var errors = aggregateEx.InnerExceptions.Select(e => e.Message).ToList();
        Console.WriteLine($"Configuration missed: {string.Join(", ", errors)}");
        return Results.NotFound(new { success = false, errors });
    }

    var kernel = Kernel.CreateBuilder()
        .AddOpenAIChatCompletion(modelId!, new Uri(endpoint!), apiKey).Build();

    chat.AddSystemMessage(prompt);
    chat.AddUserMessage(verb);

    try
    {
        var response = await kernel.GetRequiredService<IChatCompletionService>()
            .GetChatMessageContentAsync(chat, executionSettings, kernel, ct);

        return Results.Ok(new { success = true, content = response.Content });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { success = false, error = ex.Message });
    }
});

app.Run();