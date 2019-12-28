namespace Divergic.Logging.UnitTests.Models
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public class ToStringFailure : SerializeFailure
    {
        [SuppressMessage(
            "Design",
            "CA1065:Do not raise exceptions in unexpected locations",
            Justification = "The purpose is to test a failure")]
        public override string ToString()
        {
            throw new InvalidOperationException();
        }
    }
}