using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Core.IO
{
    public class FileHolder : IDisposable
    {
        private Dictionary<string, FileStream> filesDictionary = new Dictionary<string, FileStream>();

        public FileStream GetFileStream(string filePath)
        {
            if(filesDictionary.ContainsKey(filePath)) {
                return filesDictionary[filePath];
            }
            return null;
        }

        public void HoldFile(FileStream fileStream)
        {
            if(fileStream is null) {
                throw new ArgumentNullException(nameof(fileStream));
            }

            if(filesDictionary.ContainsKey(fileStream.Name)) {
                if(filesDictionary[fileStream.Name] != fileStream) {
                    UnholdAndCloseFile(filesDictionary[fileStream.Name]);
                    filesDictionary.Add(fileStream.Name, fileStream);
                }
                return;
            }
            filesDictionary.Add(fileStream.Name, fileStream);
        }

        public void UnholdAndCloseFile(string filePath)
        {
            FileStream stream = GetFileStream(filePath);
            if(stream == null) {
                return;
            }
            UnholdAndCloseFile(stream);
        }

        public void UnholdAndCloseFile(FileStream fileStream)
        {
            UnholdFile(fileStream);
            fileStream.Dispose();
        }

        public void UnholdFile(FileStream fileStream)
        {
            if(fileStream is null) {
                throw new ArgumentNullException(nameof(fileStream));
            }

            if(filesDictionary.ContainsKey(fileStream.Name)) {
                filesDictionary.Remove(fileStream.Name);
            }
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if(!disposedValue) {
                if(disposing) {
                    // TODO: dispose managed state (managed objects).
                }

                foreach(var fileKey in filesDictionary.Keys.ToList()) {
                    filesDictionary[fileKey]?.Dispose();
                    filesDictionary.Remove(fileKey);
                }

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~FileHolder()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}
