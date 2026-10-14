using System;
using Taskington.Base.Models;
using Taskington.Base.Scripting;
using Xunit;

namespace Taskington.Base.Tests;

public class LuaScriptEngineTests
{
    private readonly LuaScriptEngine engine = new();

    [Fact]
    public void ValidateAcceptsValidScript()
    {
        ScriptValidationResult result = engine.Validate(
            "local name = 'backup'\nif name then\n    log(name)\nend");

        Assert.True(result.IsValid);
        Assert.Null(result.ErrorMessage);
        Assert.Null(result.ErrorLine);
    }

    [Fact]
    public void ValidateAcceptsEmptyScript()
    {
        ScriptValidationResult result = engine.Validate("");

        Assert.True(result.IsValid);
    }

    [Fact]
    public void ValidateAcceptsUnknownGlobals()
    {
        ScriptValidationResult result = engine.Validate("os.time()");

        Assert.True(result.IsValid);
    }

    [Fact]
    public void ValidateRejectsNullScript()
    {
        Assert.Throws<ArgumentNullException>(() => engine.Validate(null!));
    }

    [Fact]
    public void ValidateReportsSyntaxErrorWithLineInformation()
    {
        ScriptValidationResult result = engine.Validate(
            "local a = 1\nlocal b = 2\na = = 3");

        Assert.False(result.IsValid);
        Assert.NotNull(result.ErrorMessage);
        Assert.Equal(3, result.ErrorLine);
    }

    [Fact]
    public void ValidateReportsUnclosedStringOnItsLine()
    {
        ScriptValidationResult result = engine.Validate(
            "log('hello')\nlog('unclosed)");

        Assert.False(result.IsValid);
        Assert.Equal(2, result.ErrorLine);
    }

    [Fact]
    public void ValidateReportsUnterminatedBlock()
    {
        ScriptValidationResult result = engine.Validate(
            "if true then\n    log('never closed')");

        Assert.False(result.IsValid);
        Assert.Equal(2, result.ErrorLine);
    }
}
