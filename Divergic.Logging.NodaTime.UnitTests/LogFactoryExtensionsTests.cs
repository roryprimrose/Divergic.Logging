namespace Divergic.Logging.NodaTime.UnitTests
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using Xunit;

    public class LogFactoryExtensionsTests
    {
        [Fact]
        public void UsingNodaTimeTypesConfiguresContextDataSerializerWithNodaTime()
        {
            var factory = Substitute.For<ILoggerFactory>();

            factory.UsingNodaTimeTypes();

            ExceptionData.SerializerSettings.Converters
                .Any(x => x.GetType().FullName.StartsWith("NodaTime.Serialization.JsonNet.", StringComparison.Ordinal)).Should().BeTrue();
        }

        [Fact]
        public void UsingNodaTimeTypesThrowsExceptionWithNullFactory()
        {
            ILoggerFactory factory = null;

            Action action = () => factory.UsingNodaTimeTypes();

            action.Should().Throw<ArgumentNullException>();
        }
    }
}