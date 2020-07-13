using Microsoft.Extensions.DependencyInjection;

namespace DoctorCode.AspNetCore.Modules.Abstrctions
{
    /// <summary>
    /// 
    /// </summary>
    public interface IModuleConfigureServices
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        void ConfigureServices(IServiceCollection services);
    }
}
