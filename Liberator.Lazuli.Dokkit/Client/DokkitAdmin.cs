using Liberator.Lazuli.Dokkit.Exceptions;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Liberator.Lazuli.Dokkit.Client
{
    /// <summary>
    /// Administrates your Dokkits
    /// </summary>
    public class DokkitAdmin
    {
        internal string URI = "";
        internal string Key = "";

        /// <summary>
        /// The document client being used
        /// </summary>
        public DocumentClient Client { get; set; }

        /// <summary>
        /// The default constructor
        /// </summary>
        /// <param name="uri">The URL for the client.</param>
        /// <param name="key">The access key.</param>
        public DokkitAdmin(string uri, string key)
        {
            URI = uri;
            Key = key;
            Client = new DocumentClient(new Uri(uri), Key);
        }

        /// <summary>
        /// Gets a database, if it exists
        /// </summary>
        /// <param name="databaseName">The name of the database</param>
        /// <returns>The requested database.</returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="database"></param>
        /// <param name="collName"></param>
        /// <returns></returns>
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


        private StoredProcedure GetStoredProcedure(StoredProcedure storedProcedure, string sprocName)
        {
            try
            {
                return Client.CreateStoredProcedureQuery(storedProcedure.SelfLink)
                                .Where(sp => sp.Id == sprocName)
                                .ToArray()
                                .FirstOrDefault();
            }
            catch (Exception e)
            {
                throw new DokkitException("Could not get a collection with that name.", e);
            }
        }

        /// <summary>
        /// Checks for the existence of a database on the client.
        /// </summary>
        /// <param name="databaseName"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Checks for the existence of a collection on the database.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="collName"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Checks for the existence of a stored procedure for the collection.
        /// </summary>
        /// <param name="collection">The Document Collection in which the stored procedure is found.</param>
        /// <param name="sprocName">The name of the stored procedure.</param>
        /// <returns>A boolean which is true of the SPROC exists.</returns>
        public bool DoesStoredProcedureExist(DocumentCollection collection, string sprocName)
        {
            try
            {
                return Client.CreateStoredProcedureQuery(collection.SelfLink)
                    .Where(sp => sp.Id == sprocName)
                    .ToArray()
                    .Any();
            }
            catch (Exception e)
            {
                throw new DokkitException("", e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseName"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="database"></param>
        /// <param name="collName"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets a named stored procedure from a collection
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="sprocName"></param>
        /// <returns></returns>
        public async Task<StoredProcedure> GetStoredProcedure(DocumentCollection collection, string sprocName)
        {
            try
            {
                if (DoesStoredProcedureExist(collection, sprocName))
                {
                    return await GetStoredProcedure(collection, sprocName);
                }
                return null;
            }
            catch (Exception e)
            {
                throw new DokkitException("Could not return the named stored procedure.", e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="sproc"></param>
        /// <returns></returns>
        public async Task<StoredProcedure> GetOrCreateStoredProcedure(DocumentCollection collection, StoredProcedure sproc)
        {
            try
            {
                if (DoesStoredProcedureExist(collection, sproc.Id))
                {
                    return await GetStoredProcedure(collection, sproc.Id);
                }
                return await Client.CreateStoredProcedureAsync(sproc.Id, sproc);
            }
            catch (Exception e)
            {
                throw new DokkitException("Could not return the named stored procedure.", e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseName"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="database"></param>
        /// <param name="collName"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="sprocName"></param>
        /// <returns></returns>
        public async Task<ResourceResponse<StoredProcedure>> DeleteStoredProcedure(DocumentCollection collection, string sprocName)
        {
            try
            {
                if (DoesStoredProcedureExist(collection, sprocName))
                {
                    return await Client.DeleteStoredProcedureAsync(sprocName);
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
