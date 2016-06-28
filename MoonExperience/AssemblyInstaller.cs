namespace MoonExperience
{
    using System.Diagnostics;

    /// <summary>
    ///     Installs assemblies by their URL.
    /// </summary>
    internal class AssemblyInstaller
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Installs the assembly.
        /// </summary>
        /// <param name="urls">The urls.</param>
        public static void InstallAssembly(string[] urls)
        {
            foreach (var url in urls)
            {
                Process.Start(new ProcessStartInfo(url) { CreateNoWindow = true });
            }
        }

        #endregion
    }
}