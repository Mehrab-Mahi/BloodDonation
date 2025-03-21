using System;
using System.Linq;
using BloodDonation.Application.Interfaces;
using BloodDonation.Application.ViewModels;
using BloodDonation.Domain.Entities;
using BloodDonation.Domain.Interfaces;

namespace BloodDonation.Application.Services;

public class NewsService : INewsService
{
    private readonly IRepository<News> _newsRepository;

    public NewsService(IRepository<News> newsRepository)
    {
        _newsRepository = newsRepository;
    }

    public PayloadResponse Create(NewsVm newsData)
    {
        try
        {
            var news = new News()
            {
                Name = newsData.Name,
                Description = newsData.Description,
                Url = newsData.Url,
            };

            _newsRepository.Insert(news);
            _newsRepository.SaveChanges();

            return new PayloadResponse()
            {
                IsSuccess = true,
                PayloadType = "News",
                Content = null,
                Message = "News created successfully"
            };
        }
        catch (Exception ex)
        {
            return new PayloadResponse()
            {
                IsSuccess = false,
                PayloadType = "News",
                Content = null,
                Message = ex.Message
            };
        }
    }

    public PayloadResponse Update(NewsVm newsData)
    {
        try
        {
            var news = _newsRepository.GetConditional(n => n.Id == newsData.Id);

            news.Name = newsData.Name;
            news.Description = newsData.Description;
            news.Url = newsData.Url;

            _newsRepository.Update(news);
            _newsRepository.SaveChanges();

            return new PayloadResponse()
            {
                IsSuccess = true,
                PayloadType = "News",
                Content = null,
                Message = "News update successfully"
            };
        }
        catch (Exception ex)
        {
            return new PayloadResponse()
            {
                IsSuccess = false,
                PayloadType = "News",
                Content = null,
                Message = $"Notice update failed because {ex.Message}"
            };
        }
    }

    public PayloadResponse Delete(string id)
    {
        try
        {
            var news = _newsRepository.GetConditional(n => n.Id == id);

            if (news == null)
            {
                return new PayloadResponse
                {
                    IsSuccess = false,
                    PayloadType = "News",
                    Content = null,
                    Message = "News not found"
                };
            }

            _newsRepository.Delete(news);
            _newsRepository.SaveChanges();

            return new PayloadResponse()
            {
                IsSuccess = true,
                PayloadType = "News",
                Content = null,
                Message = "News deleted successfully"
            };
        }
        catch (Exception ex)
        {
            return new PayloadResponse()
            {
                IsSuccess = false,
                PayloadType = "News",
                Content = null,
                Message = $"News deletion failed because {ex.Message}"
            };
        }
    }

    public News Get(string id)
    {
        return _newsRepository.GetConditional(n => n.Id == id);
    }

    public object GetAll(int pageNo, int pageSize)
    {
        var allNews = _newsRepository.GetAll();

        var totalRowCount = allNews.Count();

        var newsList = allNews
            .OrderByDescending(n => n.LastModifiedTime)
            .Skip((pageNo - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new
        {
            data = newsList,
            rowCount = totalRowCount
        };
    }
}