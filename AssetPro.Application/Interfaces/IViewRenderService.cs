using System.Threading.Tasks;

namespace AssetPro.Application.Interfaces
{
    public interface IViewRenderService
    {
        Task<string> RenderToStringAsync(string viewName, object model);
    }
}