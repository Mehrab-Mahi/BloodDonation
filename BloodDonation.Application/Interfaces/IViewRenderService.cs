using System.Threading.Tasks;

namespace BloodDonation.Application.Interfaces
{
    public interface IViewRenderService
    {
        Task<string> RenderToStringAsync(string viewName, object model);
    }
}