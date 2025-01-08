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
    public class NoticeService : INoticeService
    {
        private readonly IRepository<Notice> _noticeRepository;
        private readonly IRepository<FileModelMapping> _fileModelRepository;
        private readonly IFileService _fileService;

        public NoticeService(IRepository<Notice> noticeRepository,
            IRepository<FileModelMapping> fileModelRepository,
            IFileService fileService)
        {
            _noticeRepository = noticeRepository;
            _fileModelRepository = fileModelRepository;
            _fileService = fileService;
        }

        public PayloadResponse Create(NoticeVm noticeData)
        {
            try
            {
                var notice = new Notice()
                {
                    Name = noticeData.Name,
                    Description = noticeData.Description
                };

                _noticeRepository.Insert(notice);
                _noticeRepository.SaveChanges();

                UploadNoticeFiles(notice.Id, noticeData.Files);

                return new PayloadResponse()
                {
                    IsSuccess = true,
                    PayloadType = "Notice",
                    Content = null,
                    Message = "Notice created successfully"
                };
            }
            catch (Exception ex)
            {
                return new PayloadResponse()
                {
                    IsSuccess = false,
                    PayloadType = "Notice",
                    Content = null,
                    Message = $"Notice creation failed because {ex.Message}"
                };
            }
        }

        public PayloadResponse Update(NoticeVm noticeData)
        {
            try
            {
                var notice = _noticeRepository.GetConditional(n => n.Id == noticeData.Id);

                notice.Name = noticeData.Name;
                notice.Description = noticeData.Description;

                UpdateNoticeFiles(notice.Id, noticeData.FileUrls, noticeData.Files);

                return new PayloadResponse()
                {
                    IsSuccess = true,
                    PayloadType = "Notice",
                    Content = null,
                    Message = "Notice update successfully"
                };
            }
            catch (Exception ex)
            {
                return new PayloadResponse()
                {
                    IsSuccess = false,
                    PayloadType = "Notice",
                    Content = null,
                    Message = $"Notice update failed because {ex.Message}"
                };
            }
        }

        public PayloadResponse Delete(string id)
        {
            try
            {
                var notice = _noticeRepository.GetConditional(n => n.Id == id);

                if (notice == null)
                {
                    return new PayloadResponse
                    {
                        IsSuccess = false,
                        PayloadType = "Notice",
                        Content = null,
                        Message = "Notice not found"
                    };
                }

                _noticeRepository.Delete(notice);
                _noticeRepository.SaveChanges();

                var noticeFiles = _fileModelRepository
                    .GetAll()
                    .Where(f => f.ModelId == id && f.ModelName == "Notice")
                    .ToList();

                _fileModelRepository.Delete(noticeFiles);
                _fileModelRepository.SaveChanges();

                return new PayloadResponse()
                {
                    IsSuccess = true,
                    PayloadType = "Notice",
                    Content = null,
                    Message = "Notice deleted successfully"
                };
            }
            catch (Exception ex)
            {
                return new PayloadResponse()
                {
                    IsSuccess = false,
                    PayloadType = "Notice",
                    Content = null,
                    Message = $"Notice deletion failed because {ex.Message}"
                };
            }
        }

        public NoticeVm Get(string id)
        {
            var notice = _noticeRepository.GetConditional(n => n.Id == id);

            var fileUrls = _fileModelRepository
                .GetAll()
                .Where(m => m.ModelId == id && m.ModelName == "Notice")
                .Select(f => f.FileUrl)
                .ToList();

            return new NoticeVm()
            {
                Id = id,
                Name = notice.Name,
                Description = notice.Description,
                FileUrls = fileUrls
            };
        }

        public List<NoticeVm> GetAll(int pageNo, int pageSize)
        {
            var notices = _noticeRepository
                .GetAll()
                .OrderByDescending(n => n.LastModifiedTime)
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var noticeList = new List<NoticeVm>();

            var files = _fileModelRepository
                .GetAll()
                .Where(u => u.ModelName == "Notice");

            foreach (var notice in notices)
            {
                var fileList = files
                    .Where(f => f.ModelId == notice.Id)
                    .Select(u => u.FileUrl)
                    .ToList();

                var noticeVm = new NoticeVm()
                {
                    Id = notice.Id,
                    Name = notice.Name,
                    Description = notice.Description,
                    FileUrls = fileList
                };

                noticeList.Add(noticeVm);
            }

            return noticeList;
        }

        private void UpdateNoticeFiles(string noticeId, List<string> fileUrls, List<IFormFile> noticeFiles)
        {
            var previousFileUrls = _fileModelRepository
                .GetAll()
                .Where(n => n.ModelId == noticeId && n.ModelName == "Notice")
                .ToList();

            var filesToRemove = fileUrls.Except(previousFileUrls.Select(u => u.FileUrl));

            var fileModels = previousFileUrls.Where(f => filesToRemove.Contains(f.FileUrl));

            foreach (var file in fileModels)
            {
                _fileService.DeleteFile(file.FileUrl);
                _fileModelRepository.Delete(file);
            }

            _fileModelRepository.SaveChanges();

            UploadNoticeFiles(noticeId, noticeFiles);
        }

        private void UploadNoticeFiles(string noticeId, List<IFormFile> noticeFiles)
        {
            foreach (var file in noticeFiles)
            {
                var filePath = _fileService.UploadFile(file, "Notice");

                _fileModelRepository.Insert(new FileModelMapping()
                {
                    ModelName = "Notice",
                    FileUrl = filePath,
                    ModelId = noticeId,
                    Type = "Image"
                });
            }

            _fileModelRepository.SaveChanges();
        }
    }
}
