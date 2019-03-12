using Liberator.Lazuli.MinioBuckets.Exceptions;
using Minio.DataModel;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Liberator.Lazuli.MinioBuckets.Client
{
    /// <summary>
    /// Control class for methods pertaining to buckets
    /// </summary>
    public static class BucketMethods
    {
        /// <summary>
        /// Makes a new bucket on the client.
        /// </summary>
        /// <param name="client">The client for the connection.</param>
        /// <param name="bucketName">The name of the bucket.</param>
        /// <param name="region">The region for the client.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>A task status object.</returns>
        public static TaskStatus MakeNewBucket(this LazuliClient client, string bucketName,
                                        [Optional, DefaultParameterValue("us-east-1")] string region,
                                        CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                Task task = client.minioClient.MakeBucketAsync(bucketName, region, cancellationToken);
                task.Wait();
                return task.Status;
            }
            catch (Exception e)
            {
                throw new LazuliBucketException("Unable to create the requested bucket.", e);
            }
        }

        /// <summary>
        /// Lists the buckets on the client.
        /// </summary>
        /// <param name="client">The client for the connection.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>A list of Bucket.</returns>
        public static List<Bucket> ListBuckets(this LazuliClient client,
                                        CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                Task<ListAllMyBucketsResult> listTask = client.minioClient.ListBucketsAsync(cancellationToken);
                listTask.Wait();
                return listTask.Result.Buckets;
            }
            catch (Exception e)
            {
                throw new LazuliBucketException("Unable to list buckets for client.", e);
            }

        }

        /// <summary>
        /// Checks to see if buckets exist.
        /// </summary>
        /// <param name="client">The client for the connection.</param>
        /// <param name="bucketName">The name of the bucket.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>True if the bucket exists.</returns>
        public static bool DoesBucketExist(this LazuliClient client, string bucketName,
                                        CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                Task<bool> foundTask = client.minioClient.BucketExistsAsync(bucketName, cancellationToken);
                foundTask.Wait();
                return foundTask.Result;
            }
            catch (Exception e)
            {
                throw new LazuliBucketException("Could not check for the existence of that bucket.", e);
            }
        }

        /// <summary>
        /// Removes a bucket from the client.
        /// </summary>
        /// <param name="client">The client for the connection.</param>
        /// <param name="bucketName">The name of the bucket.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>Represents the current stage in the lifecycle of a Task.</returns>
        public static TaskStatus RemoveBucket(this LazuliClient client, string bucketName,
                                        CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                Task task = client.minioClient.RemoveBucketAsync(bucketName, cancellationToken);
                task.Wait();
                return task.Status;
            }
            catch (Exception e)
            {
                throw new LazuliBucketException("Unable to remove the named bucket.", e);
            }
        }

        //TODO:Create delegate system to deal with subscriptions
        /// <summary>
        /// Lists the objects within a bucket.
        /// </summary>
        /// <param name="client">The client for the connection.</param>
        /// <param name="bucketName">The name of the bucket.</param>
        /// <param name="prefix">Filters all objects not beginning with a given prefix.</param>
        /// <param name="recursive">Set to false to emulate a directory.</param>
        public static void ListObjects(this LazuliClient client, string bucketName, string prefix, bool recursive)
        {
            try
            {
                //TODO:Work to clean this up and genericise it
                IObservable<Item> observable = client.minioClient.ListObjectsAsync(bucketName, prefix, recursive);
                IDisposable subscription = observable.Subscribe(
                    item => Console.WriteLine("Object: {0}", item.Key),
                    ex => Console.WriteLine("OnError: {0}", ex),
                    () => Console.WriteLine("Listed all objects in bucket " + bucketName + "\n"));
                
                subscription.Dispose();
            }
            catch (Exception e)
            {
                throw new LazuliBucketException("Unable to list objects from the name bucket.", e);
            }

        }

        //TODO:Create delegate system to deal with subscriptions
        /// <summary>
        /// Lists all incolmplete uploads for the bucket
        /// </summary>
        /// <param name="client">The client for the connection.</param>
        /// <param name="bucketName">The name of the bucket.</param>
        /// <param name="prefix">Prefix to list all incomplete uploads.</param>
        /// <param name="recursive">Option to list incomplete uploads recursively.</param>
        public static void ListIncompleteUploads(this LazuliClient client, string bucketName, string prefix, bool recursive = true)
        {
            try
            {
                //TODO:Work to clean this up and genericise it
                IObservable<Upload> observable = client.minioClient.ListIncompleteUploads(bucketName, prefix, recursive);
                IDisposable subscription = observable.Subscribe(
                    item => Console.WriteLine("OnNext: {0}", item.Key),
                    ex => Console.WriteLine("OnError: {0}", ex.Message),
                    () => Console.WriteLine("Listed the pending uploads to bucket " + bucketName));

                Console.Out.WriteLine();
            }
            catch (Exception e)
            {
                throw new LazuliBucketException("Unable to list the incomplete uploads to the named bucket.", e);
            }
        }
    }
}
