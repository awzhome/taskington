using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taskington.Base.TinyBus.Endpoints;
using Xunit;

namespace Taskington.Base.Tests.TinyBus;

public class AsyncMessageTests
{
    public AsyncMessageEndPoint<string> TestOneWayMessage { get; } = new();
    public AsyncRequestMessageEndPoint<string, int> TestRequestMessage { get; } = new();

    public AsyncMessageTests()
    {
    }

    [Fact]
    public async void OneWayMessage()
    {
        var messageHandled = false;
        string? messageText = null;
        TestOneWayMessage.Subscribe(async text =>
        {
            messageHandled = true;
            messageText = text;
            await Task.Delay(3000);
        });

        await TestOneWayMessage.Push("ParameterText");

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
        TestRequestMessage.Subscribe(async text =>
        {
            messageHandled1 = true;
            messageText1 = text;
            await Task.Delay(3000);
            return 42;
        });
        TestRequestMessage.Subscribe(async text =>
        {
            messageHandled2 = true;
            messageText2 = text;
            await Task.Delay(100);
            return 43;
        });


        var requestedValues = await TestRequestMessage.Request("ParameterText");

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
        TestRequestMessage.Subscribe(async text =>
        {
            messageHandled1 = true;
            messageText1 = text;
            await Task.Delay(3000);
            return 42;
        });
        TestRequestMessage.Subscribe(async text =>
        {
            messageHandled2 = true;
            messageText2 = text;
            await Task.Delay(100);
            return 43;
        }, text => false);


        var requestedValues = await TestRequestMessage.Request("ParameterText");

        Assert.True(messageHandled1);
        Assert.False(messageHandled2);
        Assert.Equal("ParameterText", messageText1);
        Assert.Null(messageText2);
        Assert.Collection(requestedValues,
            v => Assert.Equal(42, v));
    }
}

