using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace HelpdeskDAL
{
    public interface IRepository<T>
    {
        Task<List<T>> GetAll();
        Task<List<T>> GetSome(Expression<Func<T, bool>> match);
        Task<T?> GetOne(Expression<Func<T, bool>> match);
        Task<T> Add(T entity);
        Task<UpdateStatus> Update(T enity);
        Task<int> Delete(int i);
    }
}