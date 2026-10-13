namespace Taskington.Base.Models;

/// <summary>
/// Result of validating a job script before execution (spec chapter 9): syntax
/// errors prevent execution and carry line information for the editor feedback.
/// Use <see cref="Success"/> and <see cref="Failure"/> to construct.
/// </summary>
public sealed class ScriptValidationResult(bool isValid, string? errorMessage, int? errorLine)
{
    /// <summary>
    /// True if the script is valid and may be executed.
    /// </summary>
    public bool IsValid { get; } = isValid;

    /// <summary>
    /// Description of the validation error; null when the script is valid.
    /// </summary>
    public string? ErrorMessage { get; } = errorMessage;

    /// <summary>
    /// One-based script line the error occurred on; null when unknown or the
    /// script is valid.
    /// </summary>
    public int? ErrorLine { get; } = errorLine;

    public static ScriptValidationResult Success()
    {
        return new ScriptValidationResult(true, null, null);
    }

    public static ScriptValidationResult Failure(string errorMessage, int? errorLine = null)
    {
        return new ScriptValidationResult(false, errorMessage, errorLine);
    }
}
