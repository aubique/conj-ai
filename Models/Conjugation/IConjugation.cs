namespace conj_ai.Models.Conjugation;

public interface IConjugation
{
    static abstract string? Language { get; }
    string? Infinitive { get; init; }
}