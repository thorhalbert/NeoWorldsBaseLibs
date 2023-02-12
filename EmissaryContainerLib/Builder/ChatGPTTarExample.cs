// 2023/2/11 - Hey ChatGPT: show me an example of writing a tar file in c# with sharpziplib


using System;
using System.IO;
using ICSharpCode.SharpZipLib.Tar;

namespace TarExample
{
    class Program
    {
        static void Main(string[] args)
        {
            // The directory to be archived
            string directoryPath = @"C:\example\directory";

            // The path of the tar archive to be created
            string tarFilePath = @"C:\example\directory.tar";

            // Create a TarOutputStream object
            using (Stream tarFileStream = File.Create(tarFilePath))
            using (TarOutputStream tarOutputStream = new TarOutputStream(tarFileStream))
            {
                // Get all files in the directory
                string[] files = Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories);

                // Add each file to the tar archive
                foreach (string file in files)
                {
                    // Create a TarEntry object
                    TarEntry tarEntry = TarEntry.CreateEntryFromFile(file);

                    // Add the TarEntry object to the TarOutputStream
                    tarOutputStream.PutNextEntry(tarEntry);

                    // Write the file data to the TarOutputStream
                    using (FileStream fileStream = File.OpenRead(file))
                    {
                        byte[] buffer = new byte[fileStream.Length];
                        fileStream.Read(buffer, 0, buffer.Length);
                        tarOutputStream.Write(buffer, 0, buffer.Length);
                    }
                }
            }

            Console.WriteLine("Tar archive created successfully!");
        }
    }
}
