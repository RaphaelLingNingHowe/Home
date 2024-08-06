namespace ProgramGuard.Dtos.Auth
{
    public class LoginResponseDto
    {
        public bool RequirePasswordChange { get; set; }
        public string Token { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
