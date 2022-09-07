using System.Linq;
using Taskington.Base.TinyBus;
using Xunit;

namespace Taskington.Base.Tests.TinyBus;

public class TypedSyncMessageTests
{
    public record TestOneWayMessage(string Param) : Message<TestOneWayMessage>
    {
        public static void CleanUp() => UnsubscribeAll();
    }
    public record TestRequestMessage(string Param) : RequestMessage<TestRequestMessage, int>
    {
        public static void CleanUp() => UnsubscribeAll();
    }

    public TypedSyncMessageTests()
    {
        TestOneWayMessage.CleanUp();
        TestRequestMessage.CleanUp();
    }

    [Fact]
    public void OneWayMessage()
    {
        var messageHandled = false;
        string? messageText = null;
        TestOneWayMessage.Subscribe(message =>
        {
            messageHandled = true;
            messageText = message.Param;
        });

        (new TestOneWayMessage("ParameterText")).Publish();

        Assert.True(messageHandled);
        Assert.Equal("ParameterText", messageText);
    }

    [Fact]
    public void RequestMessage()
    {
        var messageHandled1 = false;
        string? messageText1 = null;
        var messageHandled2 = false;
        string? messageText2 = null;
        TestRequestMessage.Subscribe(message =>
        {
            messageHandled1 = true;
            messageText1 = message.Param;
            return 42;
        });
        TestRequestMessage.Subscribe(message =>
        {
            messageHandled2 = true;
            messageText2 = message.Param;
            return 43;
        });


        var requestedValues = (new TestRequestMessage("ParameterText")).Request().ToList();

        Assert.True(messageHandled1);
        Assert.True(messageHandled2);
        Assert.Equal("ParameterText", messageText1);
        Assert.Equal("ParameterText", messageText2);
        Assert.Collection(requestedValues,
            v => Assert.Equal(42, v),
            v => Assert.Equal(43, v));
    }

    [Fact]
    public void RequestMessageWithPredicate()
    {
        var messageHandled1 = false;
        string? messageText1 = null;
        var messageHandled2 = false;
        string? messageText2 = null;
        TestRequestMessage.Subscribe(message =>
        {
            messageHandled1 = true;
            messageText1 = message.Param;
            return 42;
        });
        TestRequestMessage.Subscribe(message =>
        {
            messageHandled2 = true;
            messageText2 = message.Param;
            return 43;
        }, text => false);

        var requestedValues = (new TestRequestMessage("ParameterText")).Request().ToList();

        Assert.True(messageHandled1);
        Assert.False(messageHandled2);
        Assert.Equal("ParameterText", messageText1);
        Assert.Null(messageText2);
        Assert.Collection(requestedValues,
            v => Assert.Equal(42, v));
    }
}

