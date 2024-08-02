using LiteDB;
using System.Linq.Expressions;
using UraDocs.ApiService.Domain;

namespace UraDocs.ApiService.Data;

public class DataProvider
{
    private readonly LiteDatabase _database;
    public DataProvider()
    {
        _database = new LiteDatabase("ura.db");
    }

    public ILiteCollection<FileHash> FileHashes => _database.GetCollection<FileHash>();

    public void Update<T>(T entity) where T : class
    {
        _database.GetCollection<T>().Update(entity);
    }

    public T? FirstOrDefault<T>(Expression<Func<T, bool>> predicate) where T : class
    {
        return _database.GetCollection<T>().FindOne(predicate);
    }

    public void Insert<T>(T entity) where T : class
    {
        _database.GetCollection<T>().Insert(entity);
    }

    public void Delete<T>(T entity, Expression<Func<T, object>> idProperty) where T : class
    {
        var id = idProperty.Compile()(entity) as BsonValue;
        _database.GetCollection<T>().Delete(id);
    }

    public bool Exists<T>(Expression<Func<T, bool>> predicate) where T : class
    {
        return _database.GetCollection<T>().Exists(predicate);
    }
}
