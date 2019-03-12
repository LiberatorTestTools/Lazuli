using Liberator.Lazuli.MinioBuckets.Exceptions;
using Minio;
using Minio.DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Liberator.Lazuli.MinioBuckets.Client
{
    /// <summary>
    /// Base class for file object methods.
    /// </summary>
    public static class FileObject
    {
        /// <summary>
        /// Get an object. The object will be streamed to the callback given by the user.
        /// </summary>
        /// <param name="client">The client for the connection.</param>
        /// <param name="bucketName">Bucket to retrieve object from</param>
        /// <param name="objectName">Name of object to retrieve</param>
        /// <param name="fileName">Path to the file.</param>
        /// <param name="sse">Server-side encryption option.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>A task object representing the request.</returns>
        public static TaskStatus DownloadToFile(this LazuliClient client, string bucketName, string objectName, string fileName,
                                                    [Optional, DefaultParameterValue(null)] ServerSideEncryption sse,
                                                    CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                File.Delete(fileName);
                Task task = client.minioClient.GetObjectAsync(bucketName, objectName, fileName, sse, cancellationToken);
                task.Wait();
                return task.Status;
            }
            catch (Exception e)
            {
                throw new LazuliBucketException("Unable to download the object.", e);
            }
        }

        /// <summary>
        /// Creates an object from file
        /// </summary>
        /// <param name="client">The client for the connection.</param>
        /// <param name="bucketName">Bucket to retrieve object from</param>
        /// <param name="objectName">Name of object to retrieve</param>
        /// <param name="fileName">Path to the file.</param>
        /// <param name="contentType">The type of content being uploaded.</param>
        /// <param name="metadata">Metadata for the file.</param>
        /// <param name="sse">Server-side encryption option.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>A task object representing the request.</returns>
        public static TaskStatus UploadFromFile(this LazuliClient client, string bucketName, string objectName, string fileName,
                                                [Optional, DefaultParameterValue("application/octet-stream")] string contentType,
                                                [Optional, DefaultParameterValue(null)] Dictionary<string, string> metadata,
                                                [Optional, DefaultParameterValue(null)] ServerSideEncryption sse,
                                                CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                Task task = client.minioClient.PutObjectAsync(bucketName, objectName, fileName, contentType, metadata, sse, cancellationToken);
                task.Wait();
                return task.Status;
            }
            catch (Exception e)
            {
                throw new LazuliBucketException("Unable to upload the file.", e);
            }
        }
    }
}
