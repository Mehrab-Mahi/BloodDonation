using AssetPro.Application.Helper;
using AssetPro.Application.Interfaces;
using AssetPro.Application.ViewModels;
using AssetPro.Domain.Entities;
using AssetPro.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace AssetPro.Application.Services
{
    public class AuthService : IAuthService
    {
        private IUserService _userService;
        private readonly AppSettings _appSettings;
        private IHttpContextAccessor _httpContextAccessor;
        private readonly IBaseRepository _repo;
        private readonly IRepository<AccessControl> _accessRepo;

        public AuthService(IUserService userService, IOptions<AppSettings> appSettings, IHttpContextAccessor httpContextAccessor, IBaseRepository repo, IRepository<AccessControl> accessRepo)
        {
            _userService = userService;
            _appSettings = appSettings.Value;
            _httpContextAccessor = httpContextAccessor;
            _repo = repo;
            _accessRepo = accessRepo;
        }

        public PayloadResponse Authenticate(AuthRequest model)
        {
            var user = _userService.Get(model.Email);
            if (user == null)
            {
                return new PayloadResponse
                {
                    IsSuccess = false,
                    PayloadType = "authentication",
                    Content = null,
                    Message = "authentication unsuccessful.User not found!"
                };
            }
            if (!user.IsApproved)
            {
                return new PayloadResponse
                {
                    IsSuccess = false,
                    PayloadType = "authentication",
                    Content = null,
                    Message = "authentication unsuccessful.User not Approved!"
                };
            }
            var isVerified = VerifyPassword(model.Password, user.PasswordHash);
            if (!isVerified)
            {
                return new PayloadResponse
                {
                    IsSuccess = false,
                    PayloadType = "authentication",
                    Content = null,
                    Message = "authentication unsuccessful.Password does not match"
                };
            }
            var token = GenerateJwtToken(user);
            return new PayloadResponse
            {
                IsSuccess = true,
                PayloadType = "authentication",
                Content = new AuthResponse(token),
                Message = "authentication successful"
            };
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, (user.FirstName.ToString() +" "+ user.LastName.ToString())),
                    new Claim(ClaimTypes.Email, user.EmailAddress.ToString()),
                    new Claim(type: "UserId", user.Id.ToString()),
                    new Claim(type: "RoleId", user.RoleId.ToString()),
                    new Claim(type: "IsSuperAdmin", user.IsSuperAdmin.ToString()),
                    new Claim(type: "UserName", user.UserName.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(20),
                SigningCredentials = credentials
            };
            var tokenValue = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(tokenValue);
            _httpContextAccessor.HttpContext.Session.SetString("token", token);
            _httpContextAccessor.HttpContext.Session.SetString("userName", user.UserName.ToString());
            return token;
        }

        public UserAuthVm ValidateToken(string authToken)
        {
            try
            {
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = JwtAuthentication.GetValidatorParameters(key);

                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(authToken, validationParameters, out validatedToken);
                return FillAuthClaims(principal);
            }
            catch (Exception)
            {
                return new UserAuthVm { IsAuthenticate = false };
            }
        }

        private UserAuthVm FillAuthClaims(ClaimsPrincipal principal)
        {
            if (principal.Identity.IsAuthenticated)
            {
                var email = principal.Claims.Where(_ => _.Type == ClaimTypes.Email).FirstOrDefault().Value;
                var name = principal.Claims.Where(_ => _.Type == ClaimTypes.Name).FirstOrDefault().Value;
                var id = principal.Claims.Where(_ => _.Type == "UserId").FirstOrDefault().Value;
                var roleId = principal.Claims.Where(_ => _.Type == "RoleId").FirstOrDefault().Value;
                var isSuperAdmin = principal.Claims.Where(_ => _.Type == "IsSuperAdmin").FirstOrDefault().Value;
                var userName = principal.Claims.Where(_ => _.Type == "UserName").FirstOrDefault().Value;
                return new UserAuthVm
                {
                    IsAuthenticate = true,
                    EmailAddress = email,
                    Name = name,
                    UserName = userName,
                    Id = id,
                    RoleId = roleId,
                    IsSuperAdmin = Convert.ToBoolean(isSuperAdmin)
                };
            }
            else
            {
                return new UserAuthVm { IsAuthenticate = false };
            }
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }

        public UserAuthVm GetCurrentUser()
        {
            return _httpContextAccessor.HttpContext.Session.GetObject<UserAuthVm>("Auth");
        }

        public List<AccessControlVm> GetUserMenu(string roleId)
        {
            var query = string.Empty;
            if (GetCurrentUser().IsSuperAdmin)
            {
                query = query + @"select ac.Id,ac.Name,ac.ParentId,ac.Type,ac.Url,ac.MenuId,ac.Icon
                               from AccessControls ac";
            }
            else
            {
                query = query + @$"select ac.Id,ac.Name,ac.ParentId,ac.Type,ac.Url,ac.MenuId,ac.Icon,ac.SortOrder
                        from MenuCruds mc
                        join AccessControls ac on mc.AccessControlId = ac.Id where mc.RoleId = '{roleId}'; ";
            }
            return BuildMenuTree(_repo.Query<AccessControlVm>(query));
        }

        private List<AccessControlVm> BuildMenuTree(List<AccessControlVm> accessControlVms)
        {
            var list = new List<AccessControlVm>();
            foreach (var item in accessControlVms)
            {
                if (item.ParentId != "#")
                {
                    var parent = accessControlVms.Where(_ => _.Id == item.ParentId).FirstOrDefault();
                    if (parent == null)
                    {
                        var innerParent = _accessRepo.Find(item.ParentId);

                        parent = new AccessControlVm
                        {
                            Id = innerParent.Id,
                            ParentId = innerParent.ParentId,
                            Child = new List<AccessControlVm>(),
                            Name = innerParent.Name,
                            Type = innerParent.Type,
                            Url = innerParent.Url,
                            Icon = innerParent.Icon,
                            MenuId = innerParent.MenuId,
                            SortOrder = innerParent.SortOrder
                        };

                        parent.Child.Add(item);
                        list.Add(parent);
                    }
                    else
                    {
                        var child = parent.Child.Where(_ => _.Id == item.Id).FirstOrDefault();
                        if (child == null)
                        {
                            parent.Child.Add(item);
                            var chkParent = list.Where(_ => _.Id == parent.Id).FirstOrDefault();
                            if (chkParent == null)
                            {
                                list.Add(parent);
                            }
                        }
                        else
                        {
                        }
                    }
                }
                else
                {
                    var childList = accessControlVms.Where(_ => _.ParentId == item.Id).ToList();
                    foreach (var childItem in childList)
                    {
                        if (!item.Child.Contains(childItem))
                        {
                            item.Child.Add(childItem);
                        }
                    }
                    var chkParent = list.Where(_ => _.Id == item.Id).FirstOrDefault();
                    if (chkParent == null)
                    {
                        list.Add(item);
                    }
                }
            }
            return list.GroupBy(x => x.Id).Select(y => y.First()).ToList();
        }
    }
}