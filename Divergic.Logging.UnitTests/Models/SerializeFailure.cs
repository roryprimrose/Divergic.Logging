namespace Divergic.Logging.UnitTests.Models
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage(
        "Design",
        "CA1065:Do not raise exceptions in unexpected locations",
        Justification = "The purpose is to test a failure")]
    public class SerializeFailure
    {
        private string _name;

        public string Name { get => throw new InvalidOperationException(); set => _name = value; }
    }
}