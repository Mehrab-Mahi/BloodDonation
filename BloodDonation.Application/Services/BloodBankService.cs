using System;
using System.Collections.Generic;
using System.Linq;
using BloodDonation.Application.Interfaces;
using BloodDonation.Application.Util;
using BloodDonation.Application.ViewModels;
using BloodDonation.Domain.Entities;
using BloodDonation.Domain.Interfaces;

namespace BloodDonation.Application.Services
{
    public class BloodBankService : IBloodBankService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Campaign> _campaignRepository;

        public BloodBankService(IRepository<User> userRepository, 
            IRepository<Campaign> campaignRepository)
        {
            _userRepository = userRepository;
            _campaignRepository = campaignRepository;
        }

        public List<BloodBankDonorDataVm> GetBloodBankData(BloodBankFilter filter)
        {
            var user = _userRepository
                .GetAll()
                .Where(u => u.BloodDonationStatus == "Interested");

            if (!string.IsNullOrEmpty(filter.BloodGroup))
            {
                user.Where(u => u.BloodGroup == filter.BloodGroup);
            }

            if (!string.IsNullOrEmpty(filter.Upazila))
            {
                user.Where(u => u.Upazila == filter.Upazila);
            }

            if (!string.IsNullOrEmpty(filter.Union))
            {
                user.Where(u => u.Union == filter.Union);
            }

            var startDob = GetDateDifference(filter.StartAge);
            var endDob = GetDateDifference(filter.EndAge);
            var minimumLastDonationDate = GetMinimumLastDonationDate();

            user.Where(u => u.Dob <= startDob && u.Dob >= endDob && u.LastDonationTime <= minimumLastDonationDate);

            return user
                .Skip((filter.PageNo - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .OrderBy(u => u.LastDonationTime)
                .Select(u => new BloodBankDonorDataVm()
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    BloodGroup = u.BloodGroup,
                    MobileNumber = u.MobileNumber,
                    Address = u.Address,
                    BloodDonationCount = u.BloodDonationCount,
                    LastDonationTime = u.LastDonationTime,
                    LastDonationDayCount = u.LastDonationTime == null ? 0 : (DateTime.Now - u.LastDonationTime.Value).Days
                })
                .ToList();
        }

        public DashboardDataVm GetDashboardData()
        {
            var volunteers = _userRepository
                .GetAll()
                .Where(u => u.UserType == UserTypes.Volunteer)
                .Select(u => u.Id)
                .ToList();

            var dashboardData = new DashboardDataVm()
            {
                Volunteer = volunteers.Count,
                Donor = _userRepository.GetAll().Count(u => u.UserType == UserTypes.Donor && volunteers.Contains(u.CreatedBy)),
                RegisteredDonor = _userRepository.GetAll().Count(u => u.UserType == UserTypes.Donor && !volunteers.Contains(u.CreatedBy)),
                Campaign = _campaignRepository.GetAll().Count(c => c.StartDate <= DateTime.Now && c.EndDate >= DateTime.Now)
            };

            return dashboardData;
        }

        private static DateTime GetMinimumLastDonationDate()
        {
            return DateTime.Now.AddMonths(-4);
        }

        private static DateTime GetDateDifference(int ageToReduce)
        {
            var date = DateTime.Now.AddYears((0-ageToReduce));

            return date;
        }
    }
}
