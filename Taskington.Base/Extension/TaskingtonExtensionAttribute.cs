using System;

namespace Taskington.Base.Extension;

[AttributeUsage(AttributeTargets.Assembly)]
public class TaskingtonExtensionAttribute(Type binder) : Attribute
{
    public Type Binder { get; set; } = binder;
}