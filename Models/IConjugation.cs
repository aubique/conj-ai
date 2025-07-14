namespace conj_ai.Models;

public interface IConjugation
{
    static abstract string? Language { get; }
    string? Infinitive { get; init; }
}