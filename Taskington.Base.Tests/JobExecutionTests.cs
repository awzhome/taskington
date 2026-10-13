using System;
using Taskington.Base.Models;
using Xunit;

namespace Taskington.Base.Tests;

public class JobExecutionTests
{
    private static JobExecution CreateStartedExecution(
        ExecutionMode mode = ExecutionMode.Normal)
    {
        var execution = new JobExecution(Guid.NewGuid(), mode, DateTimeOffset.Now);
        execution.Start();
        return execution;
    }

    [Fact]
    public void NewExecutionIsIdleWithZeroProgress()
    {
        var jobId = Guid.NewGuid();
        var startTime = DateTimeOffset.Now;

        var execution = new JobExecution(jobId, ExecutionMode.Normal, startTime);

        Assert.Equal(jobId, execution.JobId);
        Assert.NotEqual(Guid.Empty, execution.ExecutionId);
        Assert.Equal(JobState.Idle, execution.Status);
        Assert.Equal(ExecutionMode.Normal, execution.ExecutionMode);
        Assert.Equal(startTime, execution.StartTime);
        Assert.Null(execution.EndTime);
        Assert.Equal(0, execution.Progress);
        Assert.Null(execution.ExitCode);
    }

    [Fact]
    public void StartTransitionsIdleToRunning()
    {
        var execution = new JobExecution(Guid.NewGuid(), ExecutionMode.Dry, DateTimeOffset.Now);

        execution.Start();

        Assert.Equal(JobState.Running, execution.Status);
    }

    [Fact]
    public void StartWhileRunningThrows()
    {
        var execution = CreateStartedExecution();

        Assert.Throws<InvalidOperationException>(execution.Start);
    }

    [Fact]
    public void CompleteTransitionsRunningToCompleted()
    {
        var execution = CreateStartedExecution();

        execution.Complete(0);

        Assert.Equal(JobState.Completed, execution.Status);
        Assert.Equal(0, execution.ExitCode);
        Assert.NotNull(execution.EndTime);
    }

    [Fact]
    public void FailTransitionsRunningToFailed()
    {
        var execution = CreateStartedExecution();

        execution.Fail(2);

        Assert.Equal(JobState.Failed, execution.Status);
        Assert.Equal(2, execution.ExitCode);
        Assert.NotNull(execution.EndTime);
    }

    [Fact]
    public void CancelTransitionsRunningToCancelled()
    {
        var execution = CreateStartedExecution();

        execution.Cancel();

        Assert.Equal(JobState.Cancelled, execution.Status);
        Assert.NotNull(execution.EndTime);
    }

    [Fact]
    public void TerminalStatesCannotTransitionAgain()
    {
        var completed = CreateStartedExecution();
        completed.Complete(0);
        var failed = CreateStartedExecution();
        failed.Fail(1);
        var cancelled = CreateStartedExecution();
        cancelled.Cancel();
        var idle = new JobExecution(Guid.NewGuid(), ExecutionMode.Normal, DateTimeOffset.Now);

        Assert.Throws<InvalidOperationException>(() => completed.Complete(0));
        Assert.Throws<InvalidOperationException>(() => completed.Fail(1));
        Assert.Throws<InvalidOperationException>(() => completed.Cancel());
        Assert.Throws<InvalidOperationException>(() => failed.Fail(1));
        Assert.Throws<InvalidOperationException>(() => cancelled.Cancel());
        Assert.Throws<InvalidOperationException>(() => idle.Complete(0));
        Assert.Throws<InvalidOperationException>(() => idle.Cancel());
    }

    [Fact]
    public void ReportProgressUpdatesProgressWhileRunning()
    {
        var execution = CreateStartedExecution();

        execution.ReportProgress(42);

        Assert.Equal(42, execution.Progress);
    }

    [Theory]
    [InlineData(-5, 0)]
    [InlineData(0, 0)]
    [InlineData(100, 100)]
    [InlineData(150, 100)]
    public void ReportProgressClampsValuesToZeroToHundred(int reported, int expected)
    {
        var execution = CreateStartedExecution();

        execution.ReportProgress(reported);

        Assert.Equal(expected, execution.Progress);
    }

    [Fact]
    public void ReportProgressWhileNotRunningThrows()
    {
        var idle = new JobExecution(Guid.NewGuid(), ExecutionMode.Normal, DateTimeOffset.Now);
        var completed = CreateStartedExecution();
        completed.Complete(0);

        Assert.Throws<InvalidOperationException>(() => idle.ReportProgress(50));
        Assert.Throws<InvalidOperationException>(() => completed.ReportProgress(50));
    }
}
