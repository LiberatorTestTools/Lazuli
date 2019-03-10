using Minio;

namespace Liberator.Lazuli.Minio.Client
{
    /// <summary>
    /// The Client for communication with the bucket
    /// </summary>
    public class Client
    {
        /// <summary>
        /// The Minio Client
        /// </summary>
        MinioClient minioClient = null;

        /// <summary>
        /// Gets a client to allow contact with the Minio installation
        /// </summary>
        /// <param name="endpoint">The endpoint for the minio installation</param>
        /// <returns>A MinioClient object</returns>
        public MinioClient Get(string endpoint)
        {
            minioClient = new MinioClient(endpoint, "", "", "", "");
            return minioClient;
        }
    }
}
