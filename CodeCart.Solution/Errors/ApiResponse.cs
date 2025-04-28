namespace CodeCart.API.Errors;

public class ApiResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; }

    public ApiResponse(int statusCode, string? message = null)
    {
        this.StatusCode = statusCode;

        this.Message = message ?? GetDefaultMessageForStatusCode(statusCode);
    }

    private string? GetDefaultMessageForStatusCode(int statusCode)
    {
        return statusCode switch
        {
            400 => "A Bad Request, You have made",
            401 => "Authorized, you are not",
            404 => "Resource was not found",
            500 => "Errors are the path to the dark side. Errors kead to anger. Anger leads to hate. Hate leads to career change.",
            _ => null
        };
    }
}
