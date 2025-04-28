namespace Core.Abstractions.Exceptions;

public class DomainException : Exception
{
    public string ErrorCode { get; }

    public DomainException(string message, string errorCode = "Domain.Error") 
        : base(message)
    {
        ErrorCode = errorCode;
    }
}
