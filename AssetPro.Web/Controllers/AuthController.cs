using BloodDonation.Application.Interfaces;
using BloodDonation.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonation.Web.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("token")]
        public IActionResult Token([FromBody] AuthRequest model)
        {
            var response = _authService.Authenticate(model);
            return Ok(response);
        }

        [Authorize]
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("currentuser")]
        public IActionResult GetCurrentUser()
        {
            return Ok(_authService.GetCurrentUser());
        }
    }
}