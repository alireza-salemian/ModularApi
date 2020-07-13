using System.Collections.Generic;

namespace DoctorCode.AspNetCore.Modules.Abstrctions
{
    /// <summary>
    /// 
    /// </summary>
    public interface IModulesProvider
    {
        /// <summary>
        /// 
        /// </summary>
        IReadOnlyCollection<ModuleInfo> Modules { get; }
    }
}
