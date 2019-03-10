using Liberator.Lazuli.Minio.Exceptions;
using Minio;
using Minio.DataModel;
using System;
using System.Threading.Tasks;

namespace Liberator.Lazuli.Minio.Client
{
    /// <summary>
    /// Base class for adding notifications to AWS
    /// </summary>
    public static class AwsNotifications
    {
        /// <summary>
        /// Gets notifications from a bucket
        /// </summary>
        /// <param name="minio">The client for the connection.</param>
        /// <param name="bucketName">The name of the bucket.</param>
        /// <returns>A task object representing the request.</returns>
        public async static Task GetAwsNotifications(this MinioClient minio, string bucketName)
        {
            try
            {
                BucketNotification notifications = await minio.GetBucketNotificationsAsync(bucketName);
            }
            catch (Exception e)
            {
                throw new LazuliBucketException("Error parsing bucket notifications", e);
            }
        }

        /// <summary>
        /// Sets notifications for a bucket
        /// </summary>
        /// <param name="minio">The client for the connection.</param>
        /// <param name="bucketName">The name of the bucket.</param>
        /// <returns>A task object representing the request.</returns>
        public async static Task SetAwsNotifications(MinioClient minio, string bucketName)
        {
            try
            {
                BucketNotification notification = new BucketNotification();

                // Uncomment the code below and change Arn and event types to configure.
                /* 
                Arn topicArn = new Arn("aws", "sns", "us-west-1", "730234153608", "topicminio");
                TopicConfig topicConfiguration = new TopicConfig(topicArn);
                List<EventType> events = new List<EventType>(){ EventType.ObjectCreatedPut , EventType.ObjectCreatedCopy };
                topicConfiguration.AddEvents(events);
                topicConfiguration.AddFilterPrefix("images");
                topicConfiguration.AddFilterSuffix("pg");
                notification.AddTopic(topicConfiguration);



                LambdaConfig lambdaConfiguration = new LambdaConfig("arn:aws:lambda:us-west-1:123434153608:function:lambdak1");
                lambdaConfiguration.AddEvents(new List<EventType>() { EventType.ObjectRemovedDelete });
                lambdaConfiguration.AddFilterPrefix("java");
                lambdaConfiguration.AddFilterSuffix("java");
                notification.AddLambda(lambdaConfiguration);
                
                QueueConfig queueConfiguration = new QueueConfig("arn:aws:sqs:us-west-1:123434153608:testminioqueue1");
                queueConfiguration.AddEvents(new List<EventType>() { EventType.ObjectCreatedCompleteMultipartUpload });
                notification.AddQueue(queueConfiguration);
                */

                await minio.SetBucketNotificationsAsync(bucketName, notification);
            }
            catch (Exception e)
            {
                throw new LazuliBucketException("Could not set the AWS Notifications for the bucket.", e);
            }
        }

        /// <summary>
        /// Removes all notifications for a bucket
        /// </summary>
        /// <param name="minio">The client for the connection.</param>
        /// <param name="bucketName">The name of the bucket.</param>
        /// <returns>A task object representing the request.</returns>
        public async static Task RemoveAllAwsNotifications(MinioClient minio, string bucketName)
        {
            try
            {
                await minio.RemoveAllBucketNotificationsAsync(bucketName);
            }
            catch (Exception e)
            {
                throw new LazuliBucketException("Could not remove the AWS Notifications for the bucket.", e);
            }

        }
    }
}
