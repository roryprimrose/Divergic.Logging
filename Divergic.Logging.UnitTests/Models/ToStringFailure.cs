namespace Divergic.Logging.UnitTests.Models
{
    using System;

    public class ToStringFailure : SerializeFailure
    {
        public override string ToString()
        {
            throw new InvalidOperationException();
        }
    }
}