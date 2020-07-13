using DoctorCode.AspNetCore.Modules;
using DoctorCode.AspNetCore.Modules.Abstrctions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 
    /// </summary>
    public static class ModulesMvcBuilderExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mvcBuilder"></param>
        /// <returns></returns>
        public static ModuleMvcBuilder AddModules(this IMvcBuilder mvcBuilder)
            => AddModules(mvcBuilder: mvcBuilder, configure: null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mvcBuilder"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static ModuleMvcBuilder AddModules(this IMvcBuilder mvcBuilder, Action<ModulesProviderContext> configure)
        {
            var modulesProvider = mvcBuilder.Services.GetServiceFromCollection<IModulesProvider>();

            if (modulesProvider is null)
            {
                var options = new ModulesOptions();
                var loggerFactory = LoggerFactory.Create(builder => configure?.Invoke(new ModulesProviderContext(options, builder)));

                modulesProvider = new DefaultModulesProvider(
                    options: options,
                    environment: mvcBuilder.Services.GetServiceFromCollection<IWebHostEnvironment>(),
                    loggerFactory: mvcBuilder.Services.GetServiceFromCollection<ILoggerFactory>() ?? loggerFactory
                );

                mvcBuilder.Services.AddSingleton(modulesProvider);
            }

            foreach (var module in modulesProvider.Modules)
            {
                if (!mvcBuilder.AssemblyPartExists(module.OrginalAssembly))
                {
                    mvcBuilder.AddApplicationPart(module.OrginalAssembly);
                }

                foreach (var assembly in module.LoadContext.Assemblies)
                {
                    if (!mvcBuilder.AssemblyPartExists(assembly))
                    {
                        mvcBuilder.AddApplicationPart(assembly);
                    }
                }

                var types = module
                    .OrginalAssembly
                    .ExportedTypes
                    .Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericType && typeof(IModuleConfigureServices).IsAssignableFrom(t));

                var serviceProvider = mvcBuilder.Services.BuildServiceProvider(validateScopes: true);

                foreach (var type in types)
                {
                    var configureServices = ActivatorUtilities.CreateInstance(serviceProvider, type) as IModuleConfigureServices;
                    configureServices.ConfigureServices(services: mvcBuilder.Services);
                }
            }

            return new ModuleMvcBuilder(mvcBuilder, modulesProvider);
        }

        private static bool AssemblyPartExists(this IMvcBuilder builder, Assembly assembly)
        {
            return builder
              .PartManager
              .ApplicationParts
              .OfType<AssemblyPart>()
              .Any(x => x.Assembly.Equals(assembly));
        }

        private static T GetServiceFromCollection<T>(this IServiceCollection services)
        {
            return (T)services.LastOrDefault(d => d.ServiceType == typeof(T))?.ImplementationInstance;
        }
    }
}
