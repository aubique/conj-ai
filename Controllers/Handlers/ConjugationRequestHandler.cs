using System.Data;
using System.Net;
using System.Text.Json;
using conj_ai.Models;
using conj_ai.Utils;
using MediatR;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace conj_ai.Controllers.Handlers;

public record ConjugationQuery<T>(string SearchTerm) : IRequest<T>;

public class ConjugationRequestHandler<T>(IConfiguration config) : IRequestHandler<ConjugationQuery<T>, T>
    where T : class, IConjugation
{
    public async Task<T> Handle(ConjugationQuery<T> request, CancellationToken ct)
    {
        var prompt = $"Conjugate the given {T.Language} verb in these tenses";
        var chat = new ChatHistory();
        var executionSettings = new OpenAIPromptExecutionSettings
        {
            ResponseFormat = typeof(T),
            Temperature = 0,
        };

        if (!ConjugationValidators.TryGetConfiguration(
            config, out var modelId, out var endpoint, out var apiKey, out var missingKeys))
        {
            throw new ConfigurationException($"Missing configuration: {string.Join(", ", missingKeys)}");
        }

        var kernel = Kernel.CreateBuilder()
            .AddOpenAIChatCompletion(modelId!, new Uri(endpoint!), apiKey)
            .Build();

        chat.AddSystemMessage(prompt);
        chat.AddUserMessage(request.SearchTerm);

        try
        {
            var jsonContent = (await kernel.GetRequiredService<IChatCompletionService>()
                .GetChatMessageContentAsync(chat, executionSettings, kernel, ct))
                .Content ?? throw new NoNullAllowedException("AI response is invalid");

            return JsonSerializer.Deserialize<T>(jsonContent)
                ?? throw new NoNullAllowedException("Cannot deserialize");
        }
        catch (HttpOperationException ex)
        {
            Console.WriteLine($"{ex.StatusCode} : {(int?)ex.StatusCode}");

            throw ex.StatusCode switch
            {
                HttpStatusCode.Unauthorized => new UnauthorizedException(ex.Message),
            };
            throw;
        }
    }
}