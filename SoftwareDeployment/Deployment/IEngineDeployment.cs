using System;
using System.Threading.Tasks;

namespace SoftwareDeployment.Deployment
{
    public interface IEngineDeployment
    {
        public static string TessdataDirectory { get; } = "G:/TessDirectory";

        public static string InstallationPath { get; set; } =
            Environment.GetFolderPath(Environment.SpecialFolder.Personal);

        Task<bool> Deploy(string sourceDirectory, string destinationDirectory);
    }
}