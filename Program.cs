using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();

var app = builder.Build();
app.UseHttpsRedirection();

app.MapGet("/{verb}", async (string verb, HttpClient httpClient, IConfiguration config) =>
{
    try
    {
        var apiKey = config["OpenRouter:ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
        {
            return Results.BadRequest("API Key не настроен. Запусти: dotnet user-secrets set \"OpenRouter:ApiKey\" \"твой-ключ\"");
        }

        var prompt = $"Conjugate the French verb '{verb}' in présent tense. Return only JSON with forms.";

        var requestBody = new
        {
            model = "openai/gpt-4o-mini",
            messages = new[] { new { role = "user", content = prompt } },
            max_tokens = 200
        };

        var json = System.Text.Json.JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        httpClient.DefaultRequestHeaders.Add("HTTP-Referer", "http://localhost:5000");

        var response = await httpClient.PostAsync("https://openrouter.ai/api/v1/chat/completions", content);
        var responseText = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            return Results.Ok(new { 
                success = true, 
                verb = verb,
                apiResponse = responseText 
            });
        }
        else
        {
            return Results.BadRequest(new { 
                success = false, 
                error = responseText 
            });
        }
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { 
            success = false, 
            error = ex.Message 
        });
    }
});

app.Run();