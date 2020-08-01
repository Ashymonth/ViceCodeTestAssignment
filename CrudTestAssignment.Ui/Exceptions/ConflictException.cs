namespace CrudTestAssignment.Ui.Exceptions
{
    public class ConflictException : ServerRequestException
    {
        public ConflictException(string message) : base(message)
        {
        }
    }
}