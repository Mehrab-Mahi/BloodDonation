using BloodDonation.Application.Helper;
using BloodDonation.Application.Interfaces;
using BloodDonation.Application.ViewModels;
using Microsoft.AspNetCore.Http;

namespace BloodDonation.Application.Services
{
    public class LoggedInUserService : ILoggedInUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoggedInUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public UserAuthVm GetLoggedInUser()
        {
            var authorization = _httpContextAccessor.HttpContext!.Request.Headers["Authorization"].ToString();

            return string.IsNullOrEmpty(authorization) ?
                null :
                _httpContextAccessor.HttpContext.Session.GetObject<UserAuthVm>("Auth");
        }
    }
}
