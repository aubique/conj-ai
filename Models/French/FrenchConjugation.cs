namespace conj_ai.Models.French;

public record FrenchConjugation : IConjugation
{
    public static string? Language => "French";
    public string? Infinitive { get; init; }
    public FrenchPersonalForms? Present { get; init; }
    public FrenchPersonalForms? PasseCompose { get; init; }
    public FrenchPersonalForms? FuturSimple { get; init; }
    public FrenchPersonalForms? Imparfait { get; init; }
    public FrenchPersonalForms? ConditionnelPresent { get; init; }
}