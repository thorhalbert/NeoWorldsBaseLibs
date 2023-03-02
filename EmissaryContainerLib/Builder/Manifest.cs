using ICSharpCode.SharpZipLib.Tar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using CanonicalGrpcProtos;
using static NeoWorlds.EmissaryContainerLib.Builder.LoadEntityProjects;

namespace EmissaryContainerLib.Builder
{
    public class ManifestExtendedHeader : ManifestHeader
    { 
        public ManifestEntry Manifest { get; set; }
        public string EmissarySHA256 { get;set; }
        public Int64 EmissarySize { get; set; }
        public string BuildName { get; internal set; }

        public ManifestExtendedHeader(ManifestHeader m)
        {
            Project = m.Project;
            FileSystem = m.FileSystem;
        }

        public EntityManifestHeader GetProto()
        {
            var ret = new EntityManifestHeader {
                BuildName = BuildName,
                Manifest = Manifest.GetProto(),
                EmissarySHA256 = EmissarySHA256,
                EntityProject = Project.GetProto(),
                EmissarySize = EmissarySize
            };

            foreach (var f in FileSystem)
                ret.ManifestEntries.Add(f.GetProto());

            return ret;
        }
    }
    /// <summary>
    /// This is the Emissary manifest. 
    /// </summary>
    public class ManifestHeader
    {
        public EntityProject Project { get; set; }
        public List<ManifestEntry> FileSystem { get; set; } = new();

        public (Stream, int) DumpManifest()
        {
            var serializer = new SerializerBuilder().Build();

            var body = serializer.Serialize(this);
            return (new MemoryStream(Encoding.UTF8.GetBytes(body)), body.Length);
        }

        internal ManifestEntry AddEntry(TarEntry tarEntry, long position)
        {
            var entry = new ManifestEntry
            {
                Path = tarEntry.Name,
                FileIdx = 0,
                Offset = position,
                Size = tarEntry.Size,
            };

            FileSystem.Add(entry);

            return entry;
        }
    }

    /// <summary>
    /// Each file in the emissary, though this does not include the manifest itself
    /// (it really can't, since it can't know it's own hash deterministically)
    /// Signature of the manifest is handled separately as is the signature of
    /// the emissary itself
    /// </summary>
    public class ManifestEntry
    {
        // Whole path (does not start with slash)
        public string Path { get; set; }
        // Size of file -- future proof for 64 bits
        public Int64 Size { get; set; }
        // Which file -- almost always zero for now
        public int FileIdx { get; set; } = 0;  
        // Offset into the emissary (with this and Size you can randomly seek)
        public Int64 Offset { get; set; }
        // Hash of the file (computed by builder as it's making it)
        public string SHA256 { get; set; }

        // Someday we may add digital signature stuff and possibily 
        // metadata and mimetype, etc -- most of the latter is up to 
        // the entity

        public EmissaryManifestEntry GetProto()
        {
            var ret = new EmissaryManifestEntry
            {
                Path = Path,
                Size = Size,
                FileIdx = FileIdx,
                Offset = Offset,
                SHA256 = SHA256
            };

            return ret;
        }
    }
}
