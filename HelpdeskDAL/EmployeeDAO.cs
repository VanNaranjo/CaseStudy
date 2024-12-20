using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Reflection;

namespace HelpdeskDAL
{
    
    public class EmployeeDAO {
        readonly IRepository<Employee> _repo;
        public EmployeeDAO()
        {
            _repo = new HelpdeskRepository<Employee>();
        }


        public async Task<Employee> GetByEmail(string? email)
        {
            return (await _repo.GetOne(emp => emp.Email == email))!;

        }


        public async Task<Employee> GetById(int id)
        {

           return (await _repo.GetOne(emp => emp.Id == id))!;

        }



        public async Task<List<Employee>> GetAll()
        {
           
            return await _repo.GetAll();
            
        }


        public async Task<int> Add(Employee newEmployee)
        {
            return (await _repo.Add(newEmployee)).Id;

        }


        public async Task<UpdateStatus> Update(Employee updatedEmployee)
        {
            return (await _repo.Update(updatedEmployee))!;

        }



        public async Task<int> Delete(int? id)
        {
            return await _repo.Delete((int)id!);

        }


        public async Task<Employee> GetByPhoneNumber(string phone)
        {
            return (await _repo.GetOne(stu => stu.PhoneNo == phone))!;

        }

        public async Task<Employee> GetByLastName(string lastname)
        {
            return (await _repo.GetOne(stu => stu.LastName == lastname))!;

        }
    }
}
