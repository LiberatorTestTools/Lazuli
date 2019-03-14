using Liberator.Lazuli.Dokkit.Exceptions;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using System;

namespace Liberator.Lazuli.Dokkit.Client
{
    public class DokkitClient
    {
        internal DocumentClient DocumentClient { get; set; }
        internal Uri ServiceEndpoint { get; set; }
        internal string AuthToken { get; set; }
        internal ConnectionPolicy ConnectionPolicy { get; set; }
        internal ConsistencyLevel ConsistencyLevel { get; set; }
        internal JsonSerializerSettings JsonSerializerSettings { get; set; }


        public DokkitClient(Uri serviceEndpoint, string authToken, ConnectionPolicy connectionPolicy, ConsistencyLevel consistencyLevel)
        {
            try
            {
                ServiceEndpoint = serviceEndpoint;
                AuthToken = authToken;
                ConnectionPolicy = connectionPolicy;
                ConsistencyLevel = consistencyLevel;
                DocumentClient = new DocumentClient(serviceEndpoint, authToken, connectionPolicy, consistencyLevel);
            }
            catch (Exception e)
            {
                throw new DokkitException("Unable to establish a connection to the client.", e);
            }
        }


        public DokkitClient(Uri serviceEndpoint, string authToken, JsonSerializerSettings serializerSettings, ConnectionPolicy connectionPolicy, ConsistencyLevel consistencyLevel)
        {
            try
            {
                ServiceEndpoint = serviceEndpoint;
                AuthToken = authToken;
                JsonSerializerSettings = serializerSettings;
                ConnectionPolicy = connectionPolicy;
                ConsistencyLevel = consistencyLevel;
                DocumentClient = new DocumentClient(serviceEndpoint, authToken, serializerSettings, connectionPolicy, consistencyLevel);
            }
            catch (Exception e)
            {
                throw new DokkitException("Unable to establish a connection to the client.", e);
            }
        }
    }
}
