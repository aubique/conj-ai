namespace conj_ai.Models;

public record MultiTenseFrenchConjugation(string? Infinitive,
                                          FrenchConjugation? Present,
                                          FrenchConjugation? PasseCompose,
                                          FrenchConjugation? FuturSimple,
                                          FrenchConjugation? Imparfait,
                                          FrenchConjugation? ConditionnelPresent);