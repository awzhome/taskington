using System;
using System.Linq;
using Taskington.Base.TinyBus;
using Xunit;

namespace Taskington.Base.Tests.TinyBus;

public class DeclarativeTypedSyncMessageTests
{
    public record TestOneWayMessage(string Param) : Message<TestOneWayMessage>
    {
        public static void CleanUp() => UnsubscribeAll();
    }

    public record TestDerivedOneWayMessage : TestOneWayMessage
    {
        public TestDerivedOneWayMessage(string param) : base(param)
        {
        }
    }

    public record TestRequestMessage(string Param) : RequestMessage<TestRequestMessage, int>
    {
        public static void CleanUp() => UnsubscribeAll();
    }

    public class TestSyncHandler
    {
        public bool MessageHandled { get; set; }
        public string? MessageText { get; set; }
        public int ReturnedValue { get; set; }

        public TestSyncHandler(int returnedValue = 0)
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

    public DeclarativeTypedSyncMessageTests()
    {
        TestOneWayMessage.CleanUp();
        TestRequestMessage.CleanUp();
    }

    [Fact]
    public void OneWayMessage()
    {
        var handler = new TestSyncHandler();
        DeclarativeSubscriptions.SubscribeAsDeclared(handler);

        (new TestOneWayMessage("ParameterText")).Publish();

        Assert.True(handler.MessageHandled, "MessageHandled");
        Assert.Equal("ParameterText", handler.MessageText);
    }


    [Fact]
    public void RequestMessage()
    {
        var handler1 = new TestSyncHandler(42);
        var handler2 = new TestSyncHandler(43);

        DeclarativeSubscriptions.SubscribeAsDeclared(handler1);
        DeclarativeSubscriptions.SubscribeAsDeclared(handler2);

        var requestedValues = (new TestRequestMessage("ParameterText")).Request().ToList();

        Assert.True(handler1.MessageHandled, "MessageHandled");
        Assert.True(handler2.MessageHandled, "MessageHandled");
        Assert.Equal("ParameterText", handler1.MessageText);
        Assert.Equal("ParameterText", handler2.MessageText);
        Assert.Collection(requestedValues,
            v => Assert.Equal(42, v),
            v => Assert.Equal(43, v));
    }
}
