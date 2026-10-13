namespace Taskington.Base.Models;

/// <summary>
/// Lifecycle state of a job or a job execution (spec chapter 4). Idle is the resting
/// state of a job; executions start in Idle, run and end in one of the terminal
/// states Completed, Failed or Cancelled.
/// </summary>
public enum JobState
{
    Idle,
    Running,
    Completed,
    Failed,
    Cancelled
}
