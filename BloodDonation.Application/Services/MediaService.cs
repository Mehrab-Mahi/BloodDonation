using System;
using System.Collections.Generic;
using System.Linq;
using BloodDonation.Application.Interfaces;
using BloodDonation.Application.ViewModels;
using BloodDonation.Domain.Entities;
using BloodDonation.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BloodDonation.Application.Services
{
    public class MediaService : IMediaService
    {
        private readonly IRepository<FileModelMapping> _fileModelRepository;
        private readonly IFileService _fileService;

        public MediaService(IRepository<FileModelMapping> fileModelRepository,
            IFileService fileService)
        {
            _fileModelRepository = fileModelRepository;
            _fileService = fileService;
        }

        public PayloadResponse UploadCampaignMedia(MediaVm mediaData)
        {
            try
            {
                if (mediaData.Images is not null)
                {
                    UploadImages(mediaData.ModelId, mediaData.Images, "Campaign");
                }

                if (mediaData.VideoUrls is not null)
                {
                    UploadVideo(mediaData.ModelId, mediaData.VideoUrls, "Campaign");
                }
                
                _fileModelRepository.SaveChanges();

                return new PayloadResponse()
                {
                    IsSuccess = true,
                    PayloadType = "Media",
                    Content = null,
                    Message = "Media uploaded successfully"
                };
            }
            catch (Exception ex)
            {
                return new PayloadResponse()
                {
                    IsSuccess = false,
                    PayloadType = "Media",
                    Content = null,
                    Message = $"Media upload failed because {ex.Message}"
                };
            }
        }

        public MediaDataVm GetAllMedia(MediaDataSizeVm mediaDataSize)
        {
            var imageUrls = new List<string>();
            var videoUrls = new List<string>();

            if (mediaDataSize.ImagePageNo > 0)
            {
                imageUrls = _fileModelRepository
                    .GetAll()
                    .Where(u => u.ModelName == "Campaign" && u.Type == "Image")
                    .OrderByDescending(c => c.LastModifiedTime)
                    .Skip((mediaDataSize.ImagePageNo - 1) * mediaDataSize.ImagePageSize)
                    .Take(mediaDataSize.ImagePageSize)
                    .Select(u => u.FileUrl)
                    .ToList();
            }

            if (mediaDataSize.VideoPageNo > 0)
            {
                videoUrls = _fileModelRepository
                    .GetAll()
                    .Where(u => u.ModelName == "Campaign" && u.Type == "Video")
                    .OrderByDescending(c => c.LastModifiedTime)
                    .Skip((mediaDataSize.VideoPageNo - 1) * mediaDataSize.VideoPageSize)
                    .Take(mediaDataSize.VideoPageSize)
                    .Select(u => u.FileUrl)
                    .ToList();
            }

            return new MediaDataVm()
            {
                ImageUrls = imageUrls,
                VideoUrls = videoUrls
            };
        }

        public PayloadResponse DeleteCampaignMedia(MediaDeleteVm mediaDeleteData)
        {
            try
            {
                var fileModel = _fileModelRepository.GetConditional(f =>
                    f.ModelId == mediaDeleteData.Id && f.FileUrl == mediaDeleteData.FileUrl);

                if (fileModel.Type == "Image")
                {
                    _fileService.DeleteFile(fileModel.FileUrl);
                }

                _fileModelRepository.Delete(fileModel);
                _fileModelRepository.SaveChanges();

                return new PayloadResponse()
                {
                    IsSuccess = true,
                    PayloadType = "Media",
                    Content = null,
                    Message = "Media deleted successfully"
                };
            }
            catch (Exception ex)
            {
                return new PayloadResponse()
                {
                    IsSuccess = false,
                    PayloadType = "Media",
                    Content = null,
                    Message = $"Media deletion failed because {ex.Message}"
                };
            }
        }

        public MediaDataVm GetCampaignMedia(string campaignId)
        {
            var imageUrls = _fileModelRepository
                .GetConditionalList(f => f.ModelId == campaignId && f.Type == "Image")
                .Select(i => i.FileUrl)
                .ToList();

            var videoUrls = _fileModelRepository
                .GetConditionalList(f => f.ModelId == campaignId && f.Type == "Video")
                .Select(i => i.FileUrl)
                .ToList();

            return new MediaDataVm()
            {
                ImageUrls = imageUrls,
                VideoUrls = videoUrls
            };
        }

        private void UploadVideo(string modelId, List<string> videoUrls, string modelName)
        {
            foreach (var url in videoUrls)
            {
                _fileModelRepository.Insert(new FileModelMapping()
                {
                    ModelName = modelName,
                    ModelId = modelId,
                    Type = "Video",
                    FileUrl = url
                });
            }
        }

        private void UploadImages(string modelId, List<IFormFile> mediaImages, string modelName)
        {
            foreach (var image in mediaImages)
            {
                var filePath = _fileService.UploadFile(image, "Media");
                _fileModelRepository.Insert(new FileModelMapping()
                {
                    ModelName = modelName,
                    ModelId = modelId,
                    Type = "Image",
                    FileUrl = filePath
                });
            }
        }
    }
}
