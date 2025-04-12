using System.Collections.Generic;

namespace BloodDonation.Application.ViewModels
{
    public class MediaDataVm
    {
        public List<MediaUrlWithCampaignDataDto> ImageData { get; set; }
        public List<MediaUrlWithCampaignDataDto> VideoData { get; set; } 
    }
}
