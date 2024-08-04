using Microsoft.AspNetCore.Mvc;
using ProgramGuard.Base;
using ProgramGuard.Data;
using ProgramGuard.Dtos.Auth;
using ProgramGuard.Dtos.User;
using ProgramGuard.Interface.Repository;
using ProgramGuard.Interface.Service;

namespace ProgramGuard.Controllers
{
    public class AuthController : BaseController
    {
        private readonly IPasswordHasherService _passwordHasherService;
        public AuthController(ProgramGuardContext context, ILogger<BaseController> logger, IUserRepository userRepository, IOperateLogRepository operateLogRepository, IPasswordHasherService passwordHasherService) : base(context, logger, userRepository, operateLogRepository)
        {
            _passwordHasherService = passwordHasherService;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userRepository.GetByAccountAsync(loginDto.Account);
            if (user == null || !_passwordHasherService.VerifyPassword(user.Password, loginDto.Password))
            {
                return Unauthorized();
            }
            if (await _user) {

            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }
    }
}
