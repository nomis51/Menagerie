using MongoDB.Driver;

namespace Menagerie.Core.Services
{
    public class MongoService
    {
        #region Props

        protected MongoClient Client { get; }
        protected IMongoDatabase Database { get;  }

        #endregion
        
        #region Constructors

        protected MongoService(string connectionString, string database)
        {
            Client = new MongoClient(connectionString);
            Database = Client.GetDatabase(database);
        }
        #endregion
        
        #region Protected methods

        protected IMongoCollection<T> GetCollection<T>(string name)
        {
            return Database.GetCollection<T>(name);
        }
        #endregion
    }
}