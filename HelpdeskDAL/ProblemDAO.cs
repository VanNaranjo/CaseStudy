using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskDAL
{
    public class ProblemDAO
    {
        readonly IRepository<Problem> _repo;
        public ProblemDAO()
        {
            _repo = new HelpdeskRepository<Problem>();
        }

        public async Task<List<Problem>> GetAll()
        {
            return await _repo.GetAll();
        }

        public async Task<Problem> GetByDescription(string description)
        {

            return (await _repo.GetOne(emp => emp.Description == description))!;

        }

    }
}
