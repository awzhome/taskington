using System;

namespace Taskington.Base.Models;

/// <summary>
/// Terminal outcome of a job execution (spec chapters 4 and 9), reported to the
/// UI and the history at cleanup time.
/// </summary>
public sealed class ExecutionResult(
    Guid executionId,
    Guid jobId,
    ExecutionMode executionMode,
    JobState status,
    DateTimeOffset startTime,
    DateTimeOffset endTime,
    int? exitCode,
    string? errorMessage)
{
    public Guid ExecutionId { get; } = executionId;

    public Guid JobId { get; } = jobId;

    public ExecutionMode ExecutionMode { get; } = executionMode;

    /// <summary>
    /// Terminal state of the execution: Completed, Failed or Cancelled.
    /// </summary>
    public JobState Status { get; } = status;

    public DateTimeOffset StartTime { get; } = startTime;

    public DateTimeOffset EndTime { get; } = endTime;

    public int? ExitCode { get; } = exitCode;

    /// <summary>
    /// Message describing why a script failed, if it did.
    /// </summary>
    public string? ErrorMessage { get; } = errorMessage;
}
