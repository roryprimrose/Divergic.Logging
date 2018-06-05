namespace Divergic.Logging
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using EnsureThat;
    using Newtonsoft.Json;

    /// <summary>
    /// The <see cref="ExceptionData"/>
    /// class provides extension methods for the <see cref="Exception"/> class.
    /// </summary>
    public static class ExceptionData
    {
        private const string ContextDataKey = "ContextData";

        /// <summary>
        /// Adds context data to the specified exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="data">The context data.</param>
        /// <returns>The exception with context data appended.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="data"/> is <c>null</c>.</exception>
        public static Exception AddContextData(this Exception exception, object data)
        {
            return AddSerializedData(exception, ContextDataKey, data);
        }

        /// <summary>
        /// Adds the specified data to the exception as a JSON serialized value.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="key">The key used to identify the data.</param>
        /// <param name="data">The data to store.</param>
        /// <returns>The exception with context data appended.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="key"/> is <c>null</c> or <see cref="string.Empty"/>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="data"/> is <c>null</c>.</exception>
        public static Exception AddSerializedData(this Exception exception, string key, object data)
        {
            Ensure.Any.IsNotNull(exception, nameof(exception));
            Ensure.String.IsNotNullOrEmpty(key, nameof(key));
            Ensure.Any.IsNotNull(data, nameof(data));

            if (HasSerializedData(exception, key))
            {
                return exception;
            }

            var convertedData = ConvertData(data);

            if (convertedData != null)
            {
                // The conversion may have found that there was nothing of value to report
                exception.Data.Add(key, convertedData);
            }

            return exception;
        }

        /// <summary>
        /// Gets whether the exception contains data stored for the specified key.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="key">The key used to store the data.</param>
        /// <returns><c>true</c> if the exception contains data for the key; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="key"/> is <c>null</c> or <see cref="string.Empty"/>.</exception>
        public static bool HasSerializedData(this Exception exception, string key)
        {
            Ensure.Any.IsNotNull(exception, nameof(exception));
            Ensure.String.IsNotNullOrEmpty(key, nameof(key));

            return exception.Data.Contains(key);
        }

        private static JsonSerializerSettings BuildSerializerSettings()
        {
            var settings = new JsonSerializerSettings
            {
                DateParseHandling = DateParseHandling.None,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                Formatting = Formatting.None
            };

            return settings;
        }

        private static object ConvertData(object data)
        {
            Debug.Assert(data != null, "No data provided");

            if (data.GetType().GetTypeInfo().IsValueType
                && data.GetType().Namespace == nameof(System))
            {
                // JSON serialization of the System value types is well supported
                return data;
            }

            if (data is string dataAsString)
            {
                if (string.IsNullOrWhiteSpace(dataAsString))
                {
                    return null;
                }

                return dataAsString;
            }

            try
            {
                // The type is either a reference type or a value type not from the System namespace.
                // It is not likely that the value is suitable for pushing straight onto the exception data property for error logging
                // We will use explicit JSON serialization to put the data into a state where it is more likely to work with logging targets
                var serializedData = JsonConvert.SerializeObject(data, SerializerSettings);

                if (serializedData == "{}")
                {
                    // The serialization returned an empty value so we will just log whatever ToString gives us
                    return data.ToString();
                }

                return serializedData;
            }
            catch (Exception)
            {
                try
                {
                    return data.ToString();
                }
                catch (Exception)
                {
                    // Nothing we can do here in terms to recording the exception
                    // We have to swallow the exception here so we don't bring down the process or cause
                    // a potential infinite loop logging exceptions that fail to log which logs the failure which fails to......
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the default serializer settings used to append context data to exceptions.
        /// </summary>
        public static JsonSerializerSettings DefaultSerializerSettings => BuildSerializerSettings();

        /// <summary>
        /// Gets or sets the serializer settings used to append context data to exceptions.
        /// </summary>
        public static JsonSerializerSettings SerializerSettings { get; set; } = DefaultSerializerSettings;
    }
}