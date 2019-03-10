using Liberator.Lazuli.MinioBuckets.Exceptions;
using Minio;
using Minio.DataModel;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Liberator.Lazuli.MinioBuckets.Client
{
    /// <summary>
    /// Base class for bucket policy operations
    /// </summary>
    public static class Policy
    {
        /// <summary>
        /// Gets the policy stored on the server for the bucket
        /// </summary>
        /// <param name="minio">The client for the connection.</param>
        /// <param name="bucketName">The name of the bucket.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>An asynchronous task representing the operation</returns>
        public static string GetPolicy(this MinioClient minio, string bucketName,
                                        CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                Task<string> policyTask = minio.GetPolicyAsync(bucketName, cancellationToken);
                policyTask.Wait();
                return policyTask.Result;
            }
            catch (Exception e)
            {
                throw new LazuliBucketException("Unable to get the policy for the bucket.", e);
            }
        }

        /// <summary>
        /// Sets the policy stored on the server for the bucket
        /// </summary>
        /// <param name="minio">The client for the connection.</param>
        /// <param name="bucketName">The name of the bucket.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>An asynchronous task representing the operation</returns>
        public static string SetPolicy(this MinioClient minio, string bucketName,
                                        CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                Task<string> policyTask = minio.GetPolicyAsync(bucketName, cancellationToken);
                policyTask.Wait();
                return policyTask.Result;
            }
            catch (Exception e)
            {
                throw new LazuliBucketException("Unable to set the policy for the bucket.", e);
            }
        }

        /// <summary>
        /// Posts a presigned object to the client.
        /// </summary>
        /// <param name="minio">The client for the connection.</param>
        /// <param name="bucketName">Bucket to retrieve object from</param>
        /// <param name="objectName">Name of object to retrieve</param>
        /// <returns>An asynchronous task representing the operation</returns>
        public async static Task PresignedPostPolicy(MinioClient minio, string bucketName, string objectName)
        {
            try
            {
                PostPolicy form = new PostPolicy();
                DateTime expiration = DateTime.UtcNow;
                form.SetExpires(expiration.AddDays(10));
                form.SetKey(objectName);
                form.SetBucket(bucketName);
                Tuple<string, Dictionary<string, string>> tuple = await minio.PresignedPostPolicyAsync(form);
                string curlCommand = "curl -X POST ";
                foreach (KeyValuePair<string, string> pair in tuple.Item2)
                {
                    curlCommand = curlCommand + String.Format(" -F {0}={1}", pair.Key, pair.Value);
                }

                curlCommand = curlCommand + " -F file=@/etc/bashrc " + tuple.Item1; // https://s3.amazonaws.com/my-bucketname";
            }
            catch (Exception e)
            {
                throw new LazuliBucketException("Unable to post the presigned object.", e);
            }
        }
    }
}
