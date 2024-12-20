using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HelpdeskDAL;

namespace HelpdeskViewModels
{
    public class DepartmentViewModel
    {
        private readonly DepartmentDao _dao;
      
        public string? Timer { get; set; }
        public string? DepartmentName { get; set; }
        public int? Id { get; set; }

        // constructor
        public DepartmentViewModel()
        {
            _dao = new DepartmentDao();
        }

        //
        // Retrieve all the empdents as ViewModel instances
        //
        public async Task<List<DepartmentViewModel>> GetAll()
        {
            List<DepartmentViewModel> allVms = new();
            try
            {
                List<Department> allDepartments = await _dao.GetAll();
                // we need to convert Department instance to DepartmentViewModel because
                // the Web Layer isn't aware of the Domain class Department
                foreach (Department emp in allDepartments)
                {
                    DepartmentViewModel empVm = new()
                    {
                        
                        Id = emp.Id,
                        DepartmentName = emp.DepartmentName,
                        // binary value needs to be stored on client as base64
                        Timer = Convert.ToBase64String(emp.Timer!)
                    };
                    allVms.Add(empVm);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
            return allVms;
        }

    }

}
