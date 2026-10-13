using Taskington.Base.Models;
using Xunit;

namespace Taskington.Base.Tests;

public class ScriptValidationResultTests
{
    [Fact]
    public void SuccessIsValidWithoutError()
    {
        var result = ScriptValidationResult.Success();

        Assert.True(result.IsValid);
        Assert.Null(result.ErrorMessage);
        Assert.Null(result.ErrorLine);
    }

    [Fact]
    public void FailureCarriesMessageAndLine()
    {
        var result = ScriptValidationResult.Failure("Unexpected symbol near 'and'", 3);

        Assert.False(result.IsValid);
        Assert.Equal("Unexpected symbol near 'and'", result.ErrorMessage);
        Assert.Equal(3, result.ErrorLine);
    }

    [Fact]
    public void FailureWithoutLineHasNullLine()
    {
        var result = ScriptValidationResult.Failure("Script is empty");

        Assert.False(result.IsValid);
        Assert.Null(result.ErrorLine);
    }
}
