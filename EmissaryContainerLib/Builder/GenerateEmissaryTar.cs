using System;
using System.IO;
using ICSharpCode.SharpZipLib.Tar;

namespace NeoWorlds.EmissaryContainerLib.Builder
{
    class GenerateTar
    {
        public GenerateTar()
        {
            // The directory to be archived
            string SolutionPath = @"C:\Projects\HelloWorldEmissary\BouncySphere\";

            // Open the "solution" yaml in base dir
            // Look for dirty projects

            // Loop through projects
            // Pull correct files into directories and write with corrected values
            // Track the hashes as we go - likely files will have to be read twice for first version

            // Write beginning of manifest (first versions will not be signed)

            // Checksum the tar file as it's being written (sha256)

            var projects = new LoadEntityProjects(Path.Combine(SolutionPath, "EntityProjects.yaml"));

            // The path of the tar archive to be created
            string tarFilePath = @"C:\example\directory.tar";

            // Create a TarOutputStream object
            using (var tarFileStream = new HashStream(File.Create(tarFilePath), System.Security.Cryptography.SHA256.Create()))
            using (var tarOutputStream = new TarOutputStream(tarFileStream, System.Text.Encoding.ASCII))
            {
                // Get all files in the directory
                string[] files = Directory.GetFiles(SolutionPath, "*.*", SearchOption.AllDirectories);

                // Add each file to the tar archive
                foreach (string file in files)
                {
                    // Create a TarEntry object
                    TarEntry tarEntry = TarEntry.CreateEntryFromFile(file);

                 


                    // Add the TarEntry object to the TarOutputStream
                    tarOutputStream.PutNextEntry(tarEntry);

                    // Write the file data to the TarOutputStream
                    using (var fileStream = new HashStream(File.OpenRead(file), System.Security.Cryptography.SHA256.Create()))
                    {
                        // This isn't right, we have to loop over the buffer here
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
