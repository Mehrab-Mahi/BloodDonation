using BloodDonation.Application.Interfaces;
using BloodDonation.Application.ViewModels;
using BloodDonation.Domain.Entities;
using BloodDonation.Domain.Interfaces;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using BloodDonation.Application.Util;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace BloodDonation.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepo;
        private readonly IRepository<Role> _roleRepo;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;
        public UserService(IRepository<User> userRepo, IRepository<Role> roleRepo, IMapper mapper, IFileService fileService)
        {
            _userRepo = userRepo;
            _roleRepo = roleRepo;
            _mapper = mapper;
            _fileService = fileService;
        }

        public User Get(AuthRequest model)
        {
            var user = _userRepo.GetConditional(u => u.MobileNumber == model.MobileNumber && u.DateOfBirth == model.DateOfBirth); 

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
                var model = new User()
                {
                    FullName = user.FullName,
                    BloodGroup = user.BloodGroup,
                    DateOfBirth = user.DateOfBirth,
                    MobileNumber = user.MobileNumber,
                    District = user.District,
                    Upazila = user.Upazila,
                    Union = user.Union,
                    Address = user.Address,
                    FatherName = user.FatherName,
                    MotherName = user.MotherName,
                    BloodDonationStatus = user.BloodDonationStatus,
                    Gender = user.Gender,
                    UserType = user.UserType,
                    LastDonationTime = user.LastDonationTime,
                    ImageUrl = user.ImageUrl,
                    IsSuperAdmin = user.IsSuperAdmin,
                    BloodDonationCount = user.BloodDonationCount
                };

                if (model.UserType != UserTypes.Admin)
                {
                    model.IsApproved = true;
                    user.Password = "123";
                }

                if (model.UserType == UserTypes.Volunteer)
                {
                    model.IsApproved = false;
                }

                model.PasswordHash = GeneratePassword(user.Password);
                model.ImageUrl = UploadAndGetImageUrl(user.ProfilePicture);

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

        private string UploadAndGetImageUrl(IFormFile userProfilePicture)
        {
            if (userProfilePicture is null) return string.Empty;

            var fileName = GetFileName(userProfilePicture.FileName);
            var path = Path.Combine(_fileService.GetRootPath(), @"\ProfilePicture\");
            _fileService.CreateDirectoryIfNotExists(path);
            var filePath = Path.Combine(path, fileName);
            _fileService.SaveFile(filePath, userProfilePicture);
            return filePath;
        }

        private string GetFileName(string fileName)
        {
            return Guid.NewGuid().ToString("N") + "-" + fileName;
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

        public PayloadResponse Update(string id, UserCreationVm user)
        {
            var model = _userRepo.GetConditional(_ => _.Id == id);
            try
            {
                model.FullName = user.FullName;
                model.BloodGroup = user.BloodGroup;
                model.DateOfBirth = user.DateOfBirth;
                model.MobileNumber = user.MobileNumber;
                model.District = user.District;
                model.Upazila = user.Upazila;
                model.Union = user.Union;
                model.Address = user.Address;
                model.FatherName = user.FatherName;
                model.MotherName = user.MotherName;
                model.BloodDonationStatus = user.BloodDonationStatus;
                model.Gender = user.Gender;
                model.UserType = user.UserType;
                model.LastDonationTime = user.LastDonationTime;
                model.BloodDonationCount = user.BloodDonationCount;

                if (user.ProfilePicture is { Length: > 0 })
                {
                    _fileService.DeleteFile(model.ImageUrl);
                    model.ImageUrl = UploadAndGetImageUrl(user.ProfilePicture);
                }

                _userRepo.Update(model);
                _userRepo.SaveChanges();

                return new PayloadResponse
                {
                    IsSuccess = true,
                    PayloadType = "User Update",
                    Content = null,
                    Message = "User Update successful"
                };
            }
            catch (Exception)
            {
                return new PayloadResponse
                {
                    IsSuccess = false,
                    PayloadType = "User Update",
                    Content = null,
                    Message = "User Update become failed"
                };
            }
        }

        private bool IsEmailExists(UserVm user)
        {
            var model = _userRepo.GetConditional(_ => _.EmailAddress == user.EmailAddress);
            return model != null;
        }

        private bool IsUserNameExists(UserVm user)
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

        public PayloadResponse ApproveUser(string id)
        {
            var model = _userRepo.GetConditional(u => u.Id == id);

            if (model is null)
            {
                return new PayloadResponse()
                {
                    IsSuccess = false,
                    Message = "User not found!"
                };
            }

            model.IsApproved = true;

            _userRepo.Update(model);
            _userRepo.SaveChanges();

            return new PayloadResponse()
            {
                IsSuccess = true,
                Message = "User has been approved successfully!"
            };
        }

        public List<UserCreationVm> GetUnapprovedUser()
        {
            var data = _userRepo
                .GetAll()
                .Where(u => u.IsApproved == false && u.UserType == UserTypes.Volunteer)
                .OrderByDescending(u => u.CreateTime)
                .ToList();
            
            return GetMappedData(data);
        }

        public List<UserCreationVm> GetAllApprovedVolunteer(int pageNo, int pageSize)
        {
            var userData = _userRepo
                .GetAll()
                .Where(u => u.UserType == UserTypes.Volunteer)
                .OrderByDescending(u => u.LastModifiedTime)
                .Skip((pageNo-1)*pageSize)
                .Take(pageSize)
                .ToList();

            return GetMappedData(userData);
        }

        public PayloadResponse DisapproveUser(string id)
        {
            var model = _userRepo.GetConditional(u => u.Id == id);

            if (model is null)
            {
                return new PayloadResponse()
                {
                    IsSuccess = false,
                    Message = "User not found!"
                };
            }

            model.IsApproved = false;

            _userRepo.Update(model);
            _userRepo.SaveChanges();

            return new PayloadResponse()
            {
                IsSuccess = true,
                Message = "User has been disapproved successfully!"
            };
        }

        private List<UserCreationVm> GetMappedData(List<User> userData)
        {
            var mappedUserData = userData.Select(user => new UserCreationVm()
            {
                Id = user.Id,
                FullName = user.FullName,
                BloodGroup = user.BloodGroup,
                DateOfBirth = user.DateOfBirth,
                MobileNumber = user.MobileNumber,
                District = user.District,
                Upazila = user.Upazila,
                Union = user.Union,
                Address = user.Address,
                FatherName = user.FatherName,
                MotherName = user.MotherName,
                BloodDonationStatus = user.BloodDonationStatus,
                Gender = user.Gender,
                UserType = user.UserType,
                LastDonationTime = user.LastDonationTime,
                ImageUrl = user.ImageUrl,
                IsSuperAdmin = user.IsSuperAdmin
            }).ToList();

            return mappedUserData;
        }
    }
}