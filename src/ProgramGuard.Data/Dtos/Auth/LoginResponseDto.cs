namespace ProgramGuard.Data.Dtos.Auth
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;

        public string? Message { get; set; } = string.Empty;
    }
}
