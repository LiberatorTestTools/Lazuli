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

        public Database GetDatabase(string databaseName)
        {
            try
            {
                return Client.CreateDatabaseQuery()
                    .Where(db => db.Id == databaseName)
                    .AsEnumerable()
                    .FirstOrDefault();
            }
            catch (Exception e)
            {
                throw new DokkitException("Could not get a database with that name.", e);
            }
        }

        private DocumentCollection GetCollection(Database database, string collName)
        {
            try
            {
                return Client.CreateDocumentCollectionQuery(database.SelfLink)
                                        .Where(coll => coll.Id == collName)
                                        .ToArray()
                                        .FirstOrDefault();
            }
            catch (Exception e)
            {
                throw new DokkitException("Could not get a collection with that name.", e);
            }
        }

        public bool DoesDatabaseExist(string databaseName)
        {
            try
            {
                return Client.CreateDatabaseQuery()
                    .Where(db => db.Id == databaseName)
                    .AsEnumerable()
                    .Any();
            }
            catch (Exception e)
            {

                throw new DokkitException("Could not ascertain if the named database exists.", e);
            }
        }

        public bool DoesCollectionExist(Database database, string collName)
        {
            try
            {
                return Client.CreateDocumentCollectionQuery(database.SelfLink)
                                .Where(coll => coll.Id == collName)
                                .ToArray()
                                .Any();
            }
            catch (Exception e)
            {
                throw new DokkitException("Could not escertain if the named collection exists.", e);
            }
        }

        public async Task<Database> GetOrCreateDatabase(string databaseName)
        {
            try
            {
                if (DoesDatabaseExist(databaseName))
                {
                    return GetDatabase(databaseName);
                }
                return await Client.CreateDatabaseAsync(new Database
                {
                    Id = databaseName
                });
            }
            catch (Exception e)
            {
                throw new DokkitException("Could not create a database with that name.", e);
            }
        }

        public async Task<DocumentCollection> GetOrCreateCollection(Database database, string collName)
        {
            try
            {
                if (DoesCollectionExist(database, collName))
                {
                    return GetCollection(database, collName);
                }
                return await Client.CreateDocumentCollectionAsync(database.SelfLink,
                   new DocumentCollection
                   {
                       Id = collName
                   });
            }
            catch (Exception e)
            {
                throw new DokkitException("Could not return a collection with that name.", e);
            }
        }

        public async Task<ResourceResponse<Database>> DeleteDatabase(string databaseName)
        {
            try
            {
                if (DoesDatabaseExist(databaseName))
                {
                    return await Client.DeleteDatabaseAsync(databaseName);
                };
                return null;
            }
            catch (Exception e)
            {
                throw new DokkitException("Could not delete the named database.", e);
            }
        }

        public async Task<ResourceResponse<DocumentCollection>> DeleteCollection(Database database, string collName)
        {
            try
            {
                if (DoesCollectionExist(database, collName))
                {
                    return await Client.DeleteDocumentCollectionAsync(collName);
                };
                return null;
            }
            catch (Exception e)
            {
                throw new DokkitException("Could not delete the named collection.", e);
            }
        }
    }
}
