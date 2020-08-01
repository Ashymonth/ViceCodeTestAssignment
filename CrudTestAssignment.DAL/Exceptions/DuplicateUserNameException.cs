using System;

namespace CrudTestAssignment.DAL.Exceptions
{
    public class DuplicateUserNameException : Exception
    {
        public DuplicateUserNameException(string message) : base(message)
        {
            
        }
    }
}