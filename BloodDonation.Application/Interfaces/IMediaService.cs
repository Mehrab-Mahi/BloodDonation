using BloodDonation.Application.ViewModels;

namespace BloodDonation.Application.Interfaces
{
    public interface IMediaService
    {
        PayloadResponse UploadCampaignMedia(MediaVm mediaData);
        MediaDataVm GetCampaignMedia(MediaDataSizeVm mediaDataSize);
        PayloadResponse DeleteCampaignMedia(MediaDeleteVm mediaDeleteData);
    }
}