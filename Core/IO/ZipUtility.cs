﻿using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;

namespace Core.IO
{
    public static class ZipUtility
    {
        // Compresses the files in the nominated folder, and creates a zip file 
        // on disk named as outPathname.
        public static void ZipFolder(string filePath, string folderName)
        {
            using(var zipFile = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            using(var zipStream = new ZipOutputStream(zipFile)) {
                zipFile.SetLength(0);
                zipStream.SetLevel(0);
                // This setting will strip the leading part of the folder path in the entries, 
                // to make the entries relative to the starting folder.
                // To include the full path for each entry up to the drive root, assign to 0.
                int folderOffset = folderName.Length + (folderName.EndsWith("\\") ? 0 : 1);

                CompressFolder(folderName, zipStream, folderOffset);
                zipStream.Finish();
            }
        }


        // Recursively compresses a folder structure
        private static void CompressFolder(string path, ZipOutputStream zipStream, int folderOffset)
        {
            var files = Directory.GetFiles(path);

            foreach(var filename in files) {

                var fi = new FileInfo(filename);

                // Make the name in zip based on the folder
                var entryName = filename.Substring(folderOffset);

                // Remove drive from name and fixe slash direction
                entryName = ZipEntry.CleanName(entryName);

                var newEntry = new ZipEntry(entryName);

                // Note the zip format stores 2 second granularity
                newEntry.DateTime = fi.LastWriteTime;

                // Specifying the AESKeySize triggers AES encryption. 
                // Allowable values are 0 (off), 128 or 256.
                // A password on the ZipOutputStream is required if using AES.
                //   newEntry.AESKeySize = 256;

                // To permit the zip to be unpacked by built-in extractor in WinXP and Server2003,
                // WinZip 8, Java, and other older code, you need to do one of the following: 
                // Specify UseZip64.Off, or set the Size.
                // If the file may be bigger than 4GB, or you do not need WinXP built-in compatibility, 
                // you do not need either, but the zip will be in Zip64 format which
                // not all utilities can understand.
                //   zipStream.UseZip64 = UseZip64.Off;
                newEntry.Size = fi.Length;

                zipStream.PutNextEntry(newEntry);

                // Zip the file in buffered chunks
                // the "using" will close the stream even if an exception occurs
                var buffer = new byte[4096];
                using(FileStream fsInput = File.OpenRead(filename)) {
                    StreamUtils.Copy(fsInput, zipStream, buffer);
                }
                zipStream.CloseEntry();
            }

            // Recursively call CompressFolder on all folders in path
            var folders = Directory.GetDirectories(path);
            foreach(var folder in folders) {
                CompressFolder(folder, zipStream, folderOffset);
            }
        }

        public static void ExtractZipFile(string filePath, string outFolder)
        {
            using(ZipFile zf = new ZipFile(filePath)) {
                foreach(ZipEntry zipEntry in zf) {
                    if(!zipEntry.IsFile) {
                        // Ignore directories
                        continue;
                    }
                    string entryFileName = zipEntry.Name;
                    // to remove the folder from the entry:
                    //entryFileName = Path.GetFileName(entryFileName);
                    // Optionally match entrynames against a selection list here
                    // to skip as desired.
                    // The unpacked length is available in the zipEntry.Size property.

                    // Manipulate the output filename here as desired.
                    var fullZipToPath = Path.Combine(outFolder, entryFileName);
                    var directoryName = Path.GetDirectoryName(fullZipToPath);
                    if(directoryName.Length > 0) {
                        Directory.CreateDirectory(directoryName);
                    }

                    // 4K is optimum
                    var buffer = new byte[4096];

                    // Unzip file in buffered chunks. This is just as fast as unpacking
                    // to a buffer the full size of the file, but does not waste memory.
                    // The "using" will close the stream even if an exception occurs.
                    using(var zipStream = zf.GetInputStream(zipEntry))
                    using(Stream fsOutput = File.Create(fullZipToPath)) {
                        StreamUtils.Copy(zipStream, fsOutput, buffer);
                    }
                }
            }
        }
    }
}
