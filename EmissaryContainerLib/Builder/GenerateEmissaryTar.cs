using System;
using System.IO;
using System.IO.Pipes;
using System.Xml.Linq;
using System.CommandLine;
using EmissaryContainerLib.Builder;
using ICSharpCode.SharpZipLib.Tar;
using SharpCompress.Compressors.Xz;
using static NeoWorlds.EmissaryContainerLib.Builder.LoadEntityProjects;

namespace NeoWorlds.EmissaryContainerLib.Builder
{
    public class GenerateTar : Command
    {
        public GenerateTar(string verb, string? description) : base(verb, description)
        {
           

        }

        public void Process()
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

            foreach (var project in projects.EntityProjects)
            {
                var dir = Path.Combine(SolutionPath, project.Directory);
                Console.WriteLine($"Project: {project.Name} - {dir}");
                ProcessProject(project, dir, projects);
            }
        }

        internal void ProcessProject(EntityProjectListing projectListing, string projectDir, LoadEntityProjects projects)
        {
            var di = new FileInfo(projectDir);
            projectDir = di.FullName;

            var projectFile = Path.Combine(projectDir, "_adm", "EntityProject.yaml");
            var project = EntityProject.LoadProject(projectFile);

            var manifest = new ManifestHeader { Project = project };

            var specDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            var emissaryBuildPath = Path.Combine(specDir, "NeoWorldsArena", "EmissaryBuild");
            Directory.CreateDirectory(emissaryBuildPath);

            var tarFilePath = Path.Combine(emissaryBuildPath, $"{project.ProjectId}.tartmp");

            ManifestEntry me;
            ManifestExtendedHeader extendedManifest;
            string emissaryHash;

            // Lots more abstraction can happen in this section

            // Create a TarOutputStream object
            using (var rawFileStream = File.Create(tarFilePath))
            using (var tarFileStream = new HashStream(rawFileStream, System.Security.Cryptography.SHA256.Create()))
            {
                using (var tarOutputStream = new TarOutputStream(tarFileStream, 1, System.Text.Encoding.ASCII))
                {
                    // Add each file to the tar archive
                    Environment.CurrentDirectory = projectDir;  // Want to be our default curdir during assembly
                    foreach (var (dir, file) in PursueDirTree(projectDir, projectDir))
                    {
                        if (!evaluateFile(dir, file, projectDir, project, tarOutputStream, out var tarEntry, out var procFile))
                            continue;

                        // Add the TarEntry object to the TarOutputStream
                        tarOutputStream.PutNextEntry(tarEntry);

                        tarOutputStream.Flush();  // Position is just off by 512 -- have to figure out why
                        me = manifest.AddEntry(tarEntry, tarFileStream.Position + 512);

                        // Write the file data to the TarOutputStream
                        using (var fileStream = File.OpenRead(procFile))
                            WriteTarStreamEntry(tarOutputStream, tarEntry, fileStream, Manifest: me);
                    }

                    var (projstr, projlen) = project.DumpProject();
                    var admProjectEntry = CreateEntry(null, "_adm", "EntityProject.yaml", projectFile, projlen);
                    tarOutputStream.PutNextEntry(admProjectEntry);

                    tarOutputStream.Flush();
                    me = manifest.AddEntry(admProjectEntry, tarFileStream.Position + 512);
                    WriteTarStreamEntry(tarOutputStream, admProjectEntry, projstr, projlen, Manifest: me);

                    var (manstr, manlen) = manifest.DumpManifest();
                    var admManifestEntry = CreateEntry(null, "_adm", "Manifest.yaml", null, manlen);
                    tarOutputStream.PutNextEntry(admManifestEntry);
                    // Of course we can't write the manifest entry for the manifest itself -- this
                    // has to be negotiated with the entity server and will be written in a separate file
                    tarOutputStream.Flush();
                    me = manifest.AddEntry(admManifestEntry, tarFileStream.Position + 512);
                    WriteTarStreamEntry(tarOutputStream, admManifestEntry, manstr, manlen, Manifest: me);
                }

                emissaryHash = Convert.ToHexString(tarFileStream.Hash());
                Console.WriteLine($"Emissary Hash={emissaryHash}");

                extendedManifest = new ManifestExtendedHeader(manifest);
                extendedManifest.Manifest = me;
                extendedManifest.EmissarySHA256 = emissaryHash;
            }

            // Emissary is closed -- now rename it into it's final form.

            var buildName = $"{project.ProjectId}-{DateTime.UtcNow.ToString("yyyyMMddHHmm")}-{emissaryHash}";
            extendedManifest.BuildName = buildName;
            var finalTarPathBase = Path.Combine(emissaryBuildPath, $"{buildName}");
            var buildFile = new FileInfo(tarFilePath);

            var emissaryFile = finalTarPathBase + ".emissary";
            var emissaryYaml = finalTarPathBase + ".manifest.yaml";

            buildFile.MoveTo(emissaryFile);

            using (var manStream = new FileStream(emissaryYaml, FileMode.Create, FileAccess.Write))
            {
                var (dumpStream, dumpLength) = extendedManifest.DumpManifest();
                dumpStream.CopyTo(manStream);
            }

            var checker = new CheckEmissary(extendedManifest, emissaryFile);

            UploadEmissary(projects, extendedManifest, emissaryFile);

            Console.WriteLine("Tar archive created successfully!");
        }

        private void UploadEmissary(LoadEntityProjects projects, ManifestExtendedHeader extendedManifest, string emissaryFile)
        {
            
        }

        private static void WriteTarStreamEntry(TarOutputStream tarOutputStream, TarEntry? tarEntry, Stream fileStream,
            int streamLen=0, ManifestEntry? Manifest = null)
        {
            using (var hashStream = new HashStream(new BinaryReader(fileStream).BaseStream, System.Security.Cryptography.SHA256.Create()))
            {
                // This isn't right, we have to loop over the buffer here
                var len = hashStream.Length;
                if (streamLen > 0) len = streamLen;

                byte[] buffer = new byte[len];
                hashStream.Read(buffer, 0, buffer.Length);
                tarOutputStream.Write(buffer, 0, buffer.Length);
                hashStream.Close();

                tarOutputStream.CloseEntry();

                var hash = Convert.ToHexString(hashStream.Hash());

                if (Manifest != null)
                    Manifest.SHA256 = hash;

                Console.WriteLine($"File: {tarEntry.Name} {hashStream.Length} Hash={hash}");
            }
        }


        private bool evaluateFile(string dir, string file, string topDir,
            EntityProject project, TarOutputStream tarOutputStream,
            out TarEntry? tarEntry, out string? procFile)
        {
            // Create a TarEntry object
            tarEntry = null;
            procFile = null;

            if (dir.Length < 1) return false;  // Nothing from top level dir

            if (dir==".vs" || dir=="obj") return false;
            if (dir.StartsWith(".vs" + Path.DirectorySeparatorChar)) return false;
            if (dir.StartsWith("obj" + Path.DirectorySeparatorChar)) return false;

            var header = new TarHeader();

            procFile = Path.Combine(topDir, dir, file);

            // Create will load us up a full tar entry - then we can fiddle with path
            tarEntry = TarEntry.CreateEntryFromFile(procFile);

            // _dat (data) heirarchy fully passed down (this is entity private data)
            if (dir=="_dat" || (dir.StartsWith("_dat" + Path.DirectorySeparatorChar))){
                tarEntry = CreateEntry(tarEntry, dir, file);
                return true;
            }

            var dotNet = @"bin\Debug\net7.0";
            if (dir == dotNet)
            {
                if (file == project.Binary)
                {
                    tarEntry = CreateEntry(tarEntry, "_bin", project.Binary);                
                    return true;
                }

                return false;
            }

            switch (dir.ToLower())
            {
                case "_adm":
                    return false;  // We will parse these and write them ourselves
                case "_pro":
                    tarEntry = CreateEntry(tarEntry, dir, file);
                    return true;
            }

            return false;
        }

        private TarEntry CreateEntry(TarEntry? tarEntry, string dir, string file, string? rawFile=null, int length=0)
        {
            dir = dir.Replace('\\', '/');  // Our tar paths are unixy -- don't allow backslashes

            var name = $"{dir}/{file}";
            if (tarEntry == null)
                if (!string.IsNullOrWhiteSpace(rawFile))
                    tarEntry = TarEntry.CreateEntryFromFile(rawFile);
                else
                    tarEntry = TarEntry.CreateTarEntry(name);
            var v = tarEntry.TarHeader;
            v.Name = name;
            if (length > 0)
                v.Size = length;

            var newTarEntry = new TarEntry(v);

            return newTarEntry;
        }

        private IEnumerable<(string, string)> PursueDirTree(string topDir, string startDir)
        {
            var di = new FileInfo(topDir);  //Normalize
            var topS = di.FullName;         
            var topD = topS+ Path.DirectorySeparatorChar;

            foreach (var f in Directory.EnumerateFiles(startDir))
            {
                var fi = new FileInfo(f);
                var pa = fi.DirectoryName;

                if (pa.StartsWith(topD))
                    pa = pa[topD.Length..];
                else if (pa.StartsWith(topS))
                    pa = pa[topS.Length..];

                yield return (pa, fi.Name);
            }

            // Descend and Recurse, but we still have to yield it
            foreach (var d in Directory.EnumerateDirectories(startDir))
                foreach (var (c1, c2) in PursueDirTree(topDir, d))
                    yield return (c1, c2);
        }

       
    }
}