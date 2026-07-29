namespace Licentra.API.Exceptions.Custom
{
    public class ConflictException : Exception
    {
        public ConflictException(string message)
            : base(message)
        {
        }
    }
}