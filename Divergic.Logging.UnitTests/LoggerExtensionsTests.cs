namespace Divergic.Logging.UnitTests
{
    using System;
    using Divergic.Logging.Xunit;
    using FluentAssertions;
    using global::Xunit;
    using global::Xunit.Abstractions;
    using Microsoft.Extensions.Logging;

    public class LoggerExtensionsTests
    {
        private readonly ICacheLogger _logger;

        public LoggerExtensionsTests(ITestOutputHelper output)
        {
            _logger = output.BuildLogger();
        }

        [Fact]
        public void LogCriticalWithContextThrowsExceptionWithEventIdNullExceptionAndContextData()
        {
            var eventId = new EventId(Environment.TickCount);
            var contextData = Guid.NewGuid().ToString();

            Action action = () => _logger.LogCriticalWithContext(eventId, null, contextData);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void LogCriticalWithContextThrowsExceptionWithEventIdNullExceptionContextDataAndMessage()
        {
            var eventId = new EventId(Environment.TickCount);
            var contextData = Guid.NewGuid().ToString();
            const string Message = "{0}-{1}";
            var args = new object[]
            {
                123, true
            };

            Action action = () => _logger.LogCriticalWithContext(eventId, null, contextData, Message, args);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void LogCriticalWithContextThrowsExceptionWithNullExceptionAndContextData()
        {
            var contextData = Guid.NewGuid().ToString();

            Action action = () => _logger.LogCriticalWithContext(null, contextData);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void LogCriticalWithContextThrowsExceptionWithNullExceptionContextDataAndMessage()
        {
            var contextData = Guid.NewGuid().ToString();
            const string Message = "{0}-{1}";
            var args = new object[]
            {
                123, true
            };

            Action action = () => _logger.LogCriticalWithContext(null, contextData, Message, args);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void LogCriticalWithContextThrowsExceptionWithNullLoggerEventIdExceptionAndContextData()
        {
            var eventId = new EventId(Environment.TickCount);
            var exception = new TimeoutException();
            var contextData = Guid.NewGuid().ToString();

            var log = (ILogger)null;

            Action action = () => log.LogCriticalWithContext(eventId, exception, contextData);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void LogCriticalWithContextThrowsExceptionWithNullLoggerEventIdExceptionContextDataAndMessage()
        {
            var eventId = new EventId(Environment.TickCount);
            var exception = new TimeoutException();
            var contextData = Guid.NewGuid().ToString();
            const string Message = "{0}-{1}";
            var args = new object[]
            {
                123, true
            };

            var log = (ILogger)null;

            Action action = () => log.LogCriticalWithContext(eventId, exception, contextData, Message, args);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void LogCriticalWithContextThrowsExceptionWithNullLoggerExceptionAndContextData()
        {
            var exception = new TimeoutException();
            var contextData = Guid.NewGuid().ToString();

            var log = (ILogger)null;

            Action action = () => log.LogCriticalWithContext(exception, contextData);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void LogCriticalWithContextThrowsExceptionWithNullLoggerExceptionContextDataAndMessage()
        {
            var exception = new TimeoutException();
            var contextData = Guid.NewGuid().ToString();
            const string Message = "{0}-{1}";
            var args = new object[]
            {
                123, true
            };

            var log = (ILogger)null;

            Action action = () => log.LogCriticalWithContext(exception, contextData, Message, args);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void LogCriticalWithWritesLogWithEventIdExceptionAndContextData()
        {
            var eventId = new EventId(Environment.TickCount);
            var exception = new TimeoutException();
            var contextData = Guid.NewGuid().ToString();

            _logger.LogCriticalWithContext(eventId, exception, contextData);

            var actual = _logger.Last;

            actual.LogLevel.Should().Be(LogLevel.Critical);
            actual.EventId.Id.Should().Be(eventId.Id);
            actual.Message.Should().BeNull();
            actual.Exception.Should().Be(exception);
            actual.Exception.Data["ContextData"].ToString().Should().Contain(contextData);
        }

        [Fact]
        public void LogCriticalWithWritesLogWithEventIdExceptionAndNullContextData()
        {
            var eventId = new EventId(Environment.TickCount);
            var exception = new TimeoutException();

            _logger.LogCriticalWithContext(eventId, exception, null);

            var actual = _logger.Last;

            actual.LogLevel.Should().Be(LogLevel.Critical);
            actual.EventId.Id.Should().Be(eventId.Id);
            actual.Message.Should().BeNull();
            actual.Exception.Should().Be(exception);
            actual.Exception.Data.Should().BeEmpty();
        }

        [Fact]
        public void LogCriticalWithWritesLogWithEventIdExceptionContextDataAndMessage()
        {
            var eventId = new EventId(Environment.TickCount);
            var exception = new TimeoutException();
            var contextData = Guid.NewGuid().ToString();
            const string Message = "{0}-{1}";
            var args = new object[]
            {
                123, true
            };

            _logger.LogCriticalWithContext(eventId, exception, contextData, Message, args);

            var actual = _logger.Last;

            actual.LogLevel.Should().Be(LogLevel.Critical);
            actual.EventId.Id.Should().Be(eventId.Id);
            actual.Message.Should().Be("123-True");
            actual.Exception.Should().Be(exception);
            actual.Exception.Data["ContextData"].ToString().Should().Contain(contextData);
        }

        [Fact]
        public void LogCriticalWithWritesLogWithEventIdExceptionNullContextDataAndMessage()
        {
            var eventId = new EventId(Environment.TickCount);
            var exception = new TimeoutException();
            const string Message = "{0}-{1}";
            var args = new object[]
            {
                123, true
            };

            _logger.LogCriticalWithContext(eventId, exception, null, Message, args);

            var actual = _logger.Last;

            actual.LogLevel.Should().Be(LogLevel.Critical);
            actual.EventId.Id.Should().Be(eventId.Id);
            actual.Message.Should().Be("123-True");
            actual.Exception.Should().Be(exception);
            actual.Exception.Data.Should().BeEmpty();
        }

        [Fact]
        public void LogCriticalWithWritesLogWithExceptionAndContextData()
        {
            var exception = new TimeoutException();
            var contextData = Guid.NewGuid().ToString();

            _logger.LogCriticalWithContext(exception, contextData);

            var actual = _logger.Last;

            actual.LogLevel.Should().Be(LogLevel.Critical);
            actual.EventId.Id.Should().Be(0);
            actual.Message.Should().BeNull();
            actual.Exception.Should().Be(exception);
            actual.Exception.Data["ContextData"].ToString().Should().Contain(contextData);
        }

        [Fact]
        public void LogCriticalWithWritesLogWithExceptionAndNullContextData()
        {
            var exception = new TimeoutException();

            _logger.LogCriticalWithContext(exception, null);

            var actual = _logger.Last;

            actual.LogLevel.Should().Be(LogLevel.Critical);
            actual.EventId.Id.Should().Be(0);
            actual.Message.Should().BeNull();
            actual.Exception.Should().Be(exception);
            actual.Exception.Data.Should().BeEmpty();
        }

        [Fact]
        public void LogCriticalWithWritesLogWithExceptionContextDataAndMessage()
        {
            var exception = new TimeoutException();
            var contextData = Guid.NewGuid().ToString();
            const string Message = "{0}-{1}";
            var args = new object[]
            {
                123, true
            };

            _logger.LogCriticalWithContext(exception, contextData, Message, args);

            var actual = _logger.Last;

            actual.LogLevel.Should().Be(LogLevel.Critical);
            actual.EventId.Id.Should().Be(0);
            actual.Message.Should().Be("123-True");
            actual.Exception.Should().Be(exception);
            actual.Exception.Data["ContextData"].ToString().Should().Contain(contextData);
        }

        [Fact]
        public void LogCriticalWithWritesLogWithExceptionNullContextDataAndMessage()
        {
            var exception = new TimeoutException();
            const string Message = "{0}-{1}";
            var args = new object[]
            {
                123, true
            };

            _logger.LogCriticalWithContext(exception, null, Message, args);

            var actual = _logger.Last;

            actual.LogLevel.Should().Be(LogLevel.Critical);
            actual.EventId.Id.Should().Be(0);
            actual.Message.Should().Be("123-True");
            actual.Exception.Should().Be(exception);
            actual.Exception.Data.Should().BeEmpty();
        }

        [Fact]
        public void LogErrorWithContextThrowsExceptionWithEventIdNullExceptionAndContextData()
        {
            var eventId = new EventId(Environment.TickCount);
            var contextData = Guid.NewGuid().ToString();

            Action action = () => _logger.LogErrorWithContext(eventId, null, contextData);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void LogErrorWithContextThrowsExceptionWithEventIdNullExceptionContextDataAndMessage()
        {
            var eventId = new EventId(Environment.TickCount);
            var contextData = Guid.NewGuid().ToString();
            const string Message = "{0}-{1}";
            var args = new object[]
            {
                123, true
            };

            Action action = () => _logger.LogErrorWithContext(eventId, null, contextData, Message, args);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void LogErrorWithContextThrowsExceptionWithNullExceptionAndContextData()
        {
            var contextData = Guid.NewGuid().ToString();

            Action action = () => _logger.LogErrorWithContext(null, contextData);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void LogErrorWithContextThrowsExceptionWithNullExceptionContextDataAndMessage()
        {
            var contextData = Guid.NewGuid().ToString();
            const string Message = "{0}-{1}";
            var args = new object[]
            {
                123, true
            };

            Action action = () => _logger.LogErrorWithContext(null, contextData, Message, args);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void LogErrorWithContextThrowsExceptionWithNullLoggerEventIdExceptionAndContextData()
        {
            var eventId = new EventId(Environment.TickCount);
            var exception = new TimeoutException();
            var contextData = Guid.NewGuid().ToString();

            var log = (ILogger)null;

            Action action = () => log.LogErrorWithContext(eventId, exception, contextData);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void LogErrorWithContextThrowsExceptionWithNullLoggerEventIdExceptionContextDataAndMessage()
        {
            var eventId = new EventId(Environment.TickCount);
            var exception = new TimeoutException();
            var contextData = Guid.NewGuid().ToString();
            const string Message = "{0}-{1}";
            var args = new object[]
            {
                123, true
            };

            var log = (ILogger)null;

            Action action = () => log.LogErrorWithContext(eventId, exception, contextData, Message, args);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void LogErrorWithContextThrowsExceptionWithNullLoggerExceptionAndContextData()
        {
            var exception = new TimeoutException();
            var contextData = Guid.NewGuid().ToString();

            var log = (ILogger)null;

            Action action = () => log.LogErrorWithContext(exception, contextData);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void LogErrorWithContextThrowsExceptionWithNullLoggerExceptionContextDataAndMessage()
        {
            var exception = new TimeoutException();
            var contextData = Guid.NewGuid().ToString();
            const string Message = "{0}-{1}";
            var args = new object[]
            {
                123, true
            };

            var log = (ILogger)null;

            Action action = () => log.LogErrorWithContext(exception, contextData, Message, args);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void LogErrorWithWritesLogWithEventIdExceptionAndContextData()
        {
            var eventId = new EventId(Environment.TickCount);
            var exception = new TimeoutException();
            var contextData = Guid.NewGuid().ToString();

            _logger.LogErrorWithContext(eventId, exception, contextData);

            var actual = _logger.Last;

            actual.LogLevel.Should().Be(LogLevel.Error);
            actual.EventId.Id.Should().Be(eventId.Id);
            actual.Message.Should().BeNull();
            actual.Exception.Should().Be(exception);
            actual.Exception.Data["ContextData"].ToString().Should().Contain(contextData);
        }

        [Fact]
        public void LogErrorWithWritesLogWithEventIdExceptionAndNullContextData()
        {
            var eventId = new EventId(Environment.TickCount);
            var exception = new TimeoutException();

            _logger.LogErrorWithContext(eventId, exception, null);

            var actual = _logger.Last;

            actual.LogLevel.Should().Be(LogLevel.Error);
            actual.EventId.Id.Should().Be(eventId.Id);
            actual.Message.Should().BeNull();
            actual.Exception.Should().Be(exception);
            actual.Exception.Data.Should().BeEmpty();
        }

        [Fact]
        public void LogErrorWithWritesLogWithEventIdExceptionContextDataAndMessage()
        {
            var eventId = new EventId(Environment.TickCount);
            var exception = new TimeoutException();
            var contextData = Guid.NewGuid().ToString();
            const string Message = "{0}-{1}";
            var args = new object[]
            {
                123, true
            };

            _logger.LogErrorWithContext(eventId, exception, contextData, Message, args);

            var actual = _logger.Last;

            actual.LogLevel.Should().Be(LogLevel.Error);
            actual.EventId.Id.Should().Be(eventId.Id);
            actual.Message.Should().Be("123-True");
            actual.Exception.Should().Be(exception);
            actual.Exception.Data["ContextData"].ToString().Should().Contain(contextData);
        }

        [Fact]
        public void LogErrorWithWritesLogWithEventIdExceptionNullContextDataAndMessage()
        {
            var eventId = new EventId(Environment.TickCount);
            var exception = new TimeoutException();
            const string Message = "{0}-{1}";
            var args = new object[]
            {
                123, true
            };

            _logger.LogErrorWithContext(eventId, exception, null, Message, args);

            var actual = _logger.Last;

            actual.LogLevel.Should().Be(LogLevel.Error);
            actual.EventId.Id.Should().Be(eventId.Id);
            actual.Message.Should().Be("123-True");
            actual.Exception.Should().Be(exception);
            actual.Exception.Data.Should().BeEmpty();
        }

        [Fact]
        public void LogErrorWithWritesLogWithExceptionAndContextData()
        {
            var exception = new TimeoutException();
            var contextData = Guid.NewGuid().ToString();

            _logger.LogErrorWithContext(exception, contextData);

            var actual = _logger.Last;

            actual.LogLevel.Should().Be(LogLevel.Error);
            actual.EventId.Id.Should().Be(0);
            actual.Message.Should().BeNull();
            actual.Exception.Should().Be(exception);
            actual.Exception.Data["ContextData"].ToString().Should().Contain(contextData);
        }

        [Fact]
        public void LogErrorWithWritesLogWithExceptionAndNullContextData()
        {
            var exception = new TimeoutException();

            _logger.LogErrorWithContext(exception, null);

            var actual = _logger.Last;

            actual.LogLevel.Should().Be(LogLevel.Error);
            actual.EventId.Id.Should().Be(0);
            actual.Message.Should().BeNull();
            actual.Exception.Should().Be(exception);
            actual.Exception.Data.Should().BeEmpty();
        }

        [Fact]
        public void LogErrorWithWritesLogWithExceptionContextDataAndMessage()
        {
            var exception = new TimeoutException();
            var contextData = Guid.NewGuid().ToString();
            const string Message = "{0}-{1}";
            var args = new object[]
            {
                123, true
            };

            _logger.LogErrorWithContext(exception, contextData, Message, args);

            var actual = _logger.Last;

            actual.LogLevel.Should().Be(LogLevel.Error);
            actual.EventId.Id.Should().Be(0);
            actual.Message.Should().Be("123-True");
            actual.Exception.Should().Be(exception);
            actual.Exception.Data["ContextData"].ToString().Should().Contain(contextData);
        }

        [Fact]
        public void LogErrorWithWritesLogWithExceptionNullContextDataAndMessage()
        {
            var exception = new TimeoutException();
            const string Message = "{0}-{1}";
            var args = new object[]
            {
                123, true
            };

            _logger.LogErrorWithContext(exception, null, Message, args);

            var actual = _logger.Last;

            actual.LogLevel.Should().Be(LogLevel.Error);
            actual.EventId.Id.Should().Be(0);
            actual.Message.Should().Be("123-True");
            actual.Exception.Should().Be(exception);
            actual.Exception.Data.Should().BeEmpty();
        }
    }
}