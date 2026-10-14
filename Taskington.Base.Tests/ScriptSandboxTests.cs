using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lua;
using Taskington.Base.Scripting;
using Xunit;

namespace Taskington.Base.Tests;

public class ScriptSandboxTests
{
    private readonly ScriptSandbox sandbox = new(new LuaScriptEngine());

    private sealed class Recorder
    {
        public List<LuaValue> Values { get; } = [];

        public LuaFunction ToLuaFunction()
        {
            return new LuaFunction((context, _) =>
            {
                Values.Add(context.GetArgument(0));
                return new ValueTask<int>(0);
            });
        }
    }

    private static IReadOnlyDictionary<string, LuaFunction> ApiFor(Recorder recorder)
    {
        return new Dictionary<string, LuaFunction> { ["record"] = recorder.ToLuaFunction() };
    }

    [Fact]
    public void CreateStateReturnsEmptyEnvironment()
    {
        using LuaState state = sandbox.CreateState();

        Assert.Empty(state.Environment);
    }

    [Fact]
    public void CreateStateThrowsIfEnvironmentIsPrePopulated()
    {
        using LuaState state = LuaState.Create();
        state.Environment["sneaky"] = new LuaTable();

        Assert.Throws<InvalidOperationException>(() => ScriptSandbox.EnsureSecureEnvironment(state));
    }

    [Fact]
    public async Task ExecuteAsyncRunsScriptAndCallsRegisteredFunction()
    {
        var recorder = new Recorder();

        await sandbox.ExecuteAsync("record('hello')", ApiFor(recorder));

        LuaValue value = Assert.Single(recorder.Values);
        Assert.Equal("hello", value.Read<string>());
    }

    [Fact]
    public async Task ExecuteAsyncUsesFreshEnvironmentPerExecution()
    {
        var recorder = new Recorder();

        await sandbox.ExecuteAsync("leak = 'first run'\nrecord(leak)", ApiFor(recorder));
        await sandbox.ExecuteAsync("record(leak)", ApiFor(recorder));

        Assert.Equal(2, recorder.Values.Count);
        Assert.Equal("first run", recorder.Values[0].Read<string>());
        Assert.Equal(LuaValueType.Nil, recorder.Values[1].Type);
    }

    [Fact]
    public async Task StandardLibrariesAreUnreachable()
    {
        var recorder = new Recorder();
        string script =
            "record(os)\n" +
            "record(io)\n" +
            "record(debug)\n" +
            "record(package)\n" +
            "record(require)\n" +
            "record(load)\n" +
            "record(dofile)\n" +
            "record(print)\n" +
            "record(setmetatable)\n" +
            "record(getmetatable)\n" +
            "record(pcall)\n" +
            "record(error)\n" +
            "record(_G)\n" +
            "record(_VERSION)";

        await sandbox.ExecuteAsync(script, ApiFor(recorder));

        Assert.Equal(14, recorder.Values.Count);
        Assert.All(recorder.Values, value => Assert.Equal(LuaValueType.Nil, value.Type));
    }

    [Theory]
    [InlineData("os.time()")]
    [InlineData("os.remove('/tmp/taskington-escape-attempt')")]
    [InlineData("os.execute('rm -rf /tmp/taskington-escape-attempt')")]
    [InlineData("io.open('/etc/passwd')")]
    [InlineData("io.write('escaped')")]
    [InlineData("require('io')")]
    [InlineData("dofile('/etc/passwd')")]
    [InlineData("load('os.exit(1)')()")]
    [InlineData("setmetatable({}, { __index = _G })")]
    [InlineData("getmetatable('').__index = os")]
    [InlineData("debug.getregistry()")]
    [InlineData("string.rep('x', 1000000)")]
    [InlineData("collectgarbage('count')")]
    [InlineData("rawset(_G, 'os', {})")]
    public async Task EscapeAttemptsFailWithRuntimeErrors(string script)
    {
        await Assert.ThrowsAsync<LuaRuntimeException>(
            () => sandbox.ExecuteAsync(script, new Dictionary<string, LuaFunction>()));
    }

    [Fact]
    public async Task ExecuteAsyncRejectsNullArguments()
    {
        var recorder = new Recorder();

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => sandbox.ExecuteAsync(null!, ApiFor(recorder)));
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => sandbox.ExecuteAsync("record(1)", null!));
    }
}
