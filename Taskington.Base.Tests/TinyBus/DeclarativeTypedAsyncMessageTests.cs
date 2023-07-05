using System;
using System.Threading.Tasks;
using Taskington.Base.TinyBus;
using Xunit;

namespace Taskington.Base.Tests.TinyBus
{
    public class DeclarativeTypedAsyncMessageTests
    {
        public record TestOneWayMessage(string Param) : AsyncMessage<TestOneWayMessage>
        {
            public static void CleanUp() => UnsubscribeAll();
        }

        public record TestRequestMessage(string Param) : AsyncRequestMessage<TestRequestMessage, int>
        {
            public static void CleanUp() => UnsubscribeAll();
        }

        public DeclarativeTypedAsyncMessageTests()
        {
            TestOneWayMessage.CleanUp();
            TestRequestMessage.CleanUp();
        }

        public class TestAsyncHandler
        {
            public bool MessageHandled { get; set; }
            public string? MessageText { get; set; }
            public int ReturnedValue { get; set; }
            public int ReturnDelay { get; set; }

            public TestAsyncHandler(int returnedValue = 0, int returnDelay = 3000)
            {
                ReturnedValue = returnedValue;
                ReturnDelay = returnDelay;
            }

            [HandlesMessage]
            public async Task HandleOneWayMessage(TestOneWayMessage message)
            {
                MessageHandled = true;
                MessageText = message.Param;
                await Task.Delay(ReturnDelay);
            }

            [HandlesMessage]
            public async Task<int> HandleRequestMessage(TestRequestMessage message)
            {
                MessageHandled = true;
                MessageText = message.Param;
                await Task.Delay(ReturnDelay);
                return ReturnedValue;
            }
        }

        [Fact]
        public async void OneWayMessage()
        {
            var handler = new TestAsyncHandler();
            DeclarativeSubscriptions.SubscribeAsDeclared(handler);

            await new TestOneWayMessage("ParameterText").Publish();

            Assert.True(handler.MessageHandled, "MessageHandled");
            Assert.Equal("ParameterText", handler.MessageText);
        }

        [Fact]
        public async void RequestMessage()
        {
            var handler1 = new TestAsyncHandler(42);
            var handler2 = new TestAsyncHandler(43, 100);

            DeclarativeSubscriptions.SubscribeAsDeclared(handler1);
            DeclarativeSubscriptions.SubscribeAsDeclared(handler2);

            var requestedValues = await new TestRequestMessage("ParameterText").Request();

            Assert.True(handler1.MessageHandled, "MessageHandled");
            Assert.True(handler2.MessageHandled, "MessageHandled");
            Assert.Equal("ParameterText", handler1.MessageText);
            Assert.Equal("ParameterText", handler2.MessageText);
            Assert.Collection(requestedValues,
                v => Assert.Equal(42, v),
                v => Assert.Equal(43, v));
        }
    }
}

