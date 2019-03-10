using Minio;
using System.Runtime.InteropServices;

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
        /// <param name="accessKey">Access Key for authenticated requests (Optional)</param>
        /// <param name="secretKey">Secret Key for authenticated requests (Optional)</param>
        /// <param name="region">Optional custom region</param>
        /// <param name="sessionToken">Optional session token</param>
        /// <returns>A MinioClient object</returns>
        public MinioClient Get(string endpoint,
                                [Optional, DefaultParameterValue(null)] string accessKey,
                                [Optional, DefaultParameterValue(null)] string secretKey,
                                [Optional, DefaultParameterValue(null)] string region,
                                [Optional, DefaultParameterValue(null)] string sessionToken)
        {
            minioClient = new MinioClient(endpoint, accessKey, secretKey, region, sessionToken);
            return minioClient;
        }
    }
}
