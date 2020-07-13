using DoctorCode.AspNetCore.Modules.Abstrctions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;
using System.IO;
using System.Linq;

namespace DoctorCode.AspNetCore.Modules
{
    /// <summary>
    /// 
    /// </summary>
    public static class ModulesApplicationBuilderExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseModules(this IApplicationBuilder app)
        {
            var modulesProvider = app.ApplicationServices.GetRequiredService<IModulesProvider>();

            foreach (var module in modulesProvider.Modules)
            {
                var webroot = module.FileInfo.Directory.GetDirectories("wwwroot", SearchOption.TopDirectoryOnly).FirstOrDefault();
                if (webroot != null)
                {
                    app.UseStaticFiles(new StaticFileOptions()
                    {
                        FileProvider = new PhysicalFileProvider(webroot.FullName)
                    });
                }

                var types = module
                    .OrginalAssembly
                    .ExportedTypes
                    .Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericType && typeof(IModuleConfigure).IsAssignableFrom(t));

                //TODO: It's required to order configures for some purpose such as Middlewares
                foreach (var type in types)
                {
                    var moduleConfigure = Activator.CreateInstance(type) as IModuleConfigure;
                    moduleConfigure.Configure(app);
                }
            }

            return app;
        }
    }
}
