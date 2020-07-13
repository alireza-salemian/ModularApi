using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DoctorCode.AspNetCore.Modules.Abstrctions
{
    /// <summary>
    /// 
    /// </summary>
    public class ModuleMvcBuilder : IMvcBuilder
    {
        private readonly IMvcBuilder _mvcBuilder;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mvcBuilder"></param>
        /// <param name="modulesProvider"></param>
        public ModuleMvcBuilder(IMvcBuilder mvcBuilder, IModulesProvider modulesProvider)
        {
            _mvcBuilder = mvcBuilder ?? throw new ArgumentNullException(nameof(mvcBuilder));
            ModulesProvider = modulesProvider ?? throw new ArgumentNullException(nameof(modulesProvider));
        }

        /// <inheritdoc />
        public ApplicationPartManager PartManager => _mvcBuilder.PartManager;

        /// <inheritdoc />
        public IServiceCollection Services => _mvcBuilder.Services;

        /// <summary>
        /// 
        /// </summary>
        public IModulesProvider ModulesProvider { get; }
    }
}
