using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using BloodDonation.Application.Interfaces;
using BloodDonation.Application.ViewModels;
using BloodDonation.Domain.Entities;
using BloodDonation.Domain.Interfaces;

namespace BloodDonation.Application.Services;

public class NewsService : INewsService
{
    private readonly IRepository<News> _newsRepository;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _apiKey = "34247144435bdc18f2b3e0e4fe791364";

    public NewsService(IRepository<News> newsRepository, 
        IHttpClientFactory httpClientFactory)
    {
        _newsRepository = newsRepository;
        _httpClientFactory = httpClientFactory;
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
                ThumbnailUrl = GetThumbnailUrl(newsData.Url).Result
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

    private async Task<string> GetThumbnailUrl(string newsUrl)
    {
        if(string.IsNullOrEmpty(newsUrl)) return string.Empty;

        var client = _httpClientFactory.CreateClient();
        var requestUrl = $"https://api.linkpreview.net?q={Uri.EscapeDataString(newsUrl)}";
        var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
        request.Headers.Add("X-Linkpreview-Api-Key", _apiKey);
        
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(json);

        if (jsonDoc.RootElement.TryGetProperty("image", out var imageProperty))
        {
            return imageProperty.GetString();
        }

        return string.Empty;
    }

    public PayloadResponse Update(NewsVm newsData)
    {
        try
        {
            var news = _newsRepository.GetConditional(n => n.Id == newsData.Id);

            news.Name = newsData.Name;
            news.Description = newsData.Description;
            news.Url = newsData.Url;
            news.ThumbnailUrl = GetThumbnailUrl(newsData.Url).Result;

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