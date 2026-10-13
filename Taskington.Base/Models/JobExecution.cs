using System;

namespace Taskington.Base.Models;

/// <summary>
/// Execution context of a single job run (spec chapter 4): created during the
/// prepare phase in <see cref="JobState.Idle"/>, started for the execute phase and
/// finalized in exactly one terminal state. Output streaming is attached in later
/// backlog phases and is not part of this model.
/// </summary>
public sealed class JobExecution(Guid jobId, ExecutionMode executionMode, DateTimeOffset startTime)
{
    public Guid ExecutionId { get; } = Guid.NewGuid();

    public Guid JobId { get; } = jobId;

    public DateTimeOffset StartTime { get; } = startTime;

    public DateTimeOffset? EndTime { get; private set; }

    public JobState Status { get; private set; } = JobState.Idle;

    public ExecutionMode ExecutionMode { get; } = executionMode;

    /// <summary>
    /// Execution progress between 0 and 100.
    /// </summary>
    public int Progress { get; private set; }

    /// <summary>
    /// Exit code of the last executed command, if any.
    /// </summary>
    public int? ExitCode { get; private set; }

    /// <summary>
    /// Transitions the execution from Idle to Running.
    /// </summary>
    public void Start()
    {
        EnsureState(JobState.Idle, "start");
        Status = JobState.Running;
    }

    /// <summary>
    /// Reports progress while running; values outside 0-100 are clamped.
    /// </summary>
    public void ReportProgress(int progress)
    {
        EnsureState(JobState.Running, "report progress for");
        Progress = Math.Clamp(progress, 0, 100);
    }

    /// <summary>
    /// Transitions a running execution to Completed.
    /// </summary>
    public void Complete(int? exitCode)
    {
        Finalize(JobState.Completed, exitCode);
    }

    /// <summary>
    /// Transitions a running execution to Failed.
    /// </summary>
    public void Fail(int? exitCode)
    {
        Finalize(JobState.Failed, exitCode);
    }

    /// <summary>
    /// Transitions a running execution to Cancelled.
    /// </summary>
    public void Cancel()
    {
        Finalize(JobState.Cancelled, ExitCode);
    }

    private void Finalize(JobState finalState, int? exitCode)
    {
        EnsureState(JobState.Running, finalState.ToString().ToLowerInvariant());
        Status = finalState;
        ExitCode = exitCode;
        EndTime = DateTimeOffset.Now;
    }

    private void EnsureState(JobState expected, string action)
    {
        if (Status != expected)
        {
            throw new InvalidOperationException(
                $"Cannot {action} an execution in state {Status} (expected {expected}).");
        }
    }
}
