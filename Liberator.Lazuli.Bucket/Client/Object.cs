using Liberator.Lazuli.MinioBuckets.Exceptions;
using Minio;
using Minio.DataModel;
using Minio.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Liberator.Lazuli.MinioBuckets.Client
{
    /// <summary>
    /// Base class for methods pertaining to objects.
    /// </summary>
    public static class Object
    {
        //TODO:Create delegate system to deal with subscriptions
        /// <summary>
        /// Gets an object from the client.
        /// </summary>
        /// <param name="minio">The client for the connection.</param>
        /// <param name="bucketName">Bucket to retrieve object from</param>
        /// <param name="objectName">Name of object to retrieve</param>
        /// <param name="fileName">Path to the file.</param>
        /// <param name="sse">Server-side encryption option.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>A task object representing the request.</returns>
        public async static Task GetObject(this MinioClient minio, string bucketName, string objectName, string fileName,
                                                [Optional, DefaultParameterValue(null)] ServerSideEncryption sse,
                                                CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                await minio.GetObjectAsync(bucketName, objectName,
                (stream) =>
                {
                    // Uncommment to print the file on output console
                    // stream.CopyTo(Console.OpenStandardOutput());
                },
                sse,
                cancellationToken);
            }
            catch (Exception e)
            {
                throw new LazuliBucketException("Unable to get the object.", e);
            }
        }


        //TODO:Create delegate system to deal with subscriptions
        /// <summary>
        /// Gets a partial object from the client.
        /// </summary>
        /// <param name="minio">The client for the connection.</param>
        /// <param name="bucketName">Bucket to retrieve object from</param>
        /// <param name="objectName">Name of object to retrieve</param>
        /// <param name="fileName">Path to the file.</param>
        /// <param name="sse">Server-side encryption option.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>A task object representing the request.</returns>
        public async static Task GetPartialObject(this MinioClient minio, string bucketName, string objectName, string fileName,
                                                [Optional, DefaultParameterValue(null)] ServerSideEncryption sse,
                                                CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                // Check whether the object exists using StatObjectAsync(). 
                // If the object is not found, StatObjectAsync() will throw an exception.

                Task<ObjectStat> statTask = minio.StatObjectAsync(bucketName, objectName, sse, cancellationToken);
                statTask.Wait();

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
                },
                sse,
                cancellationToken);
            }
            catch (Exception e)
            {
                throw new LazuliBucketException("Unable to get the partial object.", e);
            }
        }

        /// <summary>
        /// Puts an object stream on the client.
        /// </summary>
        /// <param name="minio">The client for the connection.</param>
        /// <param name="bucketName">Bucket to retrieve object from</param>
        /// <param name="objectName">Name of object to retrieve</param>
        /// <param name="fileName">Path to the file.</param>
        /// <param name="contentType">Content type for the stream.</param>
        /// <param name="metaData">Metadata for the streaming object.</param>
        /// <param name="sse">Server-side encryption option.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>A task object representing the request.</returns>
        public static TaskStatus StreamToBucket(this MinioClient minio, string bucketName, string objectName, string fileName,
                                                        [Optional, DefaultParameterValue("application/octet-stream")] string contentType,
                                                        [Optional, DefaultParameterValue(null)]Dictionary<string, string> metaData,
                                                        [Optional, DefaultParameterValue(null)]ServerSideEncryption sse,
                                                        CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                Task task;
                byte[] bs = File.ReadAllBytes(fileName);
                using (MemoryStream filestream = new MemoryStream(bs))
                {
                    task = minio.PutObjectAsync(bucketName, objectName, filestream, filestream.Length, contentType, metaData, sse, cancellationToken);
                    task.Wait();
                }
                return task.Status;
            }
            catch (Exception e)
            {
                throw new LazuliBucketException("Unable to stream to the named bucket.", e);
            }
        }

        /// <summary>
        /// Gets statistics for an object on the server.
        /// </summary>
        /// <param name="minio">The client for the connection.</param>
        /// <param name="bucketName">Bucket to retrieve object statistics from</param>
        /// <param name="bucketObject">Name of object to retrieve statistics for.</param>
        /// <param name="sse"></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>A task object representing the request.</returns>
        public static ObjectStat GetStatistics(this MinioClient minio, string bucketName, string bucketObject,
                                                        [Optional, DefaultParameterValue(null)]ServerSideEncryption sse,
                                                        CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                Task<ObjectStat> statTask = minio.StatObjectAsync(bucketName, bucketObject, sse, cancellationToken);
                statTask.Wait();
                return statTask.Result;
            }
            catch (Exception e)
            {
                throw new LazuliBucketException("Unable to get statistics for the object.", e);
            }
        }

        /// <summary>
        /// Removes an object from the server.
        /// </summary>
        /// <param name="minio">The client for the connection.</param>
        /// <param name="bucketName">Bucket to retrieve object from</param>
        /// <param name="objectName">Name of object to retrieve</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>A task object representing the request.</returns>
        public static TaskStatus RemoveObject(this MinioClient minio, string bucketName, string objectName,
                                                CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                Task task = minio.RemoveObjectAsync(bucketName, objectName, cancellationToken);
                task.Wait();
                return task.Status;
            }
            catch (Exception e)
            {
                throw new LazuliBucketException("Unable to remove the named object.", e);
            }
        }

        //TODO:Create delegate system to deal with subscriptions
        /// <summary>
        /// Removes multiple objects from the client.
        /// </summary>
        /// <param name="minio">The client for the connection.</param>
        /// <param name="bucketName">Bucket to retrieve object from</param>
        /// <param name="objectsList">The list of objects to remove.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>True if the method completes.</returns>
        public static bool RemoveMultipleObjects(this MinioClient minio, string bucketName, List<string> objectsList,
                                                        CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                Task<IObservable<DeleteError>> observableTask = minio.RemoveObjectAsync(bucketName, objectsList, cancellationToken);
                observableTask.Wait();
                var observable = observableTask.Result;
                IDisposable subscription = observable.Subscribe(
                   deleteError => Console.WriteLine("Object: {0}", deleteError.Key),
                   ex => Console.WriteLine("OnError: {0}", ex),
                   () =>
                   {
                       Console.WriteLine("Listed all delete errors for remove objects on  " + bucketName + "\n");
                   });
                return true;
            }
            catch (Exception e)
            {
                throw new LazuliBucketException("Unable to remove one or all of the named objects.", e);
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
        public static TaskStatus CopyObject(this MinioClient minio, string fromBucketName, string fromObjectName, string destBucketName,
                                        string destObjectName, ServerSideEncryption sseSrc, ServerSideEncryption sseDest)
        {
            try
            {
                Task task = minio.CopyObjectAsync(fromBucketName, fromObjectName, destBucketName, destObjectName, copyConditions: null, sseSrc: sseSrc, sseDest: sseDest);
                task.Wait();
                return task.Status;
            }
            catch (Exception e)
            {
                throw new LazuliBucketException("Unable to copy the object.", e);
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
        /// <param name="metadata"></param>
        /// <param name="sourceSse">Server-side encryption for the source file.</param>
        /// <param name="destSse">Server-side encryption for the destination file.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>A task object representing the request.</returns>
        public static TaskStatus CopyObjectMetadata(this MinioClient minio, string fromBucketName, string fromObjectName, string destBucketName, string destObjectName,
                                                        [Optional, DefaultParameterValue(null)] Dictionary<string, string> metadata,
                                                        [Optional, DefaultParameterValue(null)]ServerSideEncryption sourceSse,
                                                        [Optional, DefaultParameterValue(null)]ServerSideEncryption destSse,
                                                        CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                CopyConditions copyCond = new CopyConditions();
                copyCond.SetReplaceMetadataDirective();

                Task task = minio.CopyObjectAsync(fromBucketName, fromObjectName, destBucketName, destObjectName, copyCond, metadata, sourceSse, destSse, cancellationToken);
                task.Wait();
                return task.Status;
            }
            catch (Exception e)
            {
                throw new LazuliBucketException("Unable to copy the object and metadata.", e);
            }
        }

        /// <summary>
        /// Remove an incomplete upload from the client.
        /// </summary>
        /// <param name="minio">The client for the connection.</param>
        /// <param name="bucketName">Bucket to retrieve object from</param>
        /// <param name="objectName">Name of object to retrieve</param>
        /// <param name="cancellationToken"></param>
        /// <returns>A task object representing the request.</returns>
        public static TaskStatus RemoveIncompleteUpload(this MinioClient minio, string bucketName, string objectName,
                                                        CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                Task task = minio.RemoveIncompleteUploadAsync(bucketName, objectName, cancellationToken);
                task.Wait();
                return task.Status;
            }
            catch (Exception e)
            {
                throw new LazuliBucketException("Unable to remove the incomplete object.", e);
            }
        }

        /// <summary>
        /// Gets a presigned object from the client.
        /// </summary>
        /// <param name="minio">The client for the connection.</param>
        /// <param name="bucketName">Bucket to retrieve object from</param>
        /// <param name="objectName">Name of object to retrieve</param>
        /// <param name="expiry"></param>
        /// <param name="reqParams"></param>
        /// <returns>A task object representing the request.</returns>
        public static string GetPresignedObject(this MinioClient minio, string bucketName, string objectName, int expiry,
                                                    [Optional, DefaultParameterValue(null)] Dictionary<string, string> reqParams)        
        {
             try
            {
                Task<string> task = minio.PresignedGetObjectAsync(bucketName, objectName, 1000, reqParams);
                task.Wait();
                return task.Result;
            }
            catch (Exception e)
            {
                throw new LazuliBucketException("Unable to get the presigned object.", e);
            }
        }

        /// <summary>
        /// Puts a presigned object on the client.
        /// </summary>
        /// <param name="minio">The client for the connection.</param>
        /// <param name="bucketName">Bucket to retrieve object from</param>
        /// <param name="objectName">Name of object to retrieve</param>
        /// <returns>A task object representing the request.</returns>
        public static string PutPresignedObject(this MinioClient minio, string bucketName, string objectName)
        {
            try
            {
                Task<string> task = minio.PresignedPutObjectAsync(bucketName, objectName, 1000);
                task.Wait();
                return task.Result;
            }
            catch (Exception e)
            {
                throw new LazuliBucketException("Unable to put the presigned object.", e);
            }
        }
    }
}
