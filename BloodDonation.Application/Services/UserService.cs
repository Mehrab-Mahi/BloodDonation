using BloodDonation.Application.Interfaces;
using BloodDonation.Application.ViewModels;
using BloodDonation.Domain.Entities;
using BloodDonation.Domain.Interfaces;
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
        private readonly IRepository<Location> _locationRepository;
        private readonly IFileService _fileService;
        public UserService(IRepository<User> userRepo,
            IRepository<Role> roleRepo,
            IFileService fileService, 
            IRepository<Location> locationRepository)
        {
            _userRepo = userRepo;
            _roleRepo = roleRepo;
            _fileService = fileService;
            _locationRepository = locationRepository;
        }

        public User Get(AuthRequest model)
        {
            var user = _userRepo.GetConditional(u => u.MobileNumber == model.MobileNumber && u.DateOfBirth == model.DateOfBirth); 

            return user;
        }

        public object GetAll(UserFilter filter)
        {
            var allUser = _userRepo
                .GetAll().Where(u => !u.IsSuperAdmin);

            if (!string.IsNullOrEmpty(filter.BloodGroup))
            {
                allUser = FilterByBloodGroup(allUser, filter.BloodGroup);
            }

            if (!string.IsNullOrEmpty(filter.Upazila))
            {
                allUser = FilterByUpazila(allUser, filter.Upazila);
            }

            if (!string.IsNullOrEmpty(filter.Union))
            {
                allUser = FilterByUnion(allUser, filter.Union);
            }
            
            if (!string.IsNullOrEmpty(filter.BloodDonationStatus))
            {
                allUser = FilterByBloodDonationStatus(allUser, filter.BloodDonationStatus);
            }

            if (!string.IsNullOrEmpty(filter.UserType))
            {
                allUser = FilterByUserType(allUser, filter.UserType);
            }

            if (filter.StartAge is null || filter.EndAge is null)
            {
                filter.StartAge = 0;
                filter.EndAge = 100;
            }

            var startDob = GetDateDifference(filter.StartAge.Value);
            var endDob = GetDateDifference(filter.EndAge.Value);

            allUser = FilterByDate(allUser, startDob, endDob);

            var totalRowCount = allUser.Count();

            if (filter.PageNo is null || filter.PageSize is null)
            {
                filter.PageNo = 0;
                filter.PageSize = 10;
            }

            allUser = allUser
                .OrderByDescending(u => u.CreateTime)
                .Skip((filter.PageNo.Value - 1) * filter.PageSize.Value)
                .Take(filter.PageSize.Value);

            var userData = (from user in allUser
                join district in _locationRepository.GetAll() on user.District equals district.Id
                join upazila in _locationRepository.GetAll() on user.Upazila equals upazila.Id
                join union in _locationRepository.GetAll() on user.Union equals union.Id
                select new UserCreationVm()
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    BloodGroup = user.BloodGroup,
                    DateOfBirth = user.DateOfBirth,
                    MobileNumber = user.MobileNumber,
                    District = user.District,
                    DistrictName = district.Name,
                    Upazila = user.Upazila,
                    UpazilaName = upazila.Name,
                    Union = user.Union,
                    UnionName = union.Name,
                    Address = user.Address,
                    FatherName = user.FatherName,
                    MotherName = user.MotherName,
                    BloodDonationStatus = user.BloodDonationStatus,
                    Gender = user.Gender,
                    UserType = user.UserType,
                    LastDonationTime = user.LastDonationTime,
                    ImageUrl = user.ImageUrl,
                    BloodDonationCount = user.BloodDonationCount,
                    IsApproved = user.IsApproved
                })
                .ToList();

            return new
            {
                data = userData,
                rowCount = totalRowCount
            };
        }

        private IQueryable<User> FilterByBloodDonationStatus(IQueryable<User> allUser, string bloodDonationStatus)
        {
            return allUser.Where(u => u.BloodDonationStatus == bloodDonationStatus);
        }

        private static IQueryable<User> FilterByUserType(IQueryable<User> allUser, string userType)
        {
            return allUser.Where(u => u.UserType == userType);
        }

        private static DateTime GetDateDifference(int ageToReduce)
        {
            var date = DateTime.Now.AddYears((0 - ageToReduce));

            return date;
        }

        private IQueryable<User> FilterByDate(IQueryable<User> user, DateTime startDob, DateTime endDob)
        {
            return user.Where(u => u.Dob <= startDob && u.Dob >= endDob);
        }

        private IQueryable<User> FilterByUnion(IQueryable<User> user, string union)
        {
            return user.Where(u => u.Union == union);
        }

        private IQueryable<User> FilterByUpazila(IQueryable<User> user, string upazila)
        {
            return user.Where(u => u.Upazila == upazila);
        }

        private IQueryable<User> FilterByBloodGroup(IQueryable<User> user, string bloodGroup)
        {
            return user.Where(u => u.BloodGroup == bloodGroup);
        }

        private string GetRoleName(string roleId)
        {
            return _roleRepo.Find(roleId).Name;
        }

        public User GetById(string id)
        {
            return _userRepo.GetConditional(u => u.Id == id);
        }

        public PayloadResponse Insert(UserCreationVm user)
        {
            if (IfDuplicateUser(user.MobileNumber, user.DateOfBirth))
            {
                return new PayloadResponse
                {
                    IsSuccess = false,
                    PayloadType = "User Creation",
                    Content = null,
                    Message = $"User with this mobile number and date of birth is already exist!"
                };
            }

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
                    BloodDonationCount = user.BloodDonationCount,
                    Dob = DateTime.Parse(user.DateOfBirth)
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
            catch (Exception ex)
            {
                return new PayloadResponse
                {
                    IsSuccess = false,
                    PayloadType = "User Creation",
                    Content = null,
                    Message = $"User Creation become unsuccessful because {ex.Message}"
                };
            }
        }

        private bool IfDuplicateUser(string mobileNumber, string dateOfBirth)
        {
            var user = _userRepo.GetAll().FirstOrDefault(u => u.MobileNumber == mobileNumber && u.DateOfBirth == dateOfBirth);

            return user is not null;
        }

        private string UploadAndGetImageUrl(IFormFile userProfilePicture)
        {
            if (userProfilePicture is null) return string.Empty;

            var fileName = GetFileName(userProfilePicture.FileName);
            var path = Path.Combine(_fileService.GetRootPath(), "ProfilePicture");
            _fileService.CreateDirectoryIfNotExists(path);
            var filePath = Path.Combine(path, fileName);
            _fileService.SaveFile(filePath, userProfilePicture);
            return Path.Combine("ProfilePicture", fileName);
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

        public PayloadResponse Update(UserCreationVm user)
        {
            var model = _userRepo.GetConditional(u => u.Id == user.Id);
            try
            {
                if (user.MobileNumber != model.MobileNumber || user.DateOfBirth != model.DateOfBirth)
                {
                    if (IfDuplicateUser(user.MobileNumber, user.DateOfBirth))
                    {
                        return new PayloadResponse
                        {
                            IsSuccess = false,
                            PayloadType = "User Update",
                            Content = null,
                            Message = "User with the mobile number and date of birth already exists!"
                        };
                    }
                }

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
                model.Dob = DateTime.Parse(user.DateOfBirth);

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

        public object GetUnapprovedUser(int pageNo, int pageSize)
        {
            var unapprovedData = _userRepo
                .GetAll()
                .Where(u => u.IsApproved == false && u.UserType == UserTypes.Volunteer)
                .OrderByDescending(u => u.CreateTime);

            var totalRowCount = unapprovedData
                .Count();
            
            var mappedData = GetMappedData(unapprovedData.Skip((pageNo-1)*pageSize).Take(pageSize).ToList());

            return new
            {
                data = mappedData,
                rowCount = totalRowCount
            };
        }

        public object GetApprovedVolunteer(int pageNo, int pageSize)
        {
            var userData = _userRepo
                .GetAll()
                .Where(u => u.UserType == UserTypes.Volunteer && u.IsApproved == true)
                .OrderByDescending(u => u.LastModifiedTime);

            var totalRowCount = userData.Count();

            var mappedData = GetMappedData(userData
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
                .ToList());

            return new
            {
                data = mappedData,
                rowCount = totalRowCount
            };
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

        public PayloadResponse DeleteUser(string id)
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

            _userRepo.Delete(model);
            _userRepo.SaveChanges();

            return new PayloadResponse()
            {
                IsSuccess = true,
                Message = "User has been deleted successfully!"
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
                IsSuperAdmin = user.IsSuperAdmin,
                IsApproved = user.IsApproved
            }).ToList();

            return mappedUserData;
        }
    }
}