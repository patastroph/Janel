using Janel.Contract.Repository;
using Janel.Data;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System;
using System.Linq;

namespace Janel.Repository {
  public class RoleRepository : IRoleRepository {
    protected IMongoDatabase database => new MongoClient(Configuration.ConnectionString).GetDatabase(AppDomain.CurrentDomain.FriendlyName.Replace(".", ""));
    internal virtual string CollectionName => typeof(IdentityRole).Name;

    public RoleRepository() {
      if (!Configuration.SerializerAsBeenSetted) {
        BsonSerializer.RegisterSerializer(DateTimeSerializer.LocalInstance);

        Configuration.SerializerAsBeenSetted = true;
      }
    }

    public virtual void Delete(IdentityRole entity) {
      Delete(new Guid(entity.Id));
    }

    public virtual void Delete(Guid id) {
      database.GetCollection<Role>(CollectionName).DeleteOne(e => e.Id.Equals(id));
    }

    public virtual IdentityRole GetById(Guid id) {
      return GetList().FirstOrDefault(i => i.Id.Equals(id));
    }
    
    public virtual IQueryable<IdentityRole> GetList() {
      return database.GetCollection<IdentityRole>(CollectionName).AsQueryable();
    }

    public virtual IdentityRole Insert(IdentityRole item) {
      database.GetCollection<IdentityRole>(CollectionName).InsertOne(item);

      return item;
    }

    public virtual IdentityRole Update(IdentityRole item) {
      database.GetCollection<IdentityRole>(CollectionName).ReplaceOne(e => e.Id == item.Id, item);

      return item;
    }

    public IdentityRole GetByName(string name) {
      return GetList().FirstOrDefault(l => l.Name.ToLower() == name.ToLower());
    }
  }
}
