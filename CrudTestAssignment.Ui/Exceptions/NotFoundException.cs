namespace CrudTestAssignment.Ui.Exceptions
{
    public class NotFoundException : ServerRequestException
    {
        public NotFoundException(string message) : base(message)
        {

        }
    }
}