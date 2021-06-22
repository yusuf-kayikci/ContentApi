using System;

namespace Contents.Common.Exceptions
{
    public class ContentOperationException : Exception
    {
        public ContentOperationException(string message) : base(message)
        {
        }
    }
}
