using System;
using System.Collections.Generic;
using Taskington.Base.Log;

namespace Taskington.Base.TinyBus
{
    public interface IEventBus
    {
        IEventBus Subscribe<T>(Action<T> subscriber) where T : IEvent;
        IEventBus Subscribe<T, R>(Func<T, R> subscriber) where T : IResponse<R>;
        IEventBus Push<T>(T @event) where T : IEvent;
        IEnumerable<R> Request<T, R>(T @event) where T : IResponse<R>;
    }

    internal class EventBus : IEventBus
    {
        private readonly ILog log;

        private readonly Dictionary<Type, List<Delegate>> subscriptions = new();

        public EventBus(ILog log)
        {
            this.log = log;
        }

        public IEventBus Subscribe<T>(Action<T> subscriber) where T : IEvent
        {
            Type eventType = typeof(T);
            if (!subscriptions.ContainsKey(eventType))
            {
                subscriptions[eventType] = new();
            }
            subscriptions[eventType].Add(subscriber);

            return this;
        }
        
        public IEventBus Subscribe<T, R>(Func<T, R> subscriber) where T : IResponse<R>
        {
            Type eventType = typeof(T);
            if (!subscriptions.ContainsKey(eventType))
            {
                subscriptions[eventType] = new();
            }
            subscriptions[eventType].Add(subscriber);

            return this;
        }

        public IEventBus Push<T>(T @event) where T : IEvent
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

            return this;
        }

        public IEnumerable<R> Request<T, R>(T @event) where T : IResponse<R>
        {
            Type eventType = typeof(T);
            if (subscriptions.ContainsKey(eventType))
            {
                foreach (var subscriber in subscriptions[eventType])
                {
                    if (subscriber is Func<T, R> subscriberCallback)
                    {
                        yield return subscriberCallback(@event);
                    }
                }
            }
        }
    }
}
