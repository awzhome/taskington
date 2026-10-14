using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lua;
using Lua.Runtime;

namespace Taskington.Base.Scripting;

/// <summary>
/// Zero-trust execution environment for job scripts (spec chapter 6): a fresh Lua
/// state per execution whose environment contains only the registered Taskington
/// API functions. Standard Lua libraries are never opened, so io, os, debug,
/// require and metatable access are unreachable from scripts.
/// </summary>
public sealed class ScriptSandbox(LuaScriptEngine scriptEngine)
{
    /// <summary>
    /// Creates a fresh Lua state for one execution. Throws if the environment is
    /// not empty, so a future LuaCSharp version that pre-populates globals cannot
    /// silently weaken the sandbox.
    /// </summary>
    public LuaState CreateState()
    {
        LuaState state = LuaState.Create();
        EnsureSecureEnvironment(state);
        return state;
    }

    /// <summary>
    /// Runs the script in a fresh environment with exactly the given API functions
    /// registered. Syntax errors throw <see cref="LuaCompileException"/>, runtime
    /// errors <see cref="LuaRuntimeException"/>; both bubble to the caller, which
    /// decides failure handling.
    /// </summary>
    public async Task ExecuteAsync(
        string script,
        IReadOnlyDictionary<string, LuaFunction> apiFunctions,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(script);
        ArgumentNullException.ThrowIfNull(apiFunctions);

        using LuaState state = CreateState();
        foreach (KeyValuePair<string, LuaFunction> entry in apiFunctions)
        {
            state.Environment[entry.Key] = entry.Value;
        }

        LuaClosure closure = scriptEngine.Compile(state, script);
        await state.RunAsync(closure, 0, cancellationToken);
    }

    internal static void EnsureSecureEnvironment(LuaState state)
    {
        foreach (KeyValuePair<LuaValue, LuaValue> entry in state.Environment)
        {
            throw new InvalidOperationException(
                $"The fresh Lua environment is not empty (found global '{entry.Key}');"
                + " refusing to run scripts in an unsecured state.");
        }
    }
}
