using System;
using System.IO;
using System.Linq;
using System.Reflection;

/// <summary>
/// Used by the InjectModuleInitializer. All code inside the Run method is ran as soon as the assembly is loaded.
/// </summary>
internal static partial class ModuleInitializer
{
    /// <summary>
    /// Initializes the module.
    /// </summary>
    internal static void Run()
    {
        if (!AppDomain.CurrentDomain.GetAssemblies().Any(x => x.GetName().Name == "Microsoft.Build"))
        {
            var binPath = typeof(ModuleInitializer)
                .Assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
                .Where(x => x.Key == "MSBuildBinPath")
                .Select(x => x.Value)
                .First();

            Microsoft.Build.Locator.MSBuildLocator.RegisterMSBuildPath(binPath);

            // Set environment variables so SDKs can be resolved. 
            Environment.SetEnvironmentVariable("MSBUILD_EXE_PATH", Path.Combine(binPath, "MSBuild.exe"), EnvironmentVariableTarget.Process);
        }

        OnRun();
	}

    /// <summary>
    /// Declare in another partial class to extend what is run when the assembly is 
    /// loaded.
    /// </summary>
    static partial void OnRun();
}