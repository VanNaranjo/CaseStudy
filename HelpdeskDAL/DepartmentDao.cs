using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Reflection;

namespace HelpdeskDAL
{

    public class DepartmentDao
    {
        readonly IRepository<Department> _repo;
        public DepartmentDao()
        {
            _repo = new HelpdeskRepository<Department>();
        }

        public async Task<List<Department>> GetAll()
        {

            return await _repo.GetAll();

        }

    }
}
