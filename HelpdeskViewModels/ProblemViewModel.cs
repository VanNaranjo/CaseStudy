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
    public class ProblemViewModel
    {
        private readonly ProblemDAO _dao;

        public string? Timer { get; set; }

        public int? Id { get; set; }

        public string? Description { get; set; }
        

   
        public ProblemViewModel()
        {
            _dao = new ProblemDAO();
        }

        public async Task<List<ProblemViewModel>> GetAll()
        {
            List<ProblemViewModel> allVms = new();
            try
            {
                List<Problem> allProblems = await _dao.GetAll();

                foreach (Problem emp in allProblems)
                {
                    ProblemViewModel empVm = new()
                    {
                        Id = emp.Id,
                        Description = emp.Description,
                        Timer = Convert.ToBase64String(emp.Timer!)

                    };
                    allVms.Add(empVm);
                }
            }
            catch (Exception ex) {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
            return allVms;
        }

        public async Task GetByDescription(string description)
        {
            try
            {
                Problem prob = await _dao.GetByDescription(description);
                Id = prob.Id;
                Description = prob.Description;

                //Timer = Convert.ToBase64String(prob.Timer!);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
        }


    }
}
