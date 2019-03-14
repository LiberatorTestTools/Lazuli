using Liberator.Lazuli.Dokkit.Exceptions;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Liberator.Lazuli.Dokkit.Client
{
    public class DokkitClient
    {
        internal DocumentClient DocumentClient { get; set; }
        internal Uri ServiceEndpoint { get; set; }
        internal string AuthToken { get; set; }
        internal ConnectionPolicy ConnectionPolicy { get; set; }
        internal ConsistencyLevel ConsistencyLevel { get; set; }


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
    }
}
