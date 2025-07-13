using System.Data;
using System.Text.Json;
using conj_ai.Models;
using MediatR;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace conj_ai.Controllers.Conjugation.Handlers;

public class ConjugationQuery(string searchTerm) : IRequest<MultiTenseFrenchConjugation>
{
    public string SearchTerm { get; init; } = searchTerm;
}

public class ConjugationRequestHandler(IConfiguration config) : IRequestHandler<ConjugationQuery, MultiTenseFrenchConjugation>
{
    public async Task<MultiTenseFrenchConjugation> Handle(ConjugationQuery request, CancellationToken ct)
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

        Console.WriteLine(request.SearchTerm);

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
            throw;
        }

        var kernel = Kernel.CreateBuilder()
            .AddOpenAIChatCompletion(modelId!, new Uri(endpoint!), apiKey).Build();

        chat.AddSystemMessage(prompt);
        chat.AddUserMessage(request.SearchTerm);

        try
        {
            var jsonContent = (await kernel.GetRequiredService<IChatCompletionService>()
                .GetChatMessageContentAsync(chat, executionSettings, kernel, ct))
                .Content ?? throw new NoNullAllowedException("JSON Content is invalid");

            return JsonSerializer.Deserialize<MultiTenseFrenchConjugation>(jsonContent) ?? throw new Exception();
        }
        catch (Exception)
        {
            throw;
        }
    }
}
