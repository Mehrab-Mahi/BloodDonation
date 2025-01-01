using AssetPro.Application.Helper;
using AssetPro.Application.Interfaces;
using AssetPro.Application.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AssetPro.Web.Controllers
{
    [AssetProAuth]
    [Route("users")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;

        public UserController(IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] UserCreationVm model)
        {
            var data = _userService.Insert(model);
            return Ok(new { data });
        }

        [HttpPut("update/{id}")]
        public IActionResult Update(string id, [FromBody] UserVm model)
        {
            var data = _userService.Update(id, model);
            return Ok(new { data });
        }

        [HttpGet("getall")]
        public IActionResult GetAll()
        {
            var list = _userService.GetAll();
            return Ok(new { data = list });
        }

        [HttpGet("getbyid/{id}")]
        public IActionResult GetById(string id)
        {
            var list = _userService.GetById(id);
            return Ok(new { data = list });
        }

        [HttpGet("getusermenu/{id}")]
        public IActionResult GetUserMenu(string id)
        {
            var data = _authService.GetUserMenu(id);
            return Ok(data);
        }
    }
}