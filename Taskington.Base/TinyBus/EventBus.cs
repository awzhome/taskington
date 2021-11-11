using System;
using System.Collections.Generic;
using Taskington.Base.Log;

namespace Taskington.Base.TinyBus
{
    public interface IEventBus
    {
        void Subscribe<T>(Action<T> subscriber);
        void Push<T>(T @event);
    }

    internal class EventBus : IEventBus
    {
        private readonly ILog log;

        private readonly Dictionary<Type, List<Delegate>> subscriptions = new();

        public EventBus(ILog log)
        {
            this.log = log;
        }

        public void Subscribe<T>(Action<T> subscriber)
        {
            Type eventType = typeof(T);
            if (!subscriptions.ContainsKey(eventType))
            {
                subscriptions[eventType] = new();
            }
            subscriptions[eventType].Add(subscriber);
        }

        public void Push<T>(T @event)
        {
            Type eventType = typeof(T);
            if (subscriptions.ContainsKey(eventType))
            {
                foreach (var subscriber in subscriptions[eventType])
                {
                    if (subscriber is Action<T> subscriberCallback)
                    {
                        subscriberCallback(@event);
                    }
                }
            }
        }
    }
}
