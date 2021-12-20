using System;

namespace Taskington.Base.Extension
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class TaskingtonExtensionAttribute : Attribute
    {
        public Type Binder { get; set; }

        public TaskingtonExtensionAttribute(Type binder)
        {
            Binder = binder;
        }
    }
}
