using Liberator.Lazuli.Minio.Exceptions;
using Minio;
using Minio.DataModel;
using System;
using System.Threading.Tasks;

namespace Liberator.Lazuli.Minio.Client
{
    /// <summary>
    /// Control class for methods pertaining to buckets
    /// </summary>
    public class Bucket
    {
        /// <summary>
        /// Makes a new bucket on the client.
        /// </summary>
        /// <param name="minio">The client for the connection.</param>
        /// <param name="bucketName">The name of the bucket.</param>
        /// <returns>A task object representing the request.</returns>
        public async static Task Make(MinioClient minio, string bucketName)
        {
            try
            {
                await minio.MakeBucketAsync(bucketName);
            }
            catch (Exception e)
            {
                throw new LazuliBucketException("Unable to create the requested bucket.", e);
            }
        }

        /// <summary>
        /// Lists the buckets on the client.
        /// </summary>
        /// <param name="minio">The client for the connection.</param>
        /// <returns>A task object representing the request.</returns>
        public async static Task List(MinioClient minio)
        {
            try
            {
                ListAllMyBucketsResult list = await minio.ListBucketsAsync();
            }
            catch (Exception e)
            {
                throw new LazuliBucketException("Unable to list buckets for client.", e);
            }

        }

        /// <summary>
        /// Checks to see if buckets exist.
        /// </summary>
        /// <param name="minio">The client for the connection.</param>
        /// <param name="bucketName">The name of the bucket.</param>
        /// <returns>A task object representing the request.</returns>
        public async static Task Exists(MinioClient minio, string bucketName)
        {
            try
            {
                bool found = await minio.BucketExistsAsync(bucketName);
            }
            catch (Exception e)
            {
                throw new LazuliBucketException("Could not check for the existence of that bucket.", e);
            }
        }

        /// <summary>
        /// Removes a bucket from the client.
        /// </summary>
        /// <param name="minio">The client for the connection.</param>
        /// <param name="bucketName">The name of the bucket.</param>
        /// <returns>A task object representing the request.</returns>
        public async static Task Remove(MinioClient minio, string bucketName)
        {
            try
            {
                await minio.RemoveBucketAsync(bucketName);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to remove the named bucket.", e);
            }
        }

        /// <summary>
        /// Lists the objects within a bucket.
        /// </summary>
        /// <param name="minio">The client for the connection.</param>
        /// <param name="bucketName">The name of the bucket.</param>
        /// <param name="prefix">Filters all objects not beginning with a given prefix.</param>
        /// <param name="recursive">Set to false to emulate a directory.</param>
        public static void ListObjects(MinioClient minio, string bucketName, string prefix, bool recursive)
        {
            try
            {
                IObservable<Item> observable = minio.ListObjectsAsync(bucketName, prefix, recursive);
                IDisposable subscription = observable.Subscribe(
                    item => Console.WriteLine("Object: {0}", item.Key),
                    ex => Console.WriteLine("OnError: {0}", ex),
                    () => Console.WriteLine("Listed all objects in bucket " + bucketName + "\n"));
                
                subscription.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine("[Bucket]  Exception: {0}", e);
            }

        }

        /// <summary>
        /// Lists all incolmplete uploads for the bucket
        /// </summary>
        /// <param name="minio">The client for the connection.</param>
        /// <param name="bucketName">The name of the bucket.</param>
        /// <param name="prefix">Prefix to list all incomplete uploads.</param>
        /// <param name="recursive">Option to list incomplete uploads recursively.</param>
        public static void ListIncompleteUploads(MinioClient minio, string bucketName, string prefix, bool recursive = true)
        {
            try
            {
                IObservable<Upload> observable = minio.ListIncompleteUploads(bucketName, prefix, recursive);
                IDisposable subscription = observable.Subscribe(
                    item => Console.WriteLine("OnNext: {0}", item.Key),
                    ex => Console.WriteLine("OnError: {0}", ex.Message),
                    () => Console.WriteLine("Listed the pending uploads to bucket " + bucketName));

                Console.Out.WriteLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e);
            }
        }
    }
}
