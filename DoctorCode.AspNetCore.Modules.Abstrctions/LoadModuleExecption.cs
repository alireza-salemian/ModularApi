using System;

namespace DoctorCode.AspNetCore.Modules.Abstrctions
{
    /// <summary>
    /// 
    /// </summary>
    public class LoadModuleExecption : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="modulePath"></param>
        public LoadModuleExecption(string modulePath) : base($"Load module in path `{modulePath}` failed.")
        {

        }
    }
}
