using System;
using System.IO;
using System.Threading.Tasks;

namespace SoftwareDeployment.Deployment
{
    public abstract class EngineDeployment : IEngineDeployment
    {
        public async Task Deploy(string sourceDirectory, string destinationDirectory, bool recursive)
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
            if (recursive)
            {
                foreach (var subDir in dirs)
                {
                    var newDestinationDir = Path.Combine(destinationDirectory, subDir.Name);
                    Deploy(subDir.FullName, newDestinationDir, true);
                }
            }
        }
    }
}