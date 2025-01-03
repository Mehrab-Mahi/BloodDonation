using BloodDonation.Application.Interfaces;
using BloodDonation.Application.Services;
using BloodDonation.Domain.Interfaces;
using BloodDonation.Infra.Data.Context;
using BloodDonation.Infra.Data.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BloodDonation.Infra.IoC
{
    public class DependencyContainer
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IConnectionStringProvider, ConnectionStringProvider>();
            services.AddScoped<IBaseRepository, BaseRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IDesignationService, DesignationService>();
            services.AddScoped<IAssetTypeService, AssetTypeService>();
            services.AddScoped<IAssetStatusService, AssetStatusService>();
            services.AddScoped<IMaintenanceTypeService, MaintenanceTypeService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ICommonService, CommonService>();
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<IViewRenderService, ViewRenderService>();
            services.AddScoped<IInvitationService, InvitationService>();
            services.AddScoped<ICampaignService, CampaignService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<ILoggedInUserService, LoggedInUserService>();
            services.AddScoped<ILocationService, LocationService>();
        }
    }
}