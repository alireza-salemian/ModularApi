using Microsoft.Extensions.Logging;
using System;

namespace DoctorCode.AspNetCore.Modules.Abstrctions
{
    /// <summary>
    /// 
    /// </summary>
    public class ModulesProviderContext
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="loggingBuilder"></param>
        public ModulesProviderContext(ModulesOptions options, ILoggingBuilder loggingBuilder)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
            LoggingBuilder = loggingBuilder ?? throw new ArgumentNullException(nameof(loggingBuilder));
        }

        /// <summary>
        /// 
        /// </summary>
        public ModulesOptions Options { get; }

        /// <summary>
        /// 
        /// </summary>
        public ILoggingBuilder LoggingBuilder { get; }
    }
}
