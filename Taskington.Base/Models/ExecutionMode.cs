namespace Taskington.Base.Models;

/// <summary>
/// How a job execution performs its actions (spec chapter 9): Normal runs real
/// actions, Dry records "would do ..." entries instead of acting.
/// </summary>
public enum ExecutionMode
{
    Normal,
    Dry
}
