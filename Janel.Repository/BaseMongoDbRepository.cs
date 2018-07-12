using System;
using System.Collections.Generic;
using System.Linq;
using Janel.Contract;
using Janel.Contract.Repository;
using Janel.Data;
using MongoDB.Driver;

namespace Janel.Repository {
  public abstract class BaseMongoDbRepository<T> : IBaseRepository<T> where T : Entity, new() {
    protected const string connectionString = "mongodb://localhost";
    protected IMongoDatabase database => new MongoClient(connectionString).GetDatabase(System.AppDomain.CurrentDomain.FriendlyName.Replace(".", ""));
    protected virtual string CollectionName { get { return this.GetType().Name;  } }

    public void Delete(T entity) {
      Delete(entity.Id);
    }

    public void Delete(Guid id) {
      database.GetCollection<T>(CollectionName).DeleteOne(e => e.Id.Equals(id));
    }

    public T GetById(Guid id) {
      return database.GetCollection<T>(CollectionName).Find(e => e.Id.Equals(id)) as T;
    }

    public abstract T GetByName(string name);

    public IQueryable<T> GetList() {
      return database.GetCollection<T>(CollectionName).AsQueryable();
    }

    public T Insert(T item) {
      item.Id = Guid.NewGuid();

      database.GetCollection<T>(CollectionName).InsertOne(item);
      
      return item;
    }

    public T Update(T item) {
      database.GetCollection<T>(CollectionName).ReplaceOne(e => e.Id == item.Id, item);

      return item;
    }
  }
}
