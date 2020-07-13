using Microsoft.AspNetCore.Builder;

namespace DoctorCode.AspNetCore.Modules.Abstrctions
{
    /// <summary>
    /// 
    /// </summary>
    public interface IModuleConfigure
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        void Configure(IApplicationBuilder app);
    }
}
