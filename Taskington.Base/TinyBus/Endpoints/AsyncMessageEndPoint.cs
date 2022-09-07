using System;
using System.Threading.Tasks;

namespace Taskington.Base.TinyBus.Endpoints;

public class AsyncMessageEndPoint : AsyncOneWayMessageEndPoint<Func<Task>, Func<bool>>
{
    public async Task Push() => await Invoke(async subscriber => await subscriber(), predicate => true);
}

public class AsyncMessageEndPoint<T1> : AsyncOneWayMessageEndPoint<Func<T1, Task>, Func<T1, bool>>
{
    public async Task Push(T1 param) =>
        await Invoke(async subscriber => await subscriber(param), subscriber => subscriber(param));
}

public class AsyncMessageEndPoint<T1, T2> : AsyncOneWayMessageEndPoint<Func<T1, T2, Task>, Func<T1, T2, bool>>
{
    public async Task Push(T1 param1, T2 param2) =>
        await Invoke(async subscriber => await subscriber(param1, param2), predicate => predicate(param1, param2));
}

public class AsyncMessageEndPoint<T1, T2, T3> : AsyncOneWayMessageEndPoint<Func<T1, T2, T3, Task>, Func<T1, T2, T3, bool>>
{
    public async Task Push(T1 param1, T2 param2, T3 param3) =>
        await Invoke(async subscriber => await subscriber(param1, param2, param3), predicate => predicate(param1, param2, param3));
}

public class AsyncMessageEndPoint<T1, T2, T3, T4> : AsyncOneWayMessageEndPoint<Func<T1, T2, T3, T4, Task>, Func<T1, T2, T3, T4, bool>>
{
    public async Task Push(T1 param1, T2 param2, T3 param3, T4 param4) =>
        await Invoke(async subscriber => await subscriber(param1, param2, param3, param4), predicate => predicate(param1, param2, param3, param4));
}

public class AsyncMessageEndPoint<T1, T2, T3, T4, T5> : AsyncOneWayMessageEndPoint<Func<T1, T2, T3, T4, T5, Task>, Func<T1, T2, T3, T4, T5, bool>>
{
    public async Task Push(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5) =>
        await Invoke(async subscriber => await subscriber(param1, param2, param3, param4, param5), predicate => predicate(param1, param2, param3, param4, param5));
}
