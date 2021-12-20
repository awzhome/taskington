using System;

namespace Taskington.Base.TinyBus
{
    public class Event : OneWayEvent<Action>
    {
        public void Push() => Invoke(subscriber => subscriber());
    }

    public class Event<T1> : OneWayEvent<Action<T1>>
    {
        public void Push(T1 param) => Invoke(subscriber => subscriber(param));
    }

    public class Event<T1, T2> : OneWayEvent<Action<T1, T2>>
    {
        public void Push(T1 param1, T2 param2) => Invoke(subscriber => subscriber(param1, param2));
    }

    public class Event<T1, T2, T3> : OneWayEvent<Action<T1, T2, T3>>
    {
        public void Push(T1 param1, T2 param2, T3 param3) => Invoke(subscriber => subscriber(param1, param2, param3));
    }

    public class Event<T1, T2, T3, T4> : OneWayEvent<Action<T1, T2, T3, T4>>
    {
        public void Push(T1 param1, T2 param2, T3 param3, T4 param4) => Invoke(subscriber => subscriber(param1, param2, param3, param4));
    }

    public class Event<T1, T2, T3, T4, T5> : OneWayEvent<Action<T1, T2, T3, T4, T5>>
    {
        public void Push(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5) => Invoke(subscriber => subscriber(param1, param2, param3, param4, param5));
    }
}
