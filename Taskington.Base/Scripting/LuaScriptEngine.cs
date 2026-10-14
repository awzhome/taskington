using System;
using Lua;
using Lua.Runtime;
using Taskington.Base.Models;

namespace Taskington.Base.Scripting;

/// <summary>
/// Wraps the LuaCSharp interpreter for script loading and parsing (spec chapter 6):
/// validates job scripts and produces structured syntax errors with line information
/// for the editor feedback. Knows nothing about execution; the <see cref="ScriptSandbox"/>
/// owns the Lua state a script runs in.
/// </summary>
public sealed class LuaScriptEngine
{
    /// <summary>Chunk name used in Lua error messages for job scripts.</summary>
    internal const string ChunkName = "script";

    /// <summary>
    /// Parses the script and reports the first syntax error, if any. Only syntax is
    /// checked: unknown globals (e.g. misspelled API functions) are runtime errors
    /// and do not fail validation.
    /// </summary>
    public ScriptValidationResult Validate(string script)
    {
        ArgumentNullException.ThrowIfNull(script);

        try
        {
            using LuaState state = LuaState.Create();
            state.Load(script, ChunkName);
            return ScriptValidationResult.Success();
        }
        catch (LuaCompileException exception)
        {
            return ScriptValidationResult.Failure(exception.MessageWithNearToken, exception.Position.Line);
        }
    }

    /// <summary>
    /// Compiles the script into a closure bound to the given state's environment.
    /// Throws <see cref="LuaCompileException"/> for invalid scripts; callers are
    /// expected to validate first.
    /// </summary>
    internal LuaClosure Compile(LuaState state, string script)
    {
        return state.Load(script, ChunkName);
    }
}
