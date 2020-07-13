using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace DoctorCode.AspNetCore.Modules.Abstrctions
{
    /// <summary>
    /// 
    /// </summary>
    public class ModuleInfo
    {
        private string _version;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="moduleFileInfo"></param>
        public ModuleInfo(FileInfo moduleFileInfo)
        {
            FileInfo = moduleFileInfo ?? throw new ArgumentNullException(nameof(moduleFileInfo));

            if (!moduleFileInfo.Exists)
            {
                throw new FileNotFoundException("The module file not found: ", moduleFileInfo.FullName);
            }

            LoadContext = new ModuleLoadContext(moduleFileInfo);
            Name = Path.GetFileNameWithoutExtension(moduleFileInfo.Name);
            OrginalAssembly = LoadContext.LoadFromAssemblyName(new AssemblyName(Name));
        }

        /// <summary>
        /// 
        /// </summary>
        public FileInfo FileInfo { get; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 
        /// </summary>
        public AssemblyLoadContext LoadContext { get; }

        /// <summary>
        /// Get the original assembly file that a shadow copy was made from it.
        /// </summary>
        public Assembly OrginalAssembly { get; }

        /// <summary>
        /// 
        /// </summary>
        public string Version => _version ??= OrginalAssembly.GetName().Version.ToString();
    }
}
