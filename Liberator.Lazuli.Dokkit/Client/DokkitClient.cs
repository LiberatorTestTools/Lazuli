using Liberator.Lazuli.Dokkit.Exceptions;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Liberator.Lazuli.Dokkit.Client
{
    /// <summary>
    /// Client for DOkkit communications with Azure DocumentDB
    /// </summary>
    public class DokkitClient
    {
        internal DocumentClient DocumentClient { get; set; }
        internal Uri ServiceEndpoint { get; set; }
        internal string AuthToken { get; set; }
        internal ConnectionPolicy ConnectionPolicy { get; set; }
        internal ConsistencyLevel? ConsistencyLevel { get; set; }
        internal IList<Permission> Permissions { get; set; }
        internal JsonSerializerSettings JsonSerializerSettings { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceEndpoint"></param>
        /// <param name="authToken"></param>
        /// <param name="connectionPolicy"></param>
        /// <param name="consistencyLevel"></param>
        public DokkitClient(Uri serviceEndpoint, string authToken, ConnectionPolicy connectionPolicy, ConsistencyLevel? consistencyLevel)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceEndpoint"></param>
        /// <param name="permissionFeed"></param>
        /// <param name="connectionPolicy"></param>
        /// <param name="consistencyLevel"></param>
        public DokkitClient(Uri serviceEndpoint, IList<Permission> permissionFeed, ConnectionPolicy connectionPolicy, ConsistencyLevel? consistencyLevel)
        {
            try
            {
                ServiceEndpoint = serviceEndpoint;
                Permissions = permissionFeed;
                ConnectionPolicy = connectionPolicy;
                ConsistencyLevel = consistencyLevel;
                DocumentClient = new DocumentClient(serviceEndpoint, permissionFeed, connectionPolicy, consistencyLevel);
            }
            catch (Exception e)
            {
                throw new DokkitException("Unable to establish a connection to the client.", e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceEndpoint"></param>
        /// <param name="authToken"></param>
        /// <param name="serializerSettings"></param>
        /// <param name="connectionPolicy"></param>
        /// <param name="consistencyLevel"></param>
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
