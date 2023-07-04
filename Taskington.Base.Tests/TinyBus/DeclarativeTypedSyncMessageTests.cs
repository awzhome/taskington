using System;
using System.Linq;
using Taskington.Base.TinyBus;
using Xunit;

namespace Taskington.Base.Tests.TinyBus;

public record TestOneWayMessage(string Param) : Message<TestOneWayMessage>
{
    public static void CleanUp() => UnsubscribeAll();
}
public record TestRequestMessage(string Param) : RequestMessage<TestRequestMessage, int>
{
    public static void CleanUp() => UnsubscribeAll();
}

public class ExplicitTestSyncHandler
{
    public bool MessageHandled { get; set; }
    public string? MessageText { get; set; }
    public int ReturnedValue { get; set; }

    public ExplicitTestSyncHandler(int returnedValue = 0)
    {
        ReturnedValue = returnedValue;
    }

    [HandlesMessage(typeof(TestOneWayMessage))]
    public void HandleOneWayMessage(TestOneWayMessage message)
    {
        MessageHandled = true;
        MessageText = message.Param;
    }

    [HandlesMessage(typeof(TestRequestMessage))]
    public int HandleRequestMessage(TestRequestMessage message)
    {
        MessageHandled = true;
        MessageText = message.Param;
        return ReturnedValue;
    }
}

public class ImplicitTestSyncHandler
{
    public bool MessageHandled { get; set; }
    public string? MessageText { get; set; }
    public int ReturnedValue { get; set; }

    public ImplicitTestSyncHandler(int returnedValue = 0)
    {
        ReturnedValue = returnedValue;
    }

    [HandlesMessage]
    public void HandleOneWayMessage(TestOneWayMessage message)
    {
        MessageHandled = true;
        MessageText = message.Param;
    }

    [HandlesMessage]
    public int HandleRequestMessage(TestRequestMessage message)
    {
        MessageHandled = true;
        MessageText = message.Param;
        return ReturnedValue;
    }
}

public class DeclarativeTypedSyncMessageTests
{
    public DeclarativeTypedSyncMessageTests()
    {
        TestOneWayMessage.CleanUp();
        TestRequestMessage.CleanUp();
    }

    [Fact]
    public void OneWayMessage()
    {
        var handler = new ExplicitTestSyncHandler();
        DeclarativeSubscriptions.SubscribeAsDeclared(handler);

        (new TestOneWayMessage("ParameterText")).Publish();

        Assert.True(handler.MessageHandled);
        Assert.Equal("ParameterText", handler.MessageText);
    }

    [Fact]
    public void RequestMessageExplicit()
    {
        var handler1 = new ExplicitTestSyncHandler(42);
        var handler2 = new ExplicitTestSyncHandler(43);

        DeclarativeSubscriptions.SubscribeAsDeclared(handler1);
        DeclarativeSubscriptions.SubscribeAsDeclared(handler2);

        var requestedValues = (new TestRequestMessage("ParameterText")).Request().ToList();

        Assert.True(handler1.MessageHandled);
        Assert.True(handler2.MessageHandled);
        Assert.Equal("ParameterText", handler1.MessageText);
        Assert.Equal("ParameterText", handler2.MessageText);
        Assert.Collection(requestedValues,
            v => Assert.Equal(42, v),
            v => Assert.Equal(43, v));
    }

    [Fact]
    public void RequestMessageImplicit()
    {
        var handler1 = new ImplicitTestSyncHandler(42);
        var handler2 = new ImplicitTestSyncHandler(43);

        DeclarativeSubscriptions.SubscribeAsDeclared(handler1);
        DeclarativeSubscriptions.SubscribeAsDeclared(handler2);

        var requestedValues = (new TestRequestMessage("ParameterText")).Request().ToList();

        Assert.True(handler1.MessageHandled);
        Assert.True(handler2.MessageHandled);
        Assert.Equal("ParameterText", handler1.MessageText);
        Assert.Equal("ParameterText", handler2.MessageText);
        Assert.Collection(requestedValues,
            v => Assert.Equal(42, v),
            v => Assert.Equal(43, v));
    }
}

