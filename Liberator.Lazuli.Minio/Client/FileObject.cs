using Liberator.Lazuli.Minio.Exceptions;
using Minio;
using Minio.DataModel;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Liberator.Lazuli.Minio.Client
{
    /// <summary>
    /// Base class for file object methods.
    /// </summary>
    public class FileObject
    {
        /// <summary>
        /// Get an object. The object will be streamed to the callback given by the user.
        /// </summary>
        /// <param name="minio">The client for the connection.</param>
        /// <param name="bucketName">Bucket to retrieve object from</param>
        /// <param name="objectName">Name of object to retrieve</param>
        /// <param name="fileName">Path to the file.</param>
        /// <param name="sse">Server-side encryption option.</param>
        /// <returns>A task object representing the request.</returns>
        public async static Task DownloadToFile(MinioClient minio, string bucketName, string objectName, string fileName, ServerSideEncryption sse)
        {
            try
            {
                File.Delete(fileName);
                await minio.GetObjectAsync(bucketName, objectName, fileName, sse: sse);
            }
            catch (Exception e)
            {
                throw new LazuliBucketException("Unable to download the object.", e);
            }
        }

        /// <summary>
        /// Creates an object from file
        /// </summary>
        /// <param name="minio">The client for the connection.</param>
        /// <param name="bucketName">Bucket to retrieve object from</param>
        /// <param name="objectName">Name of object to retrieve</param>
        /// <param name="fileName">Path to the file.</param>
        /// <returns>A task object representing the request.</returns>
        public async static Task UploadFromFile(MinioClient minio, string bucketName, string objectName, string fileName)
        {
            try
            {
                await minio.PutObjectAsync(bucketName, objectName, fileName, contentType: "application/octet-stream");
            }
            catch (Exception e)
            {
                throw new LazuliBucketException("Unable to upload the file.", e);
            }
        }
    }
}
