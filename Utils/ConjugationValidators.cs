using System.Diagnostics.CodeAnalysis;

namespace conj_ai.Utils;

public static class ConjugationValidators
{
    public static bool TryGetConfiguration(IConfiguration config,
        [NotNullWhen(true)] out string? modelId,
        [NotNullWhen(true)] out string? endpoint,
        [NotNullWhen(true)] out string? apiKey,
        [NotNullWhen(false)] out List<Exception>? exceptions)
    {
        modelId = config["OpenAI:Model"];
        endpoint = config["OpenAI:Endpoint"];
        apiKey = config["OpenAI:ApiKey"];

        exceptions = [];
        if (modelId is null) exceptions.Add(new Exception("Model is not found"));
        if (endpoint is null) exceptions.Add(new Exception("Endpoint is not found"));
        if (apiKey is null) exceptions.Add(new Exception("ApiKey is not found"));
        if (exceptions.Count > 0) return false;

        return true;
    }
}