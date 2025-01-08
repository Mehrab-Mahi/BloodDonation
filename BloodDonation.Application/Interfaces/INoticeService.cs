using System.Collections.Generic;
using BloodDonation.Application.ViewModels;

namespace BloodDonation.Application.Interfaces
{
    public interface INoticeService
    {
        PayloadResponse Create(NoticeVm noticeData);
        PayloadResponse Update(NoticeVm noticeData);
        PayloadResponse Delete(string id);
        NoticeVm Get(string id);
        List<NoticeVm> GetAll(int pageNo, int pageSize);
    }
}