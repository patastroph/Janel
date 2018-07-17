﻿using Janel.Contract.Repository;
using Janel.Data;
using System;
using System.Linq;

namespace Janel.Repository
{
  public class PersonRepository : BaseMongoDbRepository<Person, Person>, IPersonRepository {
    public override Person GetByName(string name) {
      return GetList().FirstOrDefault(l => l.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
  }
}
