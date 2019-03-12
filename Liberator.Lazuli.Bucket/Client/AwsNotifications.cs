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
    /// Base class for adding notifications to AWS
    /// </summary>
    public static class AwsNotifications
    {
        /// <summary>
        /// Gets notifications from a bucket
        /// </summary>
        /// <param name="minio">The client for the connection.</param>
        /// <param name="bucketName">The name of the bucket.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>A BucketNotification object</returns>
        public static BucketNotification GetAwsNotifications(this MinioClient minio, string bucketName,
                                                                CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                Task<BucketNotification> notificationTask = minio.GetBucketNotificationsAsync(bucketName, cancellationToken);
                notificationTask.Wait();
                return notificationTask.Result;
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
        /// <param name="configurations">Notification configuration list.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>Represents the current stage in the lifecycle of a Task.</returns>
        public static TaskStatus SetAwsNotifications(this MinioClient minio, string bucketName,
                                                IEnumerable<NotificationConfiguration> configurations,
                                                CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                BucketNotification notification = new BucketNotification();

                foreach (NotificationConfiguration configuration in configurations)
                {
                    switch (configuration.GetType().Name)
                    {
                        case "TopicConfig":
                            notification.AddTopic((TopicConfig)configuration);
                            break;
                        case "LambdaConfig":
                            notification.AddLambda((LambdaConfig)configuration);
                            break;
                        case "QueueConfig":
                            notification.AddQueue((QueueConfig)configuration);
                            break;
                    } 
                }

                Task task = minio.SetBucketNotificationsAsync(bucketName, notification, cancellationToken);
                task.Wait();
                return task.Status;
            }
            catch (Exception e)
            {
                throw new LazuliBucketException("Could not set the AWS Notifications for the bucket.", e);
            }
        }

        /// <summary>
        /// Removes all notifications for a bucket.
        /// </summary>
        /// <param name="minio">The client for the connection.</param>
        /// <param name="bucketName">The name of the bucket.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>Represents the current stage in the lifecycle of a Task.</returns>
        public static TaskStatus RemoveAllAwsNotifications(this MinioClient minio, string bucketName,
                                                        CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                Task task = minio.RemoveAllBucketNotificationsAsync(bucketName, cancellationToken);
                task.Wait();
                return task.Status;
            }
            catch (Exception e)
            {
                throw new LazuliBucketException("Could not remove the AWS Notifications for the bucket.", e);
            }

        }



        #region Configuration Examples

        //private static QueueConfig GetQueueConfiguration()
        //{
        //    QueueConfig queueConfiguration = new QueueConfig("arn:aws:sqs:us-west-1:123434153608:testminioqueue1");
        //    queueConfiguration.AddEvents(new List<EventType>() { EventType.ObjectCreatedCompleteMultipartUpload });
        //    return queueConfiguration;
        //}


        //private static LambdaConfig GetLambdaConfiguration()
        //{
        //    LambdaConfig lambdaConfiguration = new LambdaConfig("arn:aws:lambda:us-west-1:123434153608:function:lambdak1");
        //    lambdaConfiguration.AddEvents(new List<EventType>() { EventType.ObjectRemovedDelete });
        //    lambdaConfiguration.AddFilterPrefix("java");
        //    lambdaConfiguration.AddFilterSuffix("java");
        //    return lambdaConfiguration;
        //}

        //private static TopicConfig GetTopicConfiguration()
        //{
        //    Arn topicArn = new Arn("aws", "sns", "us-west-1", "730234153608", "topicminio");
        //    TopicConfig topicConfiguration = new TopicConfig(topicArn);
        //    List<EventType> events = new List<EventType>() { EventType.ObjectCreatedPut, EventType.ObjectCreatedCopy };
        //    topicConfiguration.AddEvents(events);
        //    topicConfiguration.AddFilterPrefix("images");
        //    topicConfiguration.AddFilterSuffix("pg");
        //    return topicConfiguration;
        //} 

        #endregion
    }
}
