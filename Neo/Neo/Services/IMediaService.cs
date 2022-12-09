using System.IO;

namespace Neo.Services
{
    public interface IMediaService
    {
        void SavePicture(string name, Stream data, string location="temp");
    }
}