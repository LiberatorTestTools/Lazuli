﻿using Minio;
using Minio.DataModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Liberator.Lazuli.Minio.Client
{
    /// <summary>
    /// Base class for bucket policy operations
    /// </summary>
    public class Policy
    {
        /// <summary>
        /// Gets the policy stored on the server for the bucket
        /// </summary>
        /// <param name="minio">The client for the connection.</param>
        /// <param name="bucketName">The name of the bucket.</param>
        /// <returns>An asynchronous task representing the operation</returns>
        public async static Task Get(MinioClient minio, string bucketName)
        {
            try
            {
                string policyJson = await minio.GetPolicyAsync(bucketName);
            }
            catch (Exception e)
            {
                Console.WriteLine("[Bucket]  Exception: {0}", e);
            }
        }

        /// <summary>
        /// Sets the policy stored on the server for the bucket
        /// </summary>
        /// <param name="minio">The client for the connection.</param>
        /// <param name="bucketName">The name of the bucket.</param>
        /// <returns>An asynchronous task representing the operation</returns>
        public async static Task Set(MinioClient minio, string bucketName)
        {
            try
            {
                String policyJson = await minio.GetPolicyAsync(bucketName);
            }
            catch (Exception e)
            {
                Console.WriteLine("[Bucket]  Exception: {0}", e);
            }
        }

        /// <summary>
        /// Posts a presigned object to the client.
        /// </summary>
        /// <param name="minio">The client for the connection.</param>
        /// <param name="bucketName">Bucket to retrieve object from</param>
        /// <param name="objectName">Name of object to retrieve</param>
        /// <returns>An asynchronous task representing the operation</returns>
        public async static Task PresignedPost(MinioClient minio, string bucketName, string objectName)
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
                Console.Out.WriteLine("Exception ", e.Message);
            }
        }
    }
}
