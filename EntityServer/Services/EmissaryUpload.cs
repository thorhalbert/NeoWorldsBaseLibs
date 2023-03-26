using CanonicalGrpcProtos;
using Grpc.Core;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Policy;

namespace EntityServer.Services
{
    public class EntityEmissaryManagmentServer : EntityEmissaryManagment.EntityEmissaryManagmentBase
    {
        public override async Task<UploadEmissaryPrepareResult> UploadEmissaryPrepare(UploadEmissaryPrepareRequest request, ServerCallContext context)
        {

            var manifest = request.EmissaryManifest;

            var uploadContext = Guid.NewGuid();

            return new UploadEmissaryPrepareResult
            {
                MaxUploadPacket = 1024 * 1024 * 24,
                UploadGuid = uploadContext.ToString()
            };

        }

        public override async Task<UploadEmissaryBodyResult> UploadEmissaryBody(IAsyncStreamReader<UploadEmissaryBodyStream> requestStream, 
            ServerCallContext context)
        {
            bool success = true;

            // We do now have to figure out where the file goes that user is uploading
            // and needs to be stored somewhere we can deal with it if the upload times out
            // or the hash fails.   We probably need some mongo tables here.  We will want to 
            // store the manifest.

            Console.WriteLine($"Current Dir: {Environment.CurrentDirectory}");

            using var hasher = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
            
            // Stream the uploading of the emissary
            await foreach (var message in requestStream.ReadAllAsync())
            {
                // Do this en'passant, so we don't have copy the data (could use the streamhash)
                hasher.AppendData(message.UploadPacket.Span);
            }

            var hash = Convert.ToHexString(hasher.GetCurrentHash());

            // We may also need to validate the manifest

            return new UploadEmissaryBodyResult
            {
                UploadSuccess = success
            };
        }
    }
}
