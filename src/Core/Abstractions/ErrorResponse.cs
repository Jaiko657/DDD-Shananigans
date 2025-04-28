namespace Core.Abstractions;

public class ErrorResponse
{
    public string ErrorCode { get; }
    public string Description { get; }

    public ErrorResponse(string errorCode, string description)
    {
        ErrorCode = errorCode;
        Description = description;
    }
}
