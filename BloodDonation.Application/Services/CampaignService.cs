using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BloodDonation.Application.Interfaces;
using BloodDonation.Application.Util;
using BloodDonation.Application.ViewModels;
using BloodDonation.Domain.Entities;
using BloodDonation.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BloodDonation.Application.Services
{
    public class CampaignService : ICampaignService
    {
        private readonly IRepository<Campaign> _campaignRepository;
        private readonly IRepository<CampaignVolunteerMapping> _campaignVolunteerMappingRepository;
        private readonly IFileService _fileService;
        private readonly ILoggedInUserService _loggedInUserService;

        public CampaignService(IRepository<Campaign> campaignRepository,
            IRepository<CampaignVolunteerMapping> campaignVolunteerMappingRepository,
            IFileService fileService,
            ILoggedInUserService loggedInUserService)
        {
            _campaignRepository = campaignRepository;
            _campaignVolunteerMappingRepository = campaignVolunteerMappingRepository;
            _fileService = fileService;
            _loggedInUserService = loggedInUserService;
        }

        public PayloadResponse Create(CampaignVm campaignData)
        {
            try
            {
                var bannerUrl = UploadAndGetBannerUrl(campaignData.Banner);
                var campaign = new Campaign()
                {
                    Name = campaignData.Name,
                    StartDate = campaignData.StartDate,
                    EndDate = campaignData.EndDate,
                    Address = campaignData.Address,
                    BannerUrl = bannerUrl
                };

                _campaignRepository.Insert(campaign);
                _campaignRepository.SaveChanges();

                AssignVolunteerToCampaign(campaign.Id, campaignData.VolunteerList);

                return new PayloadResponse
                {
                    IsSuccess = true,
                    PayloadType = "Campaign created",
                    Content = null,
                    Message = "Campaign created successfully"
                };
            }
            catch
            {
                return new PayloadResponse
                {
                    IsSuccess = false,
                    PayloadType = "Campaign",
                    Content = null,
                    Message = "Campaign creation failed"
                };
            }
        }

        public PayloadResponse Update(string id, CampaignVm campaignData)
        {
            try
            {
                var previousData = _campaignRepository.GetConditional(c => c.Id == id);
                var newBannerUrl = string.Empty;

                if (previousData == null)
                {
                    return new PayloadResponse()
                    {
                        IsSuccess = false,
                        PayloadType = "Campaign",
                        Content = null,
                        Message = "Data not found"
                    };
                }

                if (campaignData.Banner is { Length: > 0 })
                {
                    _fileService.DeleteFile(previousData.BannerUrl);
                    newBannerUrl = UploadAndGetBannerUrl(campaignData.Banner);
                }

                previousData.Name = campaignData.Name;
                previousData.StartDate = campaignData.StartDate;
                previousData.EndDate = campaignData.EndDate;
                previousData.Address = campaignData.Address;
                previousData.BannerUrl = !string.IsNullOrEmpty(newBannerUrl) ? newBannerUrl : previousData.BannerUrl;

                _campaignRepository.Update(previousData);
                _campaignRepository.SaveChanges();

                UpdateVolunteerToCampaign(id, campaignData.VolunteerList);

                return new PayloadResponse()
                {
                    IsSuccess = true,
                    PayloadType = "Campaign updated",
                    Content = null,
                    Message = "Campaign updated successfully"
                };
            }
            catch (Exception ex)
            {
                return new PayloadResponse
                {
                    IsSuccess = false,
                    PayloadType = "Campaign",
                    Content = null,
                    Message = $"Campaign update failed because {ex.Message}"
                };
            }
        }

        public PayloadResponse Delete(string id)
        {
            var campaign = _campaignRepository.GetConditional(c => c.Id == id);

            if (campaign == null)
            {
                return new PayloadResponse
                {
                    IsSuccess = false,
                    PayloadType = "Campaign",
                    Content = null,
                    Message = "Campaign not found"
                };
            }

            try
            {
                _campaignRepository.Delete(campaign);
                _campaignRepository.SaveChanges();

                var campaignVolunteer = _campaignVolunteerMappingRepository
                    .GetConditionalList(c => c.CampaignId == id)
                    .ToList();

                _campaignVolunteerMappingRepository.Delete(campaignVolunteer);
                _campaignVolunteerMappingRepository.SaveChanges();

                return new PayloadResponse()
                {
                    IsSuccess = true,
                    PayloadType = "Campaign delete",
                    Content = null,
                    Message = "Campaign deleted successfully"
                };
            }
            catch (Exception ex)
            {
                return new PayloadResponse
                {
                    IsSuccess = false,
                    PayloadType = "Campaign",
                    Content = null,
                    Message = $"Campaign deletion failed because {ex.Message}"
                };
            }
        }
        
        public CampaignVm Get(string id)
        {
            var campaign = _campaignRepository.GetConditional(c => c.Id == id);

            if (campaign == null)
            {
                return new CampaignVm();
            }

            var volunteerList = _campaignVolunteerMappingRepository
                .GetConditionalList(c => c.CampaignId == id)
                .Select(c => c.VolunteerId)
                .ToList();

            return new CampaignVm()
            {
                Id = id,
                Name = campaign.Name,
                StartDate = campaign.StartDate,
                EndDate = campaign.EndDate,
                Address = campaign.Address,
                BannerUrl = campaign.BannerUrl,
                VolunteerList = volunteerList
            };
        }

        public List<Campaign> GetAll(int pageNo, int pageSize)
        {
            var currentUser = _loggedInUserService.GetLoggedInUser();

            if (currentUser is { UserType: UserTypes.Volunteer })
            {
                return VolunteerPermittedCampaign(currentUser.Id, pageNo, pageSize);
            }

            var campaignList = _campaignRepository
                .GetAll()
                .OrderByDescending(c => c.CreateTime)
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return campaignList;
        }

        private List<Campaign> VolunteerPermittedCampaign(string currentUserId, int pageNo, int pageSize)
        {
            var permittedCampaign = _campaignVolunteerMappingRepository
                .GetConditionalList(v => v.VolunteerId == currentUserId)
                .Select(c => c.CampaignId)
                .Distinct()
                .ToList();

            var campaignList = _campaignRepository
                .GetAll()
                .Where(c => permittedCampaign.Contains(c.Id))
                .OrderByDescending(c => c.CreateTime)
                .Skip((pageNo - 1)*pageSize)
                .Take(pageSize)
                .ToList();

            return campaignList;
        }

        private void UpdateVolunteerToCampaign(string id, List<string> volunteerList)
        {
            var previousVolunteer = _campaignVolunteerMappingRepository
                .GetConditionalList(c => c.CampaignId == id)
                .ToList();

            _campaignVolunteerMappingRepository.Delete(previousVolunteer);

            foreach (var volunteerId in volunteerList)
            {
                _campaignVolunteerMappingRepository.Insert(new CampaignVolunteerMapping()
                {
                    CampaignId = id,
                    VolunteerId = volunteerId
                });
            }

            _campaignVolunteerMappingRepository.SaveChanges();
        }

        private string UploadAndGetBannerUrl(IFormFile campaignBanner)
        {
            if(campaignBanner is null) return string.Empty;

            var fileName = GetFileName(campaignBanner.FileName);
            var path = Path.Combine(_fileService.GetRootPath(), @"\Campaign\");
            _fileService.CreateDirectoryIfNotExists(path);
            var filePath = Path.Combine(path, fileName);
            _fileService.SaveFile(filePath, campaignBanner);
            return filePath;
        }

        private static string GetFileName(string campaignBannerFileName)
        {
            return Guid.NewGuid().ToString("N") + "-" + campaignBannerFileName;
        }

        private void AssignVolunteerToCampaign(string campaignId, List<string> campaignDataVolunteerList)
        {
            foreach (var volunteerId in campaignDataVolunteerList)
            {
                _campaignVolunteerMappingRepository.Insert(new CampaignVolunteerMapping()
                {
                    CampaignId = campaignId,
                    VolunteerId = volunteerId
                });
            }

            _campaignVolunteerMappingRepository.SaveChanges();
        }
    }
}
