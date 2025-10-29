using LiteDB;
using Serilog;

namespace Menagerie.Data.Providers;

public static class DatabaseProvider
{
    #region Constants

    private const string DataFileName = "data.db";
    private const string DataFolder = "%USERPROFILE%/Documents/My Games/Menagerie/";

    #endregion

    #region Members

    private static string _dataFilePath;
    private static readonly object TransactionLock = new();
    private static readonly object ConnectionLock = new();
    private static LiteDatabase? _database;

    #endregion

    #region Constructors

    public static void Initialize()
    {
        var path = Environment.ExpandEnvironmentVariables(DataFolder);
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        _dataFilePath = Path.Join(path, DataFileName);
    }

    #endregion

    #region Public methods

    public static bool Delete<T>(string collectionName, string id)
    {
        try
        {
            lock (TransactionLock)
            {
                if (_database is null) Connect();
                if (_database is null) return false;


                var collection = _database.GetCollection<T>(collectionName);
                collection.Delete(id);

                Disconnect();
                Connect();
                return true;
            }
        }
        catch (Exception e)
        {
            Log.Warning("Unable to delete from database {collection}: {message}. {stackTrace}", collectionName, e.Message,
                e.StackTrace);
        }

        return false;
    }

    public static bool Save<T>(string collectionName, T data)
    {
        try
        {
            lock (TransactionLock)
            {
                if (_database is null) Connect();
                if (_database is null) return false;

                var collection = _database.GetCollection<T>(collectionName);
                collection.Upsert(data);

                Disconnect();
                Connect();

                return true;
            }
        }
        catch (Exception e)
        {
            Log.Warning("Unable to write to database {collection}: {message}. {stackTrace}", collectionName, e.Message,
                e.StackTrace);
        }

        return false;
    }

    public static bool Insert<T>(string collectionName, T data)
    {
        try
        {
            lock (TransactionLock)
            {
                if (_database is null) Connect();
                if (_database is null) return false;

                var collection = _database.GetCollection<T>(collectionName);
                collection.Insert(data);

                Disconnect();
                Connect();

                return true;
            }
        }
        catch (Exception e)
        {
            Log.Warning("Unable to write to database {collection}: {message}. {stackTrace}", collectionName, e.Message,
                e.StackTrace);
        }

        return false;
    }

    public static ILiteCollection<T>? Query<T>(string collectionName)
    {
        try
        {
            lock (TransactionLock)
            {
                if (_database is null) Connect();

                return _database?.GetCollection<T>(collectionName);
            }
        }
        catch (Exception e)
        {
            Log.Warning("Unable to write to database {collection}: {message}. {stackTrace}", collectionName, e.Message,
                e.StackTrace);
        }

        return default;
    }

    public static void OnExit()
    {
        Disconnect();
    }

    #endregion

    #region Private methods

    private static void Disconnect()
    {
        lock (ConnectionLock)
        {
            if (_database is null) return;
            _database.Dispose();
            _database = null;
        }
    }

    private static void Connect()
    {
        lock (ConnectionLock)
        {
            _database = new LiteDatabase(_dataFilePath);
        }
    }

    #endregion
}