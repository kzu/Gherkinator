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
		Microsoft.Build.Locator.MSBuildLocator.RegisterMSBuildPath(ThisAssembly.Metadata.MSBuildBinPath);
        OnRun();
	}

    /// <summary>
    /// Declare in another partial class to extend what is run when the assembly is 
    /// loaded.
    /// </summary>
    static partial void OnRun();
}