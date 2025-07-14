using System.Diagnostics.CodeAnalysis;

namespace conj_ai.Utils;

public static class ConjugationValidators
{
    public static bool TryGetConfiguration(IConfiguration config,
        [NotNullWhen(true)] out string? modelId,
        [NotNullWhen(true)] out string? endpoint,
        [NotNullWhen(true)] out string? apiKey,
        [NotNullWhen(false)] out List<string>? missingKeys)
    {
        modelId = config["OpenAI:Model"];
        endpoint = config["OpenAI:Endpoint"];
        apiKey = config["OpenAI:ApiKey"];

        missingKeys = [];
        if (modelId is null) missingKeys.Add("OpenAI:Model");
        if (endpoint is null) missingKeys.Add("OpenAI:Endpoint");
        if (apiKey is null) missingKeys.Add("OpenAI:ApiKey");
        if (missingKeys.Count > 0) return false;

        return true;
    }
}