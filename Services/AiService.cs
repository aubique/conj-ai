using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace conj_v2.Services;

public interface IAiService
{
    Task<ChatMessageContent> ChatAsync<T>(string aiModel, string systemPrompt, string userPrompt, CancellationToken cancellationToken);
}

public class AiService(IConfiguration configuration) : IAiService
{
    public async Task<ChatMessageContent> ChatAsync<T>(string aiModel, string systemPrompt, string userPrompt, CancellationToken cancellationToken)
    {
        var history = new ChatHistory();
        history.AddSystemMessage(systemPrompt);
        history.AddUserMessage(userPrompt);

        var executionSettings = new OpenAIPromptExecutionSettings
        {
            ResponseFormat = typeof(T),
            Temperature = 0,
        };

        var chatCompletionService = GetChatCompletionService(aiModel);

        return await chatCompletionService.GetChatMessageContentAsync(history, executionSettings, cancellationToken: cancellationToken);
    }

    private IChatCompletionService GetChatCompletionService(string modelName)
    {
        var token = configuration["OpenRouter:ApiKey"];

        var kernel = Kernel.CreateBuilder()
            .AddOpenAIChatCompletion(
                modelId: modelName,
                apiKey: token,
                endpoint: new Uri(configuration.GetValue("OpenRouter:BaseUrl", "")))
            .Build();

        return kernel.GetRequiredService<IChatCompletionService>();
    }
}