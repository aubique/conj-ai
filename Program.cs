using conj_v2.Models;
using conj_v2.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IAiService, AiService>();

var app = builder.Build();
app.UseHttpsRedirection();

app.MapGet("/{verb}", async (string verb, IAiService aiService, CancellationToken ct) =>
{
    const string prompt = $"Conjugate the given French verb in these tenses: présent and futur proche.";
    try
    {
        var response = await aiService.ChatAsync<MultiTenseFrenchConjugation>("openai/gpt-4o-mini", prompt, verb, ct);
        return Results.Ok(new { success = true, content = response.Content });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { success = false, error = ex.Message });
    }
});

app.Run();