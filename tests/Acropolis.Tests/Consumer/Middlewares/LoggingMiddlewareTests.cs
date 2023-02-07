using Acropolis.Consumer.Middlewares;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Ziggurat;

namespace Acropolis.Tests.Consumer.Middlewares;

public class LoggingMiddlewareTests
{
    [Fact]
    public async Task ProcessMessageAsync_NewMessage_CallServiceProcessMessage()
    {
        // Arrange
        var mockService = new Mock<IConsumerService<TestMessage>>();
        var service = CreateLoggingMiddleware(null);
        var message = new TestMessage("message1", "group1");

        // Act
        await service.OnExecutingAsync(message, testMessage => mockService.Object.ProcessMessageAsync(testMessage));

        // Assert
        mockService.Verify(x => x.ProcessMessageAsync(message), Times.Once);
    }

    [Fact]
    public async Task ProcessMessageAsync_OtherException_PropagateException()
    {
        // Arrange
        var mockService = new Mock<IConsumerService<TestMessage>>();
        var mockLogger = new Mock<ILogger<LoggingMiddleware<TestMessage>>>();
        var service = CreateLoggingMiddleware(mockLogger);
        var message = new TestMessage("message1", "group1");

        mockService
            .Setup(x => x.ProcessMessageAsync(message))
            .Throws<InvalidOperationException>();

        // Act
        var action = async () =>
            await service.OnExecutingAsync(message,
                testMessage => mockService.Object.ProcessMessageAsync(testMessage));

        // Assert
        await action.Should().ThrowAsync<InvalidOperationException>();
        mockLogger.VerifyLog(logger =>
                logger.LogInformation("Message was processed already. Ignoring message1:group1."),
            Times.Never);
    }

    private static LoggingMiddleware<TestMessage> CreateLoggingMiddleware(
        Mock<ILogger<LoggingMiddleware<TestMessage>>>? mockLogger)
    {
        mockLogger ??= new Mock<ILogger<LoggingMiddleware<TestMessage>>>();

        var service = new LoggingMiddleware<TestMessage>(mockLogger.Object);
        return service;
    }

    public record TestMessage(string MessageId, string MessageGroup) : IMessage
    {
        public string MessageId { get; set; } = MessageId;
        public string MessageGroup { get; set; } = MessageGroup;
    }
}