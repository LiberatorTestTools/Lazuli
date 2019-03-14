using Liberator.Lazuli.Dokkit.Exceptions;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Liberator.Lazuli.Dokkit.Client
{
    public class DokkitFiler
    {
        internal string URI = "";
        internal string Key = "";

        public DocumentClient Client { get; set; }

        public DokkitFiler(string uri, string key)
        {
            URI = uri;
            Key = key;
            Client = new DocumentClient(new Uri(uri), Key);
        }

        public async Task<Database> GetDatabase(string databaseName)
        {
            try
            {
                if (Client.CreateDatabaseQuery()
            .Where(db => db.Id == databaseName)
            .AsEnumerable()
            .Any())
                {
                    return Client.CreateDatabaseQuery()
                        .Where(db => db.Id == databaseName)
                        .AsEnumerable()
                        .FirstOrDefault();
                }
                return await Client.CreateDatabaseAsync(new Database
                {
                    Id = databaseName
                });
            }
            catch (Exception e)
            {
                throw new DokkitException("Could not return a database with that name.", e);
            }
        }

        public async Task<DocumentCollection> GetCollection(Database database, string collName)
        {
            if (Client.CreateDocumentCollectionQuery(database.SelfLink)
                .Where(coll => coll.Id == collName)
                .ToArray()
                .Any())
            {
                return Client.CreateDocumentCollectionQuery(database.SelfLink)
                    .Where(coll => coll.Id == collName)
                    .ToArray()
                    .FirstOrDefault();
            }
            return await Client.CreateDocumentCollectionAsync(database.SelfLink,
               new DocumentCollection
               {
                   Id = collName
               });
        }

        public async Task<ResourceResponse<Database>> DeleteDatabase(string databaseName)
        {
            if (Client.CreateDatabaseQuery()
                .Where(db => db.Id == databaseName)
                .AsEnumerable()
                .Any())
            {
                return await Client.DeleteDatabaseAsync(databaseName);
            };
            return null;
        }

        public async Task<ResourceResponse<DocumentCollection>> DeleteCollection(Database database, string collName)
        {
            if (Client.CreateDocumentCollectionQuery(database.SelfLink)
                .Where(coll => coll.Id == collName)
                .ToArray()
                .Any())
            {
                return await Client.DeleteDocumentCollectionAsync(collName);
            };
            return null;
        }
    }
}
