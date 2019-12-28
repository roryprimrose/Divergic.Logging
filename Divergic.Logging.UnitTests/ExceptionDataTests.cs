namespace Divergic.Logging.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Divergic.Logging.UnitTests.Models;
    using FluentAssertions;
    using global::Xunit;
    using global::Xunit.Abstractions;
    using ModelBuilder;
    using Newtonsoft.Json;
    using NodaTime;
    using NodaTime.Serialization.JsonNet;

    public class ExceptionDataTests
    {
        private readonly ITestOutputHelper _output;

        static ExceptionDataTests()
        {
            ExceptionData.SerializerSettings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
        }

        public ExceptionDataTests(ITestOutputHelper output)
        {
            _output = output;
        }

        public static IEnumerable<object[]> ValueTypeValues()
        {
            yield return new object[] {Environment.TickCount};
            yield return new object[] {true};
            yield return new object[] {false};
            yield return new object[] {DateTimeOffset.UtcNow};
            yield return new object[] {Guid.NewGuid()};
        }

        [Fact]
        public void AddContextDataAppendsSerializedValueToException()
        {
            var value = Model.Create<Company>();
            var expected = JsonConvert.SerializeObject(value, ExceptionData.SerializerSettings);

            var sut = new TimeoutException();

            sut.AddContextData(value);

            var actual = sut.Data["ContextData"].As<string>();

            _output.WriteLine("Stored context data is: {0}", actual);

            actual.Should().Be(expected);
        }

        [Fact]
        public void AddContextDataThrowsExceptionWithNullData()
        {
            var sut = new TimeoutException();

            Action action = () => sut.AddContextData(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddContextDataThrowsExceptionWithNullException()
        {
            var value = Guid.NewGuid().ToString();
            var sut = (Exception) null;

            Action action = () => sut.AddContextData(value);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddSerializedDataAppendsDataToStringWhenSerializationFails()
        {
            var key = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);
            var value = new SerializeFailure();

            var sut = new TimeoutException();

            sut.AddSerializedData(key, value);

            var actual = sut.Data[key].As<string>();

            _output.WriteLine("Stored data is: {0}", actual);

            actual.Should().Be(typeof(SerializeFailure).FullName);
        }

        [Fact]
        public void AddSerializedDataAppendsDataToStringWhenSerializationReturnsEmptyJson()
        {
            var key = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);
            var value = new EmptyModel();

            var sut = new TimeoutException();

            sut.AddSerializedData(key, value);

            var actual = sut.Data[key].As<string>();

            _output.WriteLine("Stored data is: {0}", actual);

            actual.Should().Be(typeof(EmptyModel).FullName);
        }

        [Fact]
        public void AddSerializedDataAppendsSerializedNonSystemValueType()
        {
            var key = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);
            var value = SystemClock.Instance.GetCurrentInstant();
            var expected = JsonConvert.SerializeObject(value, ExceptionData.SerializerSettings);

            var sut = new TimeoutException();

            sut.AddSerializedData(key, value);

            var actual = sut.Data[key];

            _output.WriteLine("Stored data is: {0}", actual);

            actual.Should().Be(expected);
        }

        [Fact]
        public void AddSerializedDataAppendsSerializedValueToException()
        {
            var key = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);
            var value = Model.Create<Company>();
            var expected = JsonConvert.SerializeObject(value, ExceptionData.SerializerSettings);

            var sut = new TimeoutException();

            sut.AddSerializedData(key, value);

            var actual = sut.Data[key].As<string>();

            _output.WriteLine("Stored data is: {0}", actual);

            actual.Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(ValueTypeValues))]
        public void AddSerializedDataAppendsValueTypeValuesTest(object value)
        {
            var key = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);

            var sut = new TimeoutException();

            sut.AddSerializedData(key, value);

            var actual = sut.Data[key];

            _output.WriteLine("Stored data is: {0}", actual);

            actual.Should().Be(value);
        }

        [Fact]
        public void AddSerializedDataDoesNotAppendValueWhenDataAlreadyStored()
        {
            var key = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);
            var value = Guid.NewGuid().ToString();
            var nextValue = Guid.NewGuid().ToString();

            var sut = new TimeoutException();

            sut.AddSerializedData(key, value);
            sut.AddSerializedData(key, nextValue);

            var actual = sut.Data[key];

            _output.WriteLine("Stored data is: {0}", actual);

            actual.Should().Be(value);
        }

        [Fact]
        public void AddSerializedDataDoesNotAppendValueWhenSerializationAndToStringBothFail()
        {
            var key = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);
            var value = new ToStringFailure();

            var sut = new TimeoutException();

            sut.AddSerializedData(key, value);

            sut.Data.Contains(key).Should().BeFalse();
        }

        [Fact]
        public void AddSerializedDataDoesNotIncludeNullNestedTypeInExceptionData()
        {
            var key = Guid.NewGuid().ToString();
            var value = Model.Ignoring<Company>(x => x.Owner).Create<Company>();

            var sut = new TimeoutException();

            sut.AddSerializedData(key, value);

            var expected = sut.Data[key] as string;

            expected.Should().NotContain(typeof(SerializeFailure).FullName);
        }

        [Fact]
        public void AddSerializedDataIgnoresFailureToReadPropertiesForExceptionData()
        {
            var key = Guid.NewGuid().ToString();
            var value = new SerializeFailure
                {Name = Guid.NewGuid().ToString()};

            var sut = new TimeoutException();

            sut.AddSerializedData(key, value);

            var expected = sut.Data[key] as string;

            expected.Should().Be(typeof(SerializeFailure).FullName);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("  ", false)]
        [InlineData("Stuff", true)]
        public void AddSerializedDataOnlyAppendsStringsWithContentsTest(string value, bool expected)
        {
            var key = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);

            var sut = new TimeoutException();

            sut.AddSerializedData(key, value);

            var actual = sut.Data[key];

            _output.WriteLine("Stored data is: {0}", actual);

            if (expected)
            {
                actual.Should().Be(value);
            }
            else
            {
                sut.Data.Contains(key).Should().BeFalse();
                actual.Should().BeNull();
            }
        }

        [Fact]
        public void AddSerializedDataThrowsExceptionWithNullData()
        {
            var key = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);

            var sut = new TimeoutException();

            Action action = () => sut.AddSerializedData(key, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddSerializedDataThrowsExceptionWithNullException()
        {
            var key = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);
            var value = Guid.NewGuid().ToString();
            var sut = (Exception) null;

            Action action = () => sut.AddSerializedData(key, value);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("   ", true)]
        [InlineData("Mykey", true)]
        public void AddSerializedDataValidatesKeyValueTest(string key, bool isValid)
        {
            var value = Guid.NewGuid().ToString();

            var sut = new TimeoutException();

            Action action = () => sut.AddSerializedData(key, value);

            if (isValid)
            {
                action.Should().NotThrow();
            }
            else
            {
                action.Should().Throw<ArgumentException>();
            }
        }

        [Fact]
        public void DefaultSerializerSettingsReturnsNewDefaultValue()
        {
            ExceptionData.DefaultSerializerSettings.MaxDepth = Environment.TickCount;

            ExceptionData.DefaultSerializerSettings.MaxDepth.Should().BeNull();
        }

        [Fact]
        public void HasSerializedDataReturnsWhetherExceptionContainsData()
        {
            var key = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);
            var value = Guid.NewGuid().ToString();
            var keyNotFound = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);

            var sut = new TimeoutException();

            sut.Data.Add(key, value);

            sut.HasSerializedData(key).Should().BeTrue();
            sut.HasSerializedData(keyNotFound).Should().BeFalse();
        }

        [Fact]
        public void HasSerializedDataThrowsExceptionWithNullException()
        {
            var key = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);

            var sut = (Exception) null;

            Action action = () => sut.HasSerializedData(key);

            action.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("  ", true)]
        [InlineData("Mykey", true)]
        public void HasSerializedDataValidatesKeyValueTest(string key, bool isValid)
        {
            var sut = new TimeoutException();

            Action action = () => sut.HasSerializedData(key);

            if (isValid)
            {
                action.Should().NotThrow();
            }
            else
            {
                action.Should().Throw<ArgumentException>();
            }
        }

        [Fact]
        public void SerializerSettingsCanAssignDefaultSerializerSettings()
        {
            ExceptionData.SerializerSettings.MaxDepth = Environment.TickCount;

            ExceptionData.SerializerSettings = ExceptionData.DefaultSerializerSettings;

            ExceptionData.SerializerSettings.MaxDepth.Should().BeNull();
        }
    }
}