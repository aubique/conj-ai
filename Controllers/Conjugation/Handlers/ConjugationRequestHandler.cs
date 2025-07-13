using System.Data;
using System.Text.Json;
using conj_ai.Models.Conjugation;
using MediatR;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace conj_ai.Controllers.Conjugation.Handlers;

public record ConjugationQuery<T>(string SearchTerm) : IRequest<T>;

public class ConjugationRequestHandler<T>(IConfiguration config) : IRequestHandler<ConjugationQuery<T>, T>
    where T : class, IConjugation
{
    public async Task<T> Handle(ConjugationQuery<T> request, CancellationToken ct)
    {
        const string prompt = "Conjugate the given French verb in these tenses";
        var chat = new ChatHistory();
        var executionSettings = new OpenAIPromptExecutionSettings
        {
            ResponseFormat = typeof(T),
            Temperature = 0,
        };

        if (!ConjugationValidators.TryGetConfiguration(
            config, out var modelId, out var endpoint, out var apiKey, out var ex))
        {
            throw new AggregateException(ex);
        }

        var kernel = Kernel.CreateBuilder()
            .AddOpenAIChatCompletion(modelId!, new Uri(endpoint!), apiKey)
            .Build();

        chat.AddSystemMessage(prompt);
        chat.AddUserMessage(request.SearchTerm);

        var jsonContent = (await kernel.GetRequiredService<IChatCompletionService>()
            .GetChatMessageContentAsync(chat, executionSettings, kernel, ct))
            .Content ?? throw new NoNullAllowedException("AI response is invalid");

        return JsonSerializer.Deserialize<T>(jsonContent) ?? throw new Exception();
    }
}
