using System;

namespace Taskington.Base.Models;

/// <summary>
/// A repeatable task scripted in the Lua-based DSL (spec chapter 4). The id is
/// app-created, equals the storage file name and never changes once assigned.
/// </summary>
public sealed class Job
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Script { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset ModifiedAt { get; set; }

    /// <summary>
    /// True for a newly added job that has not been saved yet (spec chapter 4).
    /// Temporary jobs are not persisted until saved.
    /// </summary>
    public bool IsTemporary { get; set; }
}
