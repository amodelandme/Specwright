namespace Specwright.Core.Context;

public sealed record TokenBudget(
    int MaxInputTokens,
    int ReservedOutputTokens,
    TokenBudgetStrategy Strategy);
