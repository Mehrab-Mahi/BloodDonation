using AssetPro.Application.Interfaces;
using AssetPro.Application.Services;
using AssetPro.Domain.Interfaces;
using AssetPro.Infra.Data.Context;
using AssetPro.Infra.Data.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AssetPro.Infra.IoC
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
        }
    }
}