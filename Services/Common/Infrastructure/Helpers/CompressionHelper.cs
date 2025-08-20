using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Common.Infrastructure.Helpers
{
    public static class CompressionHelper
    {
        public static byte[] Compress(byte[] data)
        {
            byte[] compressArray = null;
            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
                    {

                        gzipStream.Write(data, 0, data.Length);
                    }
                    compressArray = memoryStream.ToArray();
                }
            }
            catch //(Exception exception)
            {
                // do something !
            }
            return compressArray;
        }
        public static byte[] Decompress(byte[] data)
        {
            byte[] decompressedArray = null;
            try
            {
                using (MemoryStream decompressedStream = new MemoryStream())
                {
                    using (MemoryStream compressStream = new MemoryStream(data))
                    {
                        using (GZipStream gzipStream = new GZipStream(compressStream, CompressionMode.Decompress))
                        {
                            gzipStream.CopyTo(decompressedStream);
                        }
                    }
                    decompressedArray = decompressedStream.ToArray();
                }
            }
            catch //(Exception exception)
            {
                // do something !
            }
            return decompressedArray;
        }
        /// <summary>
        /// Create a ZIP file of the files provided.
        /// </summary>
        /// <param name="fileName">The full path and name to store the ZIP file at.</param>
        /// <param name="files">The list of files to be added.</param>
        public static void CreateZipFile(string fileName, IEnumerable<string> files)
        {
            // Create and open a new ZIP file
            var zip = ZipFile.Open(fileName, ZipArchiveMode.Create);
            foreach (var file in files)
            {
                // Add the entry for each file
                zip.CreateEntryFromFile(file, Path.GetFileName(file), CompressionLevel.Optimal);
            }
            // Dispose of the object when we are done
            zip.Dispose();
        }
        public static List<Stream> UnZipStream(Stream zipStream)
        {
            List<Stream> unzippedEntryStream = new List<Stream>(); // Unzipped data from a file in the archive
            ZipArchive archive = new ZipArchive(zipStream);
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                unzippedEntryStream.Add(entry.Open()); // .Open will return a stream
            }
            return unzippedEntryStream;
        }
        public static List<UnzipStream> UnZipStreamWithName(Stream zipStream)
        {
            List<UnzipStream> unzippedEntryStream = new List<UnzipStream>(); // Unzipped data from a file in the archive
            ZipArchive archive = new ZipArchive(zipStream);
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                unzippedEntryStream.Add(new UnzipStream { stream = entry.Open(), fileName = entry.FullName }); // .Open will return a stream
            }
            return unzippedEntryStream;
        }
        public static List<Stream> UnZipStreamFindFile(Stream zipStream, string fileNameToFind)
        {
            List<Stream> unzippedEntryStream = new List<Stream>(); // Unzipped data from a file in the archive
            ZipArchive archive = new ZipArchive(zipStream);
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                unzippedEntryStream.Add(entry.Open()); // .Open will return a stream
            }
            return unzippedEntryStream;
        }

        public static void UnZipFiles(Stream zipStream, string extractToDirectory)
        {
            //Create and open a new ZIP file
            ZipArchive archive = new ZipArchive(zipStream);
            foreach (var fileEntry in archive.Entries)
            {
                fileEntry.ExtractToFile(fileEntry.Name, true);
            }
        }
    }
    public class UnzipStream
    {
        public Stream stream { get; set; }
        public string fileName { get; set; }
    }
}
