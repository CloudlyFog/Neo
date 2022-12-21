using System;
using System.IO;
using System.Threading.Tasks;

namespace SoftwareDeployment.Deployment
{
    public sealed class EngineDeployment : IEngineDeployment
    {
        private const string TessDirectory = "G:/TessDirectory";

        public string InstallationPath { get; set; } =
            Environment.GetFolderPath(Environment.SpecialFolder.Personal);

        public async Task<bool> Deploy(string sourceDirectory, string destinationDirectory)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDirectory);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            var dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDirectory);

            // Get the files in the source directory and copy to the destination directory
            foreach (var file in dir.GetFiles())
            {
                var targetFilePath = Path.Combine(destinationDirectory, file.Name);
                file.CopyTo(targetFilePath);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (true)
            {
                foreach (var subDir in dirs)
                {
                    var newDestinationDir = Path.Combine(destinationDirectory, subDir.Name);
                    Deploy(subDir.FullName, newDestinationDir);
                }
            }

            return true;
        }
    }
}