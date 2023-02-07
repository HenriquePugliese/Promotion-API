using Microsoft.Extensions.Logging;
using Moq;

namespace Acropolis.Tests.Helpers;

public static class TestingLoggerHelper
{
    /// <summary>
    /// Extension method that should be called by a mocked ILogger object. It will
    /// verify if the log method was called in an especific test case.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="expectedMessage">
    /// Message that was expected to be logged in the SUT.
    /// </param>
    /// <param name="expectedLogLevel">
    /// Level of the expected logged message (Microsoft.Extensions.Logging.LogLevel).
    /// </param>
    /// <param name="times">
    /// How many times was expected to the logger be called in the SUT.
    /// </param>
    public static Mock<ILogger<T>> VerifyLogging<T>(
        this Mock<ILogger<T>> logger,
        string expectedMessage,
        LogLevel expectedLogLevel = LogLevel.Debug,
        Times? times = null)
    {
        times ??= Times.Once();

        Func<object, Type, bool> state = (v, _) => string.Compare(v.ToString(), expectedMessage, StringComparison.Ordinal) == 0;

        logger.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == expectedLogLevel),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => state(v, t)),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), (Times)times);

        return logger;
    }
}