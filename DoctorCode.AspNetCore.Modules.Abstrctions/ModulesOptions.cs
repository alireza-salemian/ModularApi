using Microsoft.AspNetCore.Http;

namespace DoctorCode.AspNetCore.Modules.Abstrctions
{
    /// <summary>
    /// 
    /// </summary>
    public class ModulesOptions
    {
        /// <summary>
        /// 
        /// </summary>
        public const string DefaultModuleFileExtenstion = ".mvg";

        /// <summary>
        /// 
        /// </summary>
        public string ModuleFileExtenstion { get; set; } = DefaultModuleFileExtenstion;

        /// <summary>
        /// 
        /// </summary>
        public PathString ModulesPath { get; set; } = new PathString("/Modules");

        /// <summary>
        /// 
        /// </summary>
        public PathString CachePath { get; set; } = new PathString("/Cache");

        /// <summary>
        /// 
        /// </summary>
        public bool SuppressModuleLoadingException { get; set; } = true;
    }
}
