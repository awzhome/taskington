using System;

namespace Taskington.Base.TinyBus;

[AttributeUsage(AttributeTargets.Method)]
public class HandlesMessageAttribute : Attribute
{
    public HandlesMessageAttribute()
    {
    }
}
