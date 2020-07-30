using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MovieLand.BLL.Contracts
{
    public interface IFileClient
    {
        void DeleteFile(string storeName, string filePath);
        bool FileExists(string storeName, string filePath);
        Stream GetFile(string storeName, string filePath);
        Task SaveFileAsync(string storeName, string filePath, Stream fileStream);
    }
}
