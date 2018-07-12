using System.Collections.Generic;
using System;
using System.Linq;

namespace Janel.Contract.Repository {
  public interface IBaseRepository<T> where T : class, new() {
    IQueryable<T> GetList();
    T Update(T item);
    T Insert(T item);
    T GetByName(string name);
    T GetById(Guid id);
    void Delete(T entity);
    void Delete(Guid Id);
  }
}
