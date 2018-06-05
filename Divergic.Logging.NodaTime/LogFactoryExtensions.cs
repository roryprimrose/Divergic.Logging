namespace Microsoft.Extensions.Logging
{
    using System;
    using Divergic.Logging;
    using EnsureThat;
    using NodaTime;
    using NodaTime.Serialization.JsonNet;

    /// <summary>
    ///     The <see cref="LogFactoryExtensions" />
    ///     class provides extension methods for managing a <see cref="ILoggerFactory" />.
    /// </summary>
    public static class LogFactoryExtensions
    {
        /// <summary>
        /// Configures serialization of <see cref="ExceptionData"/> to support <see cref="NodaTime"/>
        /// data types when adding context data when logging exceptions.
        /// </summary>
        /// <param name="factory">The logger factory.</param>
        /// <returns>The logger factory</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="factory"/> is <c>null</c>.</exception>
        public static ILoggerFactory UsingNodaTimeTypes(this ILoggerFactory factory)
        {
            Ensure.Any.IsNotNull(factory, nameof(factory));

            ExceptionData.SerializerSettings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);

            return factory;
        }
    }
}