using conj_ai.Models;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Data;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("ip", configure =>
    {
        configure.PermitLimit = 2;
        configure.Window = TimeSpan.FromMinutes(1);
    });
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
else
{
    app.UseRateLimiter();
}

app.MapGet("/{verb}", async (string verb, IConfiguration config, CancellationToken ct) =>
{
    const string prompt = "Conjugate the given French verb in these tenses";
    var chat = new ChatHistory();
    var executionSettings = new OpenAIPromptExecutionSettings
    {
        ResponseFormat = typeof(MultiTenseFrenchConjugation),
        Temperature = 0,
    };
    var modelId = config["OpenAI:Model"];
    var endpoint = config["OpenAI:Endpoint"];
    var apiKey = config["OpenAI:ApiKey"];

    try
    {
        List<Exception> exceptions = [];
        if (modelId is null) exceptions.Add(new Exception("Model is not found"));
        if (endpoint is null) exceptions.Add(new Exception("Endpoint is not found"));
        if (apiKey is null) exceptions.Add(new Exception("ApiKey is not found"));
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
        var jsonContent = (await kernel.GetRequiredService<IChatCompletionService>()
            .GetChatMessageContentAsync(chat, executionSettings, kernel, ct))
            .Content ?? throw new NoNullAllowedException("JSON Content is invalid");

        var result = JsonSerializer.Deserialize<MultiTenseFrenchConjugation>(jsonContent);
        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { success = false, error = ex.Message });
    }
}).RequireRateLimiting("ip");

app.Run();