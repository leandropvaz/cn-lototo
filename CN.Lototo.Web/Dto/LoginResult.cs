namespace CN.Lototo.Web.Dto
{
    public class LoginResult
    {
        public bool Success { get; init; }
        public string? ErrorMessage { get; init; }

        public static LoginResult Ok() => new() { Success = true };
        public static LoginResult Fail(string message) => new() { Success = false, ErrorMessage = message };
    }
}
