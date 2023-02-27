using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EmissaryContainerLib.Builder
{
    public class CheckEmissary
    {
        public CheckEmissary(ManifestExtendedHeader manifest, string manifestFile)
        {
            using var maniStream = new BinaryReader(File.Open(manifestFile, FileMode.Open));

            checkFile(maniStream, manifest.Manifest);
            foreach (var file in manifest.FileSystem)
                checkFile(maniStream, file);
        }

        private void checkFile(BinaryReader maniStream, ManifestEntry file)
        {
            maniStream.BaseStream.Seek((int) file.Offset, SeekOrigin.Begin);

            var stoSpan = new byte[file.Size];
            var contents = maniStream.Read(stoSpan, 0, stoSpan.Length);

            string hash;

            using (var hasher = SHA256.Create())
            {
                byte[] bytes = hasher.ComputeHash(stoSpan);
                hash = Convert.ToHexString(bytes);
            }

            if (hash != file.SHA256)
                throw new Exception($"No match: {file.Path} calc={hash} em={file.SHA256}");
        }
    }
}
