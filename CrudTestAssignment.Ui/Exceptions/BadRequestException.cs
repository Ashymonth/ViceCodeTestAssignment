namespace CrudTestAssignment.Ui.Exceptions
{
    public class BadRequestException : ServerRequestException
    {
        public BadRequestException(string message) : base(message)
        {
        }
    }
}