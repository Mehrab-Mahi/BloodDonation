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
        object GetAll(int pageNo, int pageSize);
    }
}