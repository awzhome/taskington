using System;
using System.Collections.Generic;
using System.Linq;
using Taskington.Base.TinyBus.Endpoints;
using Xunit;

namespace Taskington.Base.Tests.TinyBus;

public class SyncMessageTests
{
    public MessageEndPoint<string> TestOneWayMessage { get; } = new();
    public RequestMessageEndPoint<string, int> TestRequestMessage { get; } = new();

    public SyncMessageTests()
    {
    }

    [Fact]
    public void OneWayMessage()
    {
        var messageHandled = false;
        string? messageText = null;
        TestOneWayMessage.Subscribe(text =>
        {
            messageHandled = true;
            messageText = text;
        });

        TestOneWayMessage.Push("ParameterText");

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
        TestRequestMessage.Subscribe(text =>
        {
            messageHandled1 = true;
            messageText1 = text;
            return 42;
        });
        TestRequestMessage.Subscribe(text =>
        {
            messageHandled2 = true;
            messageText2 = text;
            return 43;
        });


        var requestedValues = TestRequestMessage.Request("ParameterText").ToList();

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
        TestRequestMessage.Subscribe(text =>
        {
            messageHandled1 = true;
            messageText1 = text;
            return 42;
        });
        TestRequestMessage.Subscribe(text =>
        {
            messageHandled2 = true;
            messageText2 = text;
            return 43;
        }, text => false);


        var requestedValues = TestRequestMessage.Request("ParameterText").ToList();

        Assert.True(messageHandled1);
        Assert.False(messageHandled2);
        Assert.Equal("ParameterText", messageText1);
        Assert.Null(messageText2);
        Assert.Collection(requestedValues,
            v => Assert.Equal(42, v));
    }
}

