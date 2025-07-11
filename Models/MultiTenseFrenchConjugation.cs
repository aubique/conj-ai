namespace conj_v2.Models;

public class MultiTenseFrenchConjugation
{
    public string Infinitive { get; init; } = "";
    //public Dictionary<string, FrenchConjugation> Tenses { get; init; } = [];
    public FrenchConjugation? Present { get; init; }
    public FrenchConjugation? FuturSimple { get; init; }
    public FrenchConjugation? PasseCompose { get; init; }
}