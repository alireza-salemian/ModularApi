using DoctorCode.AspNetCore.Modules.Abstrctions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace DoctorCode.AspNetCore.Modules
{
    /// <summary>
    /// 
    /// </summary>
    public class DefaultModulesProvider : IModulesProvider
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<DefaultModulesProvider> _logger;
        private readonly DirectoryInfo _modulesDirectory;
        private readonly DirectoryInfo _modulesCacheDirectory;

        private readonly Lazy<IReadOnlyCollection<ModuleInfo>> _modules;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="environment"></param>
        /// <param name="loggerFactory"></param>
        public DefaultModulesProvider(ModulesOptions options, IWebHostEnvironment environment, ILoggerFactory loggerFactory)
        {
            if (loggerFactory is null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            Options = options ?? throw new ArgumentNullException(nameof(options));
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _logger = loggerFactory.CreateLogger<DefaultModulesProvider>();

            _modulesDirectory = new DirectoryInfo(Path.Combine(_environment.ContentRootPath, NormalizePath(Options.ModulesPath)));
            _modulesCacheDirectory = new DirectoryInfo(Path.Combine(_modulesDirectory.FullName, NormalizePath(Options.CachePath)));

            _modules = new Lazy<IReadOnlyCollection<ModuleInfo>>(LazyLoadModules, true);
        }

        /// <summary>
        /// 
        /// </summary>
        public ModulesOptions Options { get; }

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyCollection<ModuleInfo> Modules => _modules.Value;

        private List<ModuleInfo> LazyLoadModules()
        {
            var modules = new List<ModuleInfo>();

            try
            {
                if (!_modulesDirectory.Exists || !_modulesCacheDirectory.Exists)
                {
                    _modulesCacheDirectory.Create();
                }

                var modulesFiles = _modulesDirectory
                    .EnumerateFiles()
                    .Where(f => f.Extension.Equals(Options.ModuleFileExtenstion, StringComparison.OrdinalIgnoreCase));

                foreach (var file in modulesFiles)
                {
                    try
                    {
                        modules.Add(GetModuleInfo(file));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Load module in path `{0}` failed", file.FullName);

                        if (Options.SuppressModuleLoadingException == false)
                        {
                            throw new LoadModuleExecption(file.FullName);
                        }
                    }
                }
            }
            catch (Exception ex) when(!(ex is LoadModuleExecption))
            {
                _logger.LogError(ex, "There was an error loading the modules.");
            }

            return modules;
        }

        private ModuleInfo GetModuleInfo(FileInfo fileInfo)
        {
            var mustExtract = false;
            var moduleName = Path.GetFileNameWithoutExtension(fileInfo.Name);
            var extractPath = Path.Combine(_modulesCacheDirectory.FullName, moduleName);
            var extractDirectory = new DirectoryInfo(extractPath);

            if (!extractDirectory.Exists)
            {
                mustExtract = true;
            }
            else if (fileInfo.CreationTime > extractDirectory.CreationTime ||
                     fileInfo.LastWriteTime > extractDirectory.LastWriteTime ||
                     fileInfo.LastWriteTime > extractDirectory.CreationTime)
            {
                extractDirectory.Delete(true);
                mustExtract = true;
            }

            if (mustExtract)
            {
                ZipFile.ExtractToDirectory(fileInfo.FullName, extractPath);
            }

            var mainModuleFile = new FileInfo(Path.Combine(extractPath, $"{moduleName}.dll"));

            if (!mainModuleFile.Exists)
            {
                throw new FileNotFoundException($"Main module assembly not found. expected in path: `{mainModuleFile.FullName}`");
            }

            return new ModuleInfo(mainModuleFile);
        }

        private string NormalizePath(string path)
        {
            return Path.Combine(
                path.Replace('\\', Path.DirectorySeparatorChar)
                    .Replace('/', Path.DirectorySeparatorChar))
                .Trim(Path.DirectorySeparatorChar);
        }
    }
}
