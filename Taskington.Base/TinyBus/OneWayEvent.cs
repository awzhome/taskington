using System;
using System.Collections.Generic;

namespace Taskington.Base.TinyBus
{
    public abstract class OneWayEvent<T>
    {
        private readonly List<T> subscriptions = new();

        public void Subscribe(T subscriber)
        {
            subscriptions.Add(subscriber);
        }

        protected void Invoke(Action<T> invoker)
        {
            foreach (var subscriber in subscriptions)
            {
                invoker(subscriber);
            }
        }
    }
}
