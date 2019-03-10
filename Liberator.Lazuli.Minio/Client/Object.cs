using Minio;
using Minio.DataModel;
using Minio.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Liberator.Lazuli.Minio.Client
{
    /// <summary>
    /// Base class for methods pertaining to objects.
    /// </summary>
    public class Object
    {
        private static readonly int MB = 1024 * 1024;

        /// <summary>
        /// Gets an object from the client.
        /// </summary>
        /// <param name="minio">The client for the connection.</param>
        /// <param name="bucketName">Bucket to retrieve object from</param>
        /// <param name="objectName">Name of object to retrieve</param>
        /// <param name="fileName">Path to the file.</param>
        /// <returns>A task object representing the request.</returns>
        public async static Task Get(MinioClient minio, string bucketName, string objectName, string fileName)
        {
            try
            {
                await minio.GetObjectAsync(bucketName, objectName,
                (stream) =>
                {
                    // Uncommment to print the file on output console
                    // stream.CopyTo(Console.OpenStandardOutput());
                });
            }
            catch (Exception e)
            {
                Console.WriteLine("[Bucket]  Exception: {0}", e);
            }
        }

        /// <summary>
        /// Gets a partial object from the client.
        /// </summary>
        /// <param name="minio">The client for the connection.</param>
        /// <param name="bucketName">Bucket to retrieve object from</param>
        /// <param name="objectName">Name of object to retrieve</param>
        /// <param name="fileName">Path to the file.</param>
        /// <returns>A task object representing the request.</returns>
        public async static Task GetPartial(MinioClient minio, string bucketName, string objectName, string fileName)
        {
            try
            {
                // Check whether the object exists using StatObjectAsync(). If the object is not found,
                // StatObjectAsync() will throw an exception.

                await minio.StatObjectAsync(bucketName, objectName);

                // Get object content starting at byte position 1024 and length of 4096
                await minio.GetObjectAsync(bucketName, objectName, 1024L, 4096L,
                (stream) =>
                {
                    var fileStream = File.Create(fileName);
                    stream.CopyTo(fileStream);
                    fileStream.Dispose();
                    FileInfo writtenInfo = new FileInfo(fileName);
                    long file_read_size = writtenInfo.Length;
                    stream.Dispose();
                });
                Console.Out.WriteLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("[Bucket]  Exception: {0}", e);
            }
        }

        /// <summary>
        /// Puts an object stream on the client.
        /// </summary>
        /// <param name="minio">The client for the connection.</param>
        /// <param name="bucketName">Bucket to retrieve object from</param>
        /// <param name="objectName">Name of object to retrieve</param>
        /// <param name="fileName">Path to the file.</param>
        /// <param name="sse">Server-side encryption option.</param>
        /// <returns>A task object representing the request.</returns>
        public async static Task StreamToBucket(MinioClient minio, string bucketName, string objectName, string fileName, ServerSideEncryption sse)
        {
            try
            {
                byte[] bs = File.ReadAllBytes(fileName);
                using (MemoryStream filestream = new MemoryStream(bs))
                {
                    if (filestream.Length < (5 * MB))
                    {
                        Console.Out.WriteLine("Running example for API: PutObjectAsync with Stream");
                    }
                    else
                    {
                        Console.Out.WriteLine("Running example for API: PutObjectAsync with Stream and MultiPartUpload");
                    }

                    var metaData = new Dictionary<string, string>()
                                    {
                                        {"X-Amz-Meta-Test", "Test  Test"}
                                    };

                    await minio.PutObjectAsync(bucketName, objectName, filestream, filestream.Length, "application/octet-stream", metaData: metaData, sse: sse);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("[Bucket]  Exception: {0}", e);
            }
        }

        /// <summary>
        /// Gets statistics for an object on the server.
        /// </summary>
        /// <param name="minio">The client for the connection.</param>
        /// <param name="bucketName">Bucket to retrieve object statistics from</param>
        /// <param name="bucketObject">Name of object to retrieve statistics for.</param>
        /// <returns>A task object representing the request.</returns>
        public async static Task GetStatistics(MinioClient minio, string bucketName, string bucketObject)
        {
            try
            {
                ObjectStat statObject = await minio.StatObjectAsync(bucketName, bucketObject);
            }
            catch (Exception e)
            {
                Console.WriteLine("[StatObject] {0}-{1}  Exception: {2}", bucketName, bucketObject, e);
            }
        }

        /// <summary>
        /// Removes an object from the server.
        /// </summary>
        /// <param name="minio">The client for the connection.</param>
        /// <param name="bucketName">Bucket to retrieve object from</param>
        /// <param name="objectName">Name of object to retrieve</param>
        /// <returns>A task object representing the request.</returns>
        public async static Task Remove(MinioClient minio, string bucketName, string objectName)
        {
            try
            {
                await minio.RemoveObjectAsync(bucketName, objectName);
            }
            catch (Exception e)
            {
                Console.WriteLine("[Bucket-Object]  Exception: {0}", e);
            }
        }

        /// <summary>
        /// Removes multiple objects from the client.
        /// </summary>
        /// <param name="minio">The client for the connection.</param>
        /// <param name="bucketName">Bucket to retrieve object from</param>
        /// <param name="objectsList"></param>
        /// <returns>A task object representing the request.</returns>
        public async static Task RemoveMultiple(MinioClient minio, string bucketName, List<string> objectsList)
        {
            try
            {
                IObservable<DeleteError> observable = await minio.RemoveObjectAsync(bucketName, objectsList);
                IDisposable subscription = observable.Subscribe(
                   deleteError => Console.WriteLine("Object: {0}", deleteError.Key),
                   ex => Console.WriteLine("OnError: {0}", ex),
                   () =>
                   {
                       Console.WriteLine("Listed all delete errors for remove objects on  " + bucketName + "\n");
                   });
            }
            catch (Exception e)
            {
                Console.WriteLine("[Bucket-Object]  Exception: {0}", e);
            }
        }

        /// <summary>
        /// Copies an object from one bucket to another
        /// </summary>
        /// <param name="minio">The client for the connection.</param>
        /// <param name="fromBucketName">Bucket for the source file.</param>
        /// <param name="fromObjectName">Name of the object to move.</param>
        /// <param name="destBucketName">Bucket for the destination file.</param>
        /// <param name="destObjectName">Name of object after the move.</param>
        /// <param name="sseSrc">Server-side encryption for the source file.</param>
        /// <param name="sseDest">Server-side encryption for the destination file.</param>
        /// <returns>A task object representing the request.</returns>
        public async static Task Copy(MinioClient minio, string fromBucketName, string fromObjectName, string destBucketName,
                                        string destObjectName, ServerSideEncryption sseSrc, ServerSideEncryption sseDest)
        {
            try
            {
                await minio.CopyObjectAsync(fromBucketName, fromObjectName, destBucketName, destObjectName, copyConditions: null, sseSrc: sseSrc, sseDest: sseDest);
            }
            catch (Exception e)
            {
                Console.WriteLine("[Bucket]  Exception: {0}", e);
            }
        }

        /// <summary>
        /// Copies an object from one bucket to another with metadata.
        /// </summary>
        /// <param name="minio">The client for the connection.</param>
        /// <param name="fromBucketName">Bucket for the source file.</param>
        /// <param name="fromObjectName">Name of the object to move.</param>
        /// <param name="destBucketName">Bucket for the destination file.</param>
        /// <param name="destObjectName">Name of object after the move.</param>
        /// <returns>A task object representing the request.</returns>
        public async static Task CopyMetadata(MinioClient minio, string fromBucketName, string fromObjectName, string destBucketName, string destObjectName)
        {
            try
            {
                CopyConditions copyCond = new CopyConditions();
                copyCond.SetReplaceMetadataDirective();
                Dictionary<string, string> metadata = new Dictionary<string, string>()
                {
                    { "Content-Type", "application/css"},
                    {"X-Amz-Meta-Mynewkey","my-new-value"}
                };

                await minio.CopyObjectAsync(fromBucketName, fromObjectName, destBucketName, destObjectName, copyConditions: copyCond, metadata: metadata);
            }
            catch (Exception e)
            {
                Console.WriteLine("[Bucket]  Exception: {0}", e);
            }
        }

        /// <summary>
        /// Remove an incomplete upload from the client.
        /// </summary>
        /// <param name="minio">The client for the connection.</param>
        /// <param name="bucketName">Bucket to retrieve object from</param>
        /// <param name="objectName">Name of object to retrieve</param>
        /// <returns>A task object representing the request.</returns>
        public async static Task RemoveIncomplete(MinioClient minio, string bucketName, string objectName)
        {
            try
            {
                await minio.RemoveIncompleteUploadAsync(bucketName, objectName);
            }
            catch (Exception e)
            {
                Console.WriteLine("[Bucket-Object]  Exception: {0}", e);
            }
        }

        /// <summary>
        /// Gets a presigned object from the client.
        /// </summary>
        /// <param name="minio">The client for the connection.</param>
        /// <param name="bucketName">Bucket to retrieve object from</param>
        /// <param name="objectName">Name of object to retrieve</param>
        /// <returns>A task object representing the request.</returns>
        public async static Task GetPresigned(MinioClient minio, string bucketName = "my-bucket-name", string objectName = "my-object-name")
        {
            try
            {
                Dictionary<string, string> reqParams = new Dictionary<string, string>() { { "response-content-type", "application/json" } };
                string presigned_url = await minio.PresignedGetObjectAsync(bucketName, objectName, 1000, reqParams);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("Exception ", e.Message);
            }
        }

        /// <summary>
        /// Puts a presigned object on the client.
        /// </summary>
        /// <param name="minio">The client for the connection.</param>
        /// <param name="bucketName">Bucket to retrieve object from</param>
        /// <param name="objectName">Name of object to retrieve</param>
        /// <returns>A task object representing the request.</returns>
        public async static Task PutPresigned(MinioClient minio, string bucketName, string objectName)
        {
            try
            {
                string presigned_url = await minio.PresignedPutObjectAsync(bucketName, objectName, 1000);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("Exception ", e.Message);
            }
        }
    }
}
