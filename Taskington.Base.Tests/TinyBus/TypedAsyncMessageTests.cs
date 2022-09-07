using System.Threading.Tasks;
using Taskington.Base.TinyBus;
using Xunit;

namespace Taskington.Base.Tests.TinyBus;

public class TypedAsyncMessageTests
{
    public record TestOneWayMessage(string Param) : AsyncMessageData<TestOneWayMessage>
    {
        public static void CleanUp() => UnsubscribeAll();
    }
    public record TestRequestMessage(string Param) : AsyncRequestMessageData<TestRequestMessage, int>
    {
        public static void CleanUp() => UnsubscribeAll();
    }

    public TypedAsyncMessageTests()
    {
        TestOneWayMessage.CleanUp();
        TestRequestMessage.CleanUp();
    }

    [Fact]
    public async void OneWayMessage()
    {
        var messageHandled = false;
        string? messageText = null;
        TestOneWayMessage.Subscribe(async message =>
        {
            messageHandled = true;
            messageText = message.Param;
            await Task.Delay(3000);
        });

        await new TestOneWayMessage("ParameterText").Publish();

        Assert.True(messageHandled);
        Assert.Equal("ParameterText", messageText);
    }

    [Fact]
    public async void RequestMessage()
    {
        var messageHandled1 = false;
        string? messageText1 = null;
        var messageHandled2 = false;
        string? messageText2 = null;
        TestRequestMessage.Subscribe(async message =>
        {
            messageHandled1 = true;
            messageText1 = message.Param;
            await Task.Delay(3000);
            return 42;
        });
        TestRequestMessage.Subscribe(async message =>
        {
            messageHandled2 = true;
            messageText2 = message.Param;
            await Task.Delay(100);
            return 43;
        });

        var requestedValues = await new TestRequestMessage("ParameterText").Request();

        Assert.True(messageHandled1);
        Assert.True(messageHandled2);
        Assert.Equal("ParameterText", messageText1);
        Assert.Equal("ParameterText", messageText2);
        Assert.Collection(requestedValues,
            v => Assert.Equal(42, v),
            v => Assert.Equal(43, v));
    }

    [Fact]
    public async void RequestMessageWithPredicate()
    {
        var messageHandled1 = false;
        string? messageText1 = null;
        var messageHandled2 = false;
        string? messageText2 = null;
        TestRequestMessage.Subscribe(async message =>
        {
            messageHandled1 = true;
            messageText1 = message.Param;
            await Task.Delay(3000);
            return 42;
        });
        TestRequestMessage.Subscribe(async message =>
        {
            messageHandled2 = true;
            messageText2 = message.Param;
            await Task.Delay(100);
            return 43;
        }, text => false);

        var requestedValues = await new TestRequestMessage("ParameterText").Request();

        Assert.True(messageHandled1);
        Assert.False(messageHandled2);
        Assert.Equal("ParameterText", messageText1);
        Assert.Null(messageText2);
        Assert.Collection(requestedValues,
            v => Assert.Equal(42, v));
    }
}

