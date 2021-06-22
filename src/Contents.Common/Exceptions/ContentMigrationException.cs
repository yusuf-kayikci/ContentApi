using System;

namespace Contents.Common.Exceptions
{
    public class ContentMigrationException : Exception
    {
        public ContentMigrationException(string message) : base(message)
        {
        }
    }
}
