using System;
using System.Linq;
using System.Reflection;
using Janel.Contract.Repository;
using Janel.Data;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Janel.Repository {
  public abstract class BaseMongoDbRepository<BaseType, ConcreteType> : IBaseRepository<BaseType> where BaseType : Entity, new() where ConcreteType : BaseType, new() {
    protected IMongoDatabase database => new MongoClient(Configuration.ConnectionString).GetDatabase(AppDomain.CurrentDomain.FriendlyName.Replace(".", ""));
    internal virtual string CollectionName => typeof(BaseType).Name; 
    
    public BaseMongoDbRepository() {
      if (!Configuration.SerializerAsBeenSetted) {
        BsonSerializer.RegisterSerializer(DateTimeSerializer.LocalInstance);

        Configuration.SerializerAsBeenSetted = true;
      }     
    }
    
    public virtual void Delete(BaseType entity) {
      Delete(entity.Id.Value);
    }

    public virtual void Delete(Guid id) {
      database.GetCollection<ConcreteType>(CollectionName).DeleteOne(e => e.Id.Equals(id));
    }

    public virtual BaseType GetById(Guid id) {
      return GetList().FirstOrDefault(i => i.Id.Equals(id));
    }

    public abstract BaseType GetByName(string name);

    public virtual IQueryable<BaseType> GetList() {
      return database.GetCollection<ConcreteType>(CollectionName).AsQueryable();
    }

    public virtual BaseType Insert(BaseType item) {
      var concreteType = BeforeSave(item);

      concreteType.Id = Guid.NewGuid();

      database.GetCollection<ConcreteType>(CollectionName).InsertOne(concreteType);
      
      return item;
    }

    public virtual BaseType Update(BaseType item) {
      var concreteType = BeforeSave(item);

      database.GetCollection<ConcreteType>(CollectionName).ReplaceOne(e => e.Id == concreteType.Id, concreteType);

      return item;
    }

    public virtual ConcreteType BeforeSave(BaseType item) {
      if (typeof(BaseType) != typeof(ConcreteType)) {
        return CopyObject(item);
      }

      return item as ConcreteType;
    }

    private ConcreteType CopyObject(BaseType item) {
      var copy = new ConcreteType();

      var fields = typeof(BaseType).GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

      foreach (var fi in fields) {
        fi.SetValue(copy, fi.GetValue(item));
      }

      return copy;
    }
  }
}
