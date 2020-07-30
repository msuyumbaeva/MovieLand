using MovieLand.BLL.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MovieLand.BLL.Services
{
    public class LocalFileClient : IFileClient
    {
        private string _fileRoot;

        public LocalFileClient(string fileRoot) {
            _fileRoot = fileRoot;
        }

        public void DeleteFile(string storeName, string filePath) {
            var path = Path.Combine(_fileRoot, storeName, filePath);

            if (File.Exists(path)) {
                File.Delete(path);
            }
        }

        public bool FileExists(string storeName, string filePath) {
            var path = Path.Combine(_fileRoot, storeName, filePath);

            return File.Exists(path);
        }

        public Stream GetFile(string storeName, string filePath) {
            var path = Path.Combine(_fileRoot, storeName, filePath);
            Stream stream = null;

            if (File.Exists(path)) {
                stream = File.OpenRead(path);
            }

            return stream;
        }

        public async Task SaveFileAsync(string storeName, string filePath, Stream fileStream) {
            var path = Path.Combine(_fileRoot, storeName, filePath);

            if (File.Exists(path)) {
                File.Delete(path);
            }

            using (var file = new FileStream(path, FileMode.CreateNew)) {
                await fileStream.CopyToAsync(file);
            }
        }
    }
}
