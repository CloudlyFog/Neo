using System;
using System.IO;
using Neo.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(MediaService))]

namespace Neo.Services
{
    public class MediaService : IMediaService
    {
        public void SavePicture(string name, Stream data, string location = "temp")
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            documentsPath = Path.Combine(documentsPath, location);
            Directory.CreateDirectory(documentsPath);

            var filePath = Path.Combine(documentsPath, name);

            var bArray = new byte[data.Length];
            using var fs = new FileStream(filePath, FileMode.OpenOrCreate);
            using (data)
            {
                data.Read(bArray, 0, (int)data.Length);
            }

            var length = bArray.Length;
            fs.Write(bArray, 0, length);
        }
    }
}