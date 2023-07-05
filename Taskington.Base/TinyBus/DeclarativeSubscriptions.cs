using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Taskington.Base.TinyBus;

public static class DeclarativeSubscriptions
{
    public static void SubscribeAsDeclared(object handler)
    {
        var declaredHandlerMethods = handler.GetType().GetMethods().Where(
                method => method.CustomAttributes.Any(a => a.AttributeType == typeof(HandlesMessageAttribute)));
        foreach (var method in declaredHandlerMethods)
        {
            var definitions = method.CustomAttributes.Where(a => a.AttributeType == typeof(HandlesMessageAttribute));
            foreach (var definition in definitions)
            {
                SubscribeHandler(handler, method);
            }
        }
    }

    private static void SubscribeHandler(object handler, MethodInfo method)
    {
        if (method.GetParameters().Length != 1)
        {
            return;
        }

        var messageType = method.GetParameters().First().ParameterType;
        if (messageType is not null)
        {
            if (IsAsyncMessage(messageType, method))
            {
                var subscribeMethod = GetRequestSubscribeMethod(messageType, typeof(AsyncMessage<>).MakeGenericType(messageType), typeof(Task));
                subscribeMethod?.Invoke(null, new[] { method.CreateDelegate(typeof(Func<,>).MakeGenericType(messageType, method.ReturnType), handler) });
            }
            else if (IsAsyncRequestMessage(messageType, method))
            {
                var subscribeMethod = GetRequestSubscribeMethod(
                    messageType,
                    typeof(AsyncRequestMessage<,>).MakeGenericType(messageType, method.ReturnType.GetGenericArguments().First()), method.ReturnType);
                subscribeMethod?.Invoke(null, new[] { method.CreateDelegate(typeof(Func<,>).MakeGenericType(messageType, method.ReturnType), handler) });
            }
            else if (IsSyncMessage(messageType, method))
            {
                var subscribeMethod = GetOneWaySubscribeMethod(messageType, typeof(Message<>).MakeGenericType(messageType));
                subscribeMethod?.Invoke(null, new[] { method.CreateDelegate(typeof(Action<>).MakeGenericType(messageType), handler) });
            }
            else if (IsSyncRequestMessage(messageType, method))
            {
                var subscribeMethod = GetRequestSubscribeMethod(messageType, typeof(RequestMessage<,>).MakeGenericType(messageType, method.ReturnType), method.ReturnType);
                subscribeMethod?.Invoke(null, new[] { method.CreateDelegate(typeof(Func<,>).MakeGenericType(messageType, method.ReturnType), handler) });
            }
        }
    }

    private static bool IsSyncMessage(Type messageType, MethodInfo method)
    {
        try
        {
            return typeof(Message<>).MakeGenericType(messageType).IsAssignableFrom(messageType)
                && (method.GetParameters().Length == 1)
                && messageType.IsAssignableFrom(method.GetParameters()[0].ParameterType);
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static bool IsSyncRequestMessage(Type messageType, MethodInfo method)
    {
        try
        {
            return typeof(RequestMessage<,>).MakeGenericType(messageType, method.ReturnType).IsAssignableFrom(messageType)
                && (method.GetParameters().Length == 1)
                && messageType.IsAssignableFrom(method.GetParameters()[0].ParameterType);
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static bool IsAsyncMessage(Type messageType, MethodInfo method)
    {
        try
        {
            return typeof(AsyncMessage<>).MakeGenericType(messageType).IsAssignableFrom(messageType)
                && (method.GetParameters().Length == 1)
                && (method.ReturnType == typeof(Task))
                && messageType.IsAssignableFrom(method.GetParameters()[0].ParameterType);
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static bool IsAsyncRequestMessage(Type messageType, MethodInfo method)
    {
        try
        {
            return typeof(AsyncRequestMessage<,>)
                    .MakeGenericType(messageType, method.ReturnType.GetGenericArguments().First()).IsAssignableFrom(messageType)
                && (method.GetParameters().Length == 1)
                && (method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
                && messageType.IsAssignableFrom(method.GetParameters()[0].ParameterType);
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static MethodInfo? GetOneWaySubscribeMethod(Type messageType, Type baseMessageType)
    {
        return baseMessageType.GetMethod("Subscribe", new[] { typeof(Action<>).MakeGenericType(messageType) });
    }

    private static MethodInfo? GetRequestSubscribeMethod(Type messageType, Type baseMessageType, Type returnType)
    {
        return baseMessageType.GetMethod("Subscribe", new[] { typeof(Func<,>).MakeGenericType(messageType, returnType) });
    }
}

