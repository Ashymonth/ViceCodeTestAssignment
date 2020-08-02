using System;

namespace CrudTestAssignment.Ui.Exceptions
{
    public class ServerRequestException : Exception
    {
        public ServerRequestException()
        {
        }

        public ServerRequestException(string message)
            : base(message)
        {
        }

        public ServerRequestException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}