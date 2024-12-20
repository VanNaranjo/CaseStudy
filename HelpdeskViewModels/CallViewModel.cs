using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HelpdeskDAL;
using Microsoft.Identity.Client;

namespace HelpdeskViewModels
{
    public class CallViewModel
    {
        private readonly CallDAO _dao;
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int ProblemId { get; set; }
        public string? EmployeeName { get; set; }
        public string? ProblemDescription { get; set; }
        public string? TechName { get; set; }
        public int TechId { get; set; }
        public DateTime DateOpened { get; set; }
        public DateTime? DateClosed { get; set; }
        public bool OpenStatus { get; set; }
        public string? Notes { get; set; }
        public string? Timer { get; set; }

        public CallViewModel()
        {
            _dao = new CallDAO();
        }

        public async Task<List<CallViewModel>> GetAll()
        {
            List<CallViewModel> allVms = new();
            try
            {
                List<Call> allCalls = await _dao.GetAll();

                foreach (Call call in allCalls)
                {
                    CallViewModel callVm = new()
                    {
                        Id = call.Id,
                        EmployeeId = call.EmployeeId,
                        ProblemId = call.ProblemId,
                        EmployeeName = call.Employee.FirstName + " " + call.Employee.LastName,
                        ProblemDescription = call.Problem.Description,
                        TechName = call.Tech.FirstName + " " + call.Tech.LastName,
                        TechId = call.TechId,
                        DateOpened = call.DateOpened,
                        DateClosed = call.DateClosed,
                        OpenStatus = call.OpenStatus,
                        Notes = call.Notes,
                        Timer = Convert.ToBase64String(call.Timer!)
                    };
                    allVms.Add(callVm);
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

        public async Task GetById(int id)
        {
            try
            {
                Call call = await _dao.GetById(id);
                Id = call.Id;
                EmployeeId = call.EmployeeId;
                ProblemId = call.ProblemId;
                EmployeeName = call.Employee.FirstName + " " + call.Employee.LastName ;
                ProblemDescription = call.Problem.Description;
                TechName = call.Tech.FirstName + " " + call.Tech.LastName;
                TechId = call.TechId;
                DateOpened = call.DateOpened;
                DateClosed = call.DateClosed;
                OpenStatus = call.OpenStatus;
                Notes = call.Notes;
                Timer = Convert.ToBase64String(call.Timer!);
            }

            catch (NullReferenceException nex)
            {
                Debug.WriteLine(nex.Message);
                TechName = "Not Found";
            }
            catch (Exception ex)
            {
                TechName = "not found";
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
        }

        public async Task Add()
        {
            Id = -1;
            try
            {
                Call call = new()
                {
                    ProblemId = ProblemId,
                    EmployeeId = EmployeeId,
                    TechId = TechId,
                    DateOpened = DateTime.Now,
                    OpenStatus = true,
                    Notes = Notes!
                };

                Id = await _dao.Add(call);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
        }

        public async Task<int> Update()
        {
            int updateStatus;
            try
            {
                Call call = new()
                {
                    Id = Id,
                    ProblemId = ProblemId,
                    EmployeeId = EmployeeId,
                    TechId = TechId,
                    DateOpened = DateOpened,
                    OpenStatus = OpenStatus,
                    Notes = Notes!,
                    Timer = Convert.FromBase64String(Timer!),
                    DateClosed = null
                };
                if (!OpenStatus && DateClosed == null)
                    call.DateClosed = DateTime.Now;

                updateStatus = -1;
                updateStatus = Convert.ToInt16(await _dao.Update(call));

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
            return updateStatus;
        }

        public async Task<int> Delete()
        {
            try
            {
                return await _dao.Delete(Id);
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
