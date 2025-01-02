using BloodDonation.Application.Interfaces;
using BloodDonation.Application.ViewModels;
using BloodDonation.Domain.Entities;
using BloodDonation.Domain.Interfaces;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BloodDonation.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepo;
        private readonly IRepository<Role> _roleRepo;
        private readonly IMapper _mapper;

        public UserService(IRepository<User> userRepo, IRepository<Role> roleRepo, IMapper mapper)
        {
            _userRepo = userRepo;
            _roleRepo = roleRepo;
            _mapper = mapper;
        }

        public User Get(AuthRequest model)
        {
            var user = _userRepo.GetConditional(u => u.MobileNumber == model.MobileNumber && u.DateOfBirth == model.MobileNumber); 

            return user;
        }

        public List<UserVm> GetAll()
        {
            var list = new List<UserVm>();
            var users = _userRepo.GetAll().Where(_ => _.IsApproved == true).ToList();
            list = _mapper.Map(users, list);

            foreach (var item in list)
            {
                item.RoleName = string.IsNullOrEmpty(item.RoleId) ? "No Role" : GetRoleName(item.RoleId);
            }
            return list;
        }

        private string GetRoleName(string roleId)
        {
            return _roleRepo.Find(roleId).Name;
        }

        public User GetById(string id)
        {
            return _userRepo.GetConditional(_ => _.Id == id);
        }

        public PayloadResponse Insert(UserCreationVm user)
        {
            try
            {
                var model = new User();
                model = _mapper.Map(user, model);

                if (model.UserType != "Admin")
                {
                    model.IsApproved = true;
                    user.Password = "123";
                }

                if (model.UserType == "Volunteer")
                {
                    model.IsApproved = false;
                }

                model.PasswordHash = GeneratePassword(user.Password);

                _userRepo.Insert(model);
                _userRepo.SaveChanges();

                return new PayloadResponse
                {
                    IsSuccess = true,
                    PayloadType = "User Creation",
                    Content = null,
                    Message = "User Creation has been successful"
                };
            }
            catch (Exception)
            {
                return new PayloadResponse
                {
                    IsSuccess = false,
                    PayloadType = "User Creation",
                    Content = null,
                    Message = "User Creation become unsuccessful"
                };
            }
        }

        private string GeneratePassword(string password)
        {
            var defaultPass = Guid.NewGuid().ToString("N");
            if (!string.IsNullOrEmpty(password))
            {
                defaultPass = password;
            }
            return BCrypt.Net.BCrypt.HashPassword(defaultPass, workFactor: 12);
        }

        public PayloadResponse Update(string id, UserVm user)
        {
            var model = _userRepo.GetConditional(_ => _.Id == id);
            try
            {
                model = _mapper.Map(user, model);

                _userRepo.Update(model);
                _userRepo.SaveChanges();

                return new PayloadResponse
                {
                    IsSuccess = true,
                    PayloadType = "User Update",
                    Content = null,
                    Message = "User Update successfull"
                };
            }
            catch (Exception)
            {
                return new PayloadResponse
                {
                    IsSuccess = false,
                    PayloadType = "User Update",
                    Content = null,
                    Message = "User Update become unsuccessfull"
                };
            }
        }

        private bool IsEmailExists(UserCreationVm user)
        {
            var model = _userRepo.GetConditional(_ => _.EmailAddress == user.EmailAddress);
            return model != null;
        }

        private bool IsUserNameExists(UserCreationVm user)
        {
            var model = _userRepo.GetConditional(_ => _.UserName == user.UserName);
            return model != null;
        }

        public bool Delete(string id, string table)
        {
            try
            {
                _userRepo.Delete(id);
                _userRepo.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public UserTypeResponse GetUserTypeByPhoneNumberAndDob(AuthRequest model)
        {
            var data = _userRepo.GetConditional(u =>
                u.MobileNumber == model.MobileNumber && u.DateOfBirth == model.DateOfBirth);

            if (data is null)
            {
                return new UserTypeResponse()
                {
                    IsSuccess = false,
                    Message = "Mobile number and date of birth doesn't match!"
                };
            }

            return new UserTypeResponse()
            {
                UserType = data.UserType,
                IsSuccess = true,
                Message = "Mobile number and date of birth has been matched!"
            };
        }
    }
}